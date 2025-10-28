using CloudCANetCore.BO;
using DigitalSignService.Business.Service3th;
using DigitalSignService.Common;
using DigitalSignService.DAL.DTOs.Requests;
using DigitalSignService.DAL.DTOs.Requests.Sign;
using DigitalSignService.DAL.DTOs.Responses;
using DigitalSignService.DAL.DTOs.Responses.SignDTOs;
using DigitalSignService.DAL.Models;
using DPUStorageService.APIs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SignService.Common.HashSignature.Interface;
using SignService.Common.HashSignature.Pdf;

namespace DigitalSignService.Business.Services.Sign
{
    public class ViettelSigningProvider : BaseSigningProvider
    {
        private const string OID_NIST_SHA1 = "1.3.14.3.2.26";
        private const string OID_NIST_SHA256 = "2.16.840.1.101.3.4.2.1";
        private const string OID_RSA_RSA = "1.2.840.113549.1.1.1";
        //
        private readonly ViettelSignApi _viettelSignApi;

        public override string Name => "viettel";

        public ViettelSigningProvider(ILogger<ViettelSigningProvider> _logger, IOptions<DigitalSignSettings> settings, CachingService cachingService, ViettelSignApi viettelSignApi, IApiStorage apiStorage, IOptions<AppSetting> options1) : base(_logger, settings, cachingService, apiStorage, options1)
        {
            _viettelSignApi = viettelSignApi;
        }

        public override async Task<string> SignCAPDF(SignReq req, CancellationToken cancellationToken = default)
        {
            var fileByteArr = await this.DownloadFileAsync(req.DocumentInfo.Url, 200);
            if (fileByteArr == null)
            {
                return string.Empty;
            }
            return await this.SignCAPDF(req, _defaultSettings.Viettel.ClientId, _defaultSettings.Viettel.ClientSecret, _defaultSettings.Viettel.ProfileId, fileByteArr, cancellationToken);
        }

        public override async Task<SignStatusDTO> GetSignStatus(SignReq req, string transactionId, CancellationToken cancellationToken = default)
        {
            return await this.GetSignStatus(req, _defaultSettings.Viettel.ClientId, _defaultSettings.Viettel.ClientSecret, _defaultSettings.Viettel.ProfileId, transactionId, cancellationToken);
        }

        public override async Task<SignStatusDTO> HandleWebhook(string dataString, string documentName, CancellationToken cancellationToken)
        {
            try
            {
                var statusRes = new SignStatusDTO();
                var data = JsonConvert.DeserializeObject<VTWebhookData>(dataString);
                if (data is not null)
                {
                    Enum.TryParse<Enums.SigningStatus>(data.MetaData.Status, out var result);
                    statusRes.SigningStatus = result;
                    statusRes.TransactionId = data.MetaData.TransactionId;
                    var documentUrl = await this.AddSignatureToFile(data.Data.Results[0].Signature, data.MetaData.TransactionId, documentName, cancellationToken);
                    if (String.IsNullOrEmpty(documentUrl))
                    {
                        throw new Exception("Cannot get file detail from cloud, please contact with admin");
                    }
                    statusRes.DocumentUrl = documentUrl;
                }
                return statusRes;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override async Task<List<SignatureValidationResult>> VerifyPdf(string url, CancellationToken cancellationToken = default)
        {
            return await base.VerifyPdf(url);
        }

        private async Task<SignStatusDTO> GetSignStatus(SignReq req, string clientId, string clientSecret, string profileId, string transactionId, CancellationToken cancellationToken = default)
        {
            var token = await this.Login(req.UserSign.Id, clientId, clientSecret, profileId);
            if (token == null)
            {
                _logger.LogError("ERROR: Login");
                throw new Exception("Login error");
            }
            VTSignHashRes? signHashRes = await GetSignStatus(transactionId, token.AccessToken);

            if (signHashRes == null)
            {
                _logger.LogError("ERROR: Sign Hash");
                throw new Exception("Sign hash error");
            }
            else if (signHashRes.Error != null || signHashRes.ErrorDescription != null)
            {
                string log = "ERROR: Sign Hash: " + "\n"
                        + "Error: " + signHashRes.Error + "\n"
                        + "Error: " + signHashRes.ErrorDescription;
                _logger.LogError(log);
                throw new Exception(log);
            }

            var statusRes = new SignStatusDTO
            {
                TransactionId = transactionId
            };

            Enum.TryParse<Enums.SigningStatus>(signHashRes.Status.ToString(), out var result);
            statusRes.SigningStatus = result;

            if (signHashRes.Signatures == null || signHashRes.Signatures.Length == 0 || signHashRes.Status != 1)
                return statusRes;

            var documentUrl = await this.AddSignatureToFile(signHashRes.Signatures[0], transactionId, req.DocumentInfo.Name, cancellationToken);
            if (String.IsNullOrEmpty(documentUrl))
            {
                throw new Exception("Cannot get file detail from cloud, please contact with admin");
            }
            statusRes.DocumentUrl = documentUrl;

            return statusRes;
        }

        private async Task<string?> AddSignatureToFile(string datasigned, string transactionId, string documentName, CancellationToken cancellationToken = default)
        {
            try
            {
                IHashSigner? signer = null;
                string? credentialId = null;
                var cacheSigner = _cachingService.GetCacheSigner(transactionId);
                if (cacheSigner.HasValue && cacheSigner.Value.Item1 != null && !String.IsNullOrEmpty(cacheSigner.Value.Item2) && cacheSigner.Value.Item3 != 0)
                {
                    signer = cacheSigner.Value.Item1;
                    credentialId = cacheSigner.Value.Item2;
                    _cachingService.Remove($"Signer-{transactionId}");
                }
                else
                {
                    throw new Exception("Can not get signer, please try again");
                    //var fileByteArr = await this.DownloadFileAsync(req.DocumentInfo.Url);
                    //if (fileByteArr == null)
                    //    throw new Exception("Download file error");

                    //var signerTmp = await this.Signer(req.UserSign, clientId, clientSecret, profileId, fileByteArr, token.AccessToken);
                    //signer = signerTmp?.Item1;
                    //credentialId = signerTmp?.Item2;
                }

                if (signer == null || String.IsNullOrEmpty(credentialId))
                {
                    _logger.LogError("ERROR: Get Signer");
                    throw new Exception("Get signer error");
                }
                if (!signer.CheckHashSignature(datasigned))
                {
                    _logger.LogError("ERROR: Signature not match");
                    throw new Exception("Signature not match");
                }
                // Xử lý file đã ký: lưu file tạm lên cloud, bắn message cho webhook gởi về lại cho client
                byte[] signed = signer.Sign(datasigned);
                using (var stream = new MemoryStream(signed))
                {
                    var res = await _apiStorage.UploadFile(_appSetting.BucketDigitalSign, transactionId, stream, documentName, cancellationToken: cancellationToken);
                    if (res is null)
                    {
                        throw new Exception("Cannot save file to cloud, please contact with admin");
                    }

                    return await _apiStorage.GetUrlDownload(_appSetting.BucketDigitalSign, $"{transactionId}/{documentName}", res.VersionId, cancellationToken: cancellationToken);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<string> SignCAPDF(SignReq req, string clientId, string clientSecret, string profileId, byte[] file, CancellationToken cancellationToken = default)
        {
            var token = await this.Login(req.UserSign.Id, clientId, clientSecret, profileId);
            if (token == null)
            {
                _logger.LogError("ERROR: Login");
                return string.Empty;
            }

            var signerTmp = await this.Signer(req.UserSign, clientId, clientSecret, profileId, file, token.AccessToken, req.Location);
            var signer = signerTmp?.Item1;
            var credentialId = signerTmp?.Item2;
            if (signer == null || String.IsNullOrEmpty(credentialId))
            {
                _logger.LogError("ERROR: Get Signer");
                return string.Empty;
            }
            var hashValue = signer.GetSecondHashAsBase64();
            string[] hashList = new string[1];
            hashList[0] = hashValue;

            string? transactionId = await this.SignHash(hashList, req.DocumentInfo.Id, req.DocumentInfo.Name, clientId, clientSecret, credentialId, token.AccessToken);
            if (String.IsNullOrEmpty(transactionId))
            {
                Console.WriteLine("Ký không thành công");
                return string.Empty;
            }

            // Lưu Signer vào cache để dùng khi get status
            // Note: Cache capacity validation is already handled at TemplateService level
            _cachingService.SetCacheSigner(transactionId, signer, credentialId, file.LongLength);

            return transactionId;
        }

        private async Task<(IHashSigner, string)?> Signer(UserSignReq req, string clientId, string clientSecret, string profileId, byte[] file, string accessToken, string location = "Location")
        {
            var certMap = await this.GetAllCertificates(req.Id, clientId, clientSecret, profileId, accessToken);
            if (certMap == null || certMap.Count == 0)
            {
                _logger.LogError("ERROR: Not found Certificate of User");
                return null;
            }
            var lastCert = certMap.Last();
            if (!String.IsNullOrEmpty(req.SerialNumber))
                lastCert = certMap.FirstOrDefault(c => c.Value.SerialNumber != null && c.Value.SerialNumber.Equals(req.SerialNumber, StringComparison.OrdinalIgnoreCase));
            if (lastCert.Key is null)
                throw new Exception("Cannot find certificate with given serial number");

            string credentialId = lastCert.Key;
            string x509Cert = lastCert.Value.Certificates.FirstOrDefault()!;

            IHashSigner signer = HashSignerFactory.GenerateSigner(file, x509Cert, null, HashSignerFactory.PDF);
            signer.SetHashAlgorithm(SignService.Common.HashSignature.Common.MessageDigestAlgorithm.SHA256);

            // Property: Lý do ký số
            ((PdfHashSigner)signer).SetReason(req.Reason);

            // Property: set định dạng ký
            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);

            // Property: set Font
            ((PdfHashSigner)signer).SetFontName(PdfHashSigner.FontName.Times_New_Roman);
            ((PdfHashSigner)signer).SetFontSize(10);
            // Property: set image
            var imgStream = await this.DownloadFileAsync(req.Img);
            if (imgStream is null)
                throw new Exception("Cannot download signature image");

            if (req.Rotate.HasValue)
            {
                imgStream = RotateImage(imgStream, req.Rotate.Value);
            }

            ((PdfHashSigner)signer).SetCustomImage(imgStream);
            // Property: set location
            ((PdfHashSigner)signer).SetLocation(location);

            foreach (var item in req.UserSignPositions)
            {
                // Hiển thị ảnh chữ ký tại nhiều vị trí trên tài liệu
                int coorXSignature1 = item.CoorX; //Tọa độ X
                int coorYSignature1 = item.CoorY; //Tọa độ Y
                int width = item.Width; //Chiều dài vùng ký
                int height = item.Height; //Chiều cao vùng ký
                string rectangle = coorXSignature1 + "," + coorYSignature1 + ","
                                    + (coorXSignature1 + width) + "," + (coorYSignature1 + height);
                for (int i = item.StartPage; i <= item.EndPage; i++)
                {
                    ((PdfHashSigner)signer).AddSignatureView(new PdfSignatureView
                    {
                        // Nhập thông tin Rectangle dạng string theo công thức: "x,y,x+width,y+height"
                        Rectangle = rectangle,
                        Page = i
                    });
                }
            }

            return (signer, credentialId);
        }

        private async Task<Dictionary<string, VTCertRes>> GetAllCertificates(string userId, string clientId, string clientSecret, string profileId, string accessToken)
        {
            Dictionary<string, VTCertRes> certList = new Dictionary<string, VTCertRes>();
            try
            {
                VTCredentialsListRes? credentialsListRes = await this.GetCredentials(clientId, clientSecret, profileId, userId, accessToken);
                if (credentialsListRes == null)
                {
                    _logger.LogError("ERROR: Get Credentials list");
                    return certList;
                }
                else if (credentialsListRes.Error != null || credentialsListRes.ErrorDescription != null)
                {
                    _logger.LogError("ERROR: Get Credentials list: " + "\n"
                            + "Error: " + credentialsListRes.Error + "\n"
                            + "Error: " + credentialsListRes.ErrorDescription
                    );
                    return certList;
                }
                if (credentialsListRes.CredentialInfos == null || credentialsListRes.CredentialInfos.Length == 0)
                {
                    _logger.LogError("ERROR: Not found Certificate of User");
                    return certList;
                }
                foreach (VTCredentialsInfoRes item in credentialsListRes.CredentialInfos)
                {
                    certList.Add(item.CredentialId, item.Cert);
                }

                return certList;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when GetAllCertificates");
            }

            return certList;
        }

        private async Task<VTLoginRes?> Login(string userId, string clientId, string clientSecret, string profileId)
        {
            try
            {
                VTLoginReq request = new VTLoginReq
                {
                    ClientId = clientId,
                    UserId = userId,
                    ClientSecret = clientSecret,
                    ProfileId = profileId
                };

                return await _viettelSignApi.Login(request);
            }
            catch (Exception e)
            {
                _logger.LogError("Error: " + e.Message);
                return null;
            }
        }

        private async Task<VTCredentialsListRes?> GetCredentials(string clientId, string clientSecret, string profileId, string userId, string accessToken)
        {
            try
            {
                VTCredentialsReq request = new VTCredentialsReq
                {
                    ClientId = clientId,
                    UserId = userId,
                    ClientSecret = clientSecret,
                    ProfileId = profileId,
                    Certificates = "chain",
                    CertInfo = true,
                    AuthInfo = true
                };

                return await _viettelSignApi.GetCredentials(request, accessToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Error: " + e.Message);
                return null;
            }
        }

        private async Task<string?> SignHash(string[] hashList, string id, string dataDisplay, string clientId, string clientSecret, string credentialID, string accessToken)
        {
            try
            {
                int numSignatures = hashList.Length;
                VTDocumentReq[] documents = new VTDocumentReq[hashList.Length];
                for (int i = 0; i < hashList.Length; i++)
                {
                    documents[i] = new VTDocumentReq(id, dataDisplay);
                }

                string hashAlgo = OID_NIST_SHA1;
                string hash = hashList[0];
                if (hash != null && hash.Length != 28)
                {
                    hashAlgo = OID_NIST_SHA256;
                }

                string signAlgo = OID_RSA_RSA;
                VTSignHashAsyncRes? signHashRes = await SignHash(clientId, clientSecret, credentialID, accessToken, numSignatures, documents, hashList, hashAlgo, signAlgo);

                if (signHashRes == null)
                {
                    _logger.LogError("ERROR: Sign Hash");
                    return null;
                }
                else if (signHashRes.Error != null || signHashRes.ErrorDescription != null)
                {
                    _logger.LogError("ERROR: Sign Hash: " + "\n"
                            + "Error: " + signHashRes.Error + "\n"
                            + "Error: " + signHashRes.ErrorDescription
                    );
                    return null;
                }
                if (signHashRes.TransactionId == null)
                {
                    _logger.LogError("ERROR: Sign Hash");
                    return null;
                }

                return signHashRes.TransactionId;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when SignHash");
            }

            return null;
        }

        private async Task<VTSignHashAsyncRes?> SignHash(string clientId, string clientSecret, string credentialId,
            string accessToken, int numSignatures, VTDocumentReq[] documents, string[] hashs, string hashAlgo, string signAlgo)
        {
            try
            {
                VTSignHashReq request = new VTSignHashReq
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    CredentialId = credentialId,
                    NumSignatures = numSignatures,
                    Documents = documents,
                    Hash = hashs,
                    HashAlgo = hashAlgo,
                    SignAlgo = signAlgo,
                    Async = 2 // need to replace 3: webhook
                };

                return await _viettelSignApi.SignHash(request, accessToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Error: " + e.Message);
                return null;
            }
        }

        private async Task<VTSignHashRes?> GetSignStatus(string transactionId, string accessToken)
        {
            try
            {
                return await _viettelSignApi.GetSignStatus(transactionId, accessToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when GetSignStatus");
                return null;
            }
        }
    }
}
