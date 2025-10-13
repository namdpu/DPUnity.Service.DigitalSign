namespace DigitalSignService.Common
{
    public class Enums
    {
        public enum PaperSizeType
        {
            Custom = -1,
            A0,
            A1,
            A2,
            A3,
            A4,
            A5,
            A6,
            A7,
            A8,
            A9,
            A10
        }

        public enum SigningStatus
        {
            /// <sumary>
            /// 0 - In queue
            /// </sumary>
            InQueue = 0,

            /// <summary>
            /// 1 – Signing succeeded
            /// </summary>
            Success = 1,

            /// <summary>
            /// 4000 – Waiting for user confirmation
            /// </summary>
            WaitingForUserConfirmation = 4000,

            /// <summary>
            /// 6000 – User confirmed, system is signing
            /// </summary>
            SigningInProgress = 6000,

            /// <summary>
            /// 4001 – Signing request timed out
            /// </summary>
            Timeout = 4001,

            /// <summary>
            /// 4002 – User rejected the signing request
            /// </summary>
            UserRejected = 4002,

            /// <summary>
            /// 4004 – Signing failed due to an error
            /// </summary>
            SignFailed = 4004,

            /// <summary>
            /// 4005 – Insufficient balance or signing quota
            /// </summary>
            InsufficientBalance = 4005,

            /// <summary>
            /// 13004 – Digital certificate expired or revoked
            /// </summary>
            CertificateExpiredOrRevoked = 13004,

            /// <summary>
            /// 50000 – Error occurred while retrieving information
            /// </summary>
            FetchInfoError = 50000
        }
    }
}
