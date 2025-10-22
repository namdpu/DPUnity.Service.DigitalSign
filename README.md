# Digital Sign Service API Documentation

## Giới thiệu

Digital Sign Service là dịch vụ ký số điện tử hỗ trợ ký PDF với nhiều nhà cung cấp (Viettel, VNPT). Dịch vụ cho phép ký điện tử các file PDF với chữ ký số hợp pháp.

---

## Base URL

```
Production: https://api.yourdomain.com/digitalsign
Development: https://localhost:5001
```

## Authentication

Service sử dụng **JWT Bearer Token** với scheme `AuthGateway`.

### Headers yêu cầu

```http
Authorization: Bearer <your-jwt-token>
Content-Type: application/json
```

---

## Digital Signing APIs

### 1. Ký File PDF

**Endpoint:**
```http
POST /api/DigitalSign/Sign
```

**Request Body:**

```json
{
  "templateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "documentInfo": {
    "id": "doc-12345",
    "name": "contract.pdf",
    "url": "https://storage.example.com/documents/contract.pdf"
  },
  "userSign": {
    "id": "user-001",
    "img": "https://storage.example.com/signatures/signature1.png",
    "reason": "Tôi đồng ý với nội dung hợp đồng",
    "serialNumber": "01234567890",
    "rotate": 0,
    "userSignPositions": [
      {
        "coorX": 100,
        "coorY": 500,
        "width": 150,
        "height": 50,
        "startPage": 1,
        "endPage": 1
      }
    ]
  }
}
```

**Field Descriptions:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `templateId` | string (GUID) | No | ID của template nếu muốn sử dụng template có sẵn|
| `documentInfo.id` | string | Yes | ID của document trong hệ thống của bạn |
| `documentInfo.name` | string | Yes | Tên file PDF (phải có extension .pdf) |
| `documentInfo.url` | string | Yes | URL để download file PDF cần ký (phải accessible, hỗ trợ HTTP HEAD hoặc Range request) |
| `userSign.id` | string | Yes | ID chữ ký của người dùng (Có thể là CCCD hoặc chứng minh thư) |
| `userSign.img` | string | Yes | URL hình ảnh chữ ký (PNG/JPG, recommend < 200KB) |
| `userSign.reason` | string | Yes | Lý do ký (VD: "Tôi đồng ý", "Đã xem xét và chấp thuận") |
| `userSign.serialNumber` | string | No | Số serial của chứng thư số (nếu có) |
| `userSign.rotate` | integer | No | Góc xoay page trong file pdf: 0, 90, 180, 270 |
| `userSign.userSignPositions` | array | Yes* | Mảng các vị trí ký trên PDF. *Required nếu không có templateId |

**UserSignPosition Object:**

| Field | Type | Description |
|-------|------|-------------|
| `coorX` | integer | Tọa độ X (pixels, tính từ gốc tọa độ của trang cần ký) |
| `coorY` | integer | Tọa độ Y (pixels, tính từ góc tọa độ của trang cần ký) |
| `width` | integer | Chiều rộng hình ảnh chữ ký (pixels) |
| `height` | integer | Chiều cao hình ảnh chữ ký (pixels) |
| `startPage` | integer | Số trang bắt đầu ký (trang đầu tiên là 1) |
| `endPage` | integer | Số trang kết thúc ký |

**Response Success (200):**

```json
{
  "statusCode": 200,
  "message": "Sign pdf successfully",
  "data": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "errorCode": ""
}
```

**Response Data:** `transactionId` - Dùng để tra cứu trạng thái ký và tải file

**Response Errors:**

| Status | Message | Error Code | Mô tả |
|--------|---------|------------|-------|
| 400 | Only support pdf file | INVALID_FILE_TYPE | File không phải là PDF |
| 400 | Cannot retrieve file information | DOWNLOAD_FAILED | Không thể lấy thông tin file từ URL |
| 403 | Cannot process document due to storage limit | STORAGE_LIMIT_EXCEEDED | File size vượt quá giới hạn cache hiện tại |
| 404 | Cannot find template with id xxx | TEMPLATE_NOT_FOUND | Template ID không tồn tại |
| 500 | Error when sign | INTERNAL_ERROR | Lỗi hệ thống |

**Example Request (cURL):**

```bash
curl -X POST "https://api.yourdomain.com/api/DigitalSign/Sign" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "documentInfo": {
      "id": "contract-001",
      "name": "employment-contract.pdf",
      "url": "https://storage.example.com/contracts/contract-001.pdf"
    },
    "userSign": {
      "id": "user-nguyen-van-a",
      "img": "https://storage.example.com/signatures/nguyen-van-a.png",
      "reason": "Tôi đồng ý với hợp đồng lao động",
      "serialNumber": "0123456789",
      "userSignPositions": [
        {
          "coorX": 100,
          "coorY": 700,
          "width": 200,
          "height": 60,
          "startPage": 1,
          "endPage": 1
        }
      ]
    }
  }'
```

---

### 2. Kiểm tra trạng thái ký

**Endpoint:**
```http
GET /api/DigitalSign/GetSignStatus?transactionId={transactionId}
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `transactionId` | string (GUID) | Yes | Transaction ID nhận được từ API Sign |

**Response Success (200):**

**Khi đang xử lý (status != 1):**
```json
{
  "statusCode": 200,
  "message": "Get sign status successfully",
  "data": {
    "signingStatus": 4000,
    "documentUrl": null,
    "transactionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  },
  "errorCode": ""
}
```

**Khi ký thành công (status = 1):**
```json
{
  "statusCode": 200,
  "message": "Get sign status successfully",
  "data": {
    "signingStatus": 1,
    "documentUrl": "https://hcm.s3storage.vn/digital-sign/50d70d35-c89b-4abb-8dfd-b62e4d19ceb4/document.pdf?X-Amz-Expires=86400&X-Amz-Algorithm=AWS4-HMAC-SHA256&...",
    "transactionId": "50d70d35-c89b-4abb-8dfd-b62e4d19ceb4"
  },
  "errorCode": ""
}
```

**Important:** Khi `signingStatus = 1` (Success), response sẽ **tự động trả về `documentUrl`** - đây là URL của file đã ký với **thời hạn 24 giờ** (86400 giây).

**Signing Status Values:**

| Status Code | Enum | Mô tả | Action |
|------------|------|-------|--------|
| 1 | Success | ✅ Ký thành công | `documentUrl` đã có trong response, download ngay (expired sau 24h) |
| 4000 | WaitingForUserConfirmation | Chờ người dùng xác nhận trên app provider | Thông báo user mở app để confirm |
| 6000 | SigningInProgress | Đang thực hiện ký | Tiếp tục poll |
| 4001 | Timeout | ❌ Hết thời gian chờ | Cần ký lại |
| 4002 | UserRejected | ❌ Người dùng từ chối | User đã reject, cần ký lại nếu muốn |
| 4004 | SignFailed | ❌ Ký thất bại | Kiểm tra log, có thể retry |
| 4005 | InsufficientBalance | ❌ Không đủ số dư/quota | User cần nạp thêm tại provider |
| 13004 | CertificateExpiredOrRevoked | ❌ Chứng thư hết hạn/bị thu hồi | User cần gia hạn chứng thư số |
| 50000 | FetchInfoError | ❌ Lỗi khi lấy thông tin | Lỗi tạm thời, có thể retry |

**Response Errors:**

| Status | Message | Error Code |
|--------|---------|------------|
| 404 | Cannot find history sign with transaction xxx | TRANSACTION_NOT_FOUND |
| 400 | File was not sign | INVALID_STATE |

**Example Request (cURL):**

```bash
curl -X GET "https://api.yourdomain.com/api/DigitalSign/GetSignStatus?transactionId=a1b2c3d4-e5f6-7890-abcd-ef1234567890" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Best Practices:**

1. **Polling Interval**: Poll status mỗi 5-10 giây
2. **Timeout**: Dừng poll sau 2 phút nếu không có kết quả
3. **Status Handling**:
   - Status `4000, 6000`: Tiếp tục poll
   - Status `1`: ✅ **File đã ký có sẵn trong `documentUrl`** - Download ngay (expired sau 24h)
   - Status `4001, 4002, 4004, 4005, 13004, 50000`: Thông báo lỗi và dừng poll
4. **Download File**: 
   - Khi status = 1, `documentUrl` đã có trong response
   - URL có thời hạn 24 giờ, nên download và lưu trữ ngay
   - Không cần gọi thêm API `GetFileSigned` nếu đã có `documentUrl`

---

### 3. Tải file đã ký (Generate New URL)

**Endpoint:**
```http
GET /api/DigitalSign/GetFileSigned?transactionId={transactionId}
```

**Mục đích:** API này dùng để **generate URL mới** với thời hạn 24 giờ cho file đã ký, phục vụ các trường hợp:
- URL từ `GetSignStatus` đã expired (sau 24h)
- Cần tạo URL mới để download lại file
- Share file cho người khác

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `transactionId` | string (GUID) | Yes | Transaction ID nhận được từ API Sign |

**Response Success (200):**

```json
{
  "statusCode": 200,
  "message": "Get file sign successfully",
  "data": "https://hcm.s3storage.vn/digital-sign/50d70d35-c89b-4abb-8dfd-b62e4d19ceb4/document.pdf?X-Amz-Expires=86400&X-Amz-Algorithm=AWS4-HMAC-SHA256&...",
  "errorCode": ""
}
```

**Response Data:** URL mới của file PDF đã ký với **thời hạn 24 giờ** (86400 giây)

**Response Errors:**

| Status | Message | Error Code | Mô tả |
|--------|---------|------------|-------|
| 404 | Cannot find history sign with transaction xxx | TRANSACTION_NOT_FOUND | Transaction ID không tồn tại |
| 400 | File was not sign | FILE_NOT_READY | File chưa được ký (status != Success) |

**Example Request (cURL):**

```bash
curl -X GET "https://api.yourdomain.com/api/DigitalSign/GetFileSigned?transactionId=a1b2c3d4-e5f6-7890-abcd-ef1234567890" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Important Notes:**

⚠️ **Không bắt buộc gọi API này** - Khi `signingStatus = 1`, file URL đã có trong response của `GetSignStatus`  
⚠️ **Chỉ cần gọi khi:** URL từ `GetSignStatus` đã expired (sau 24h) hoặc cần tạo URL mới  
⚠️ **URL thời hạn:** Mỗi URL có thời hạn 24 giờ (X-Amz-Expires=86400)  
⚠️ **Best practice:** Lưu file về storage của bạn ngay sau khi ký thành công, đừng phụ thuộc vào presigned URL

---

## Use Cases

### Use Case 1: Ký tại 1 vị trí cố định

Ký chữ ký tại góc dưới bên phải trang 1:

```json
{
  "documentInfo": {
    "id": "doc-001",
    "name": "document.pdf",
    "url": "https://storage.example.com/document.pdf"
  },
  "userSign": {
    "id": "user-001",
    "img": "https://storage.example.com/signature.png",
    "reason": "Approved",
    "userSignPositions": [
      {
        "coorX": 400,
        "coorY": 50,
        "width": 150,
        "height": 50,
        "startPage": 1,
        "endPage": 1
      }
    ]
  }
}
```

### Use Case 2: Ký tại nhiều vị trí (multi-page)

Ký chữ ký tại trang 1, trang 3 và trang 5:

```json
{
  "documentInfo": {
    "id": "doc-002",
    "name": "contract.pdf",
    "url": "https://storage.example.com/contract.pdf"
  },
  "userSign": {
    "id": "user-001",
    "img": "https://storage.example.com/signature.png",
    "reason": "Reviewed and approved",
    "userSignPositions": [
      {
        "coorX": 100,
        "coorY": 700,
        "width": 200,
        "height": 60,
        "startPage": 1,
        "endPage": 1
      },
      {
        "coorX": 100,
        "coorY": 700,
        "width": 200,
        "height": 60,
        "startPage": 3,
        "endPage": 3
      },
      {
        "coorX": 400,
        "coorY": 100,
        "width": 150,
        "height": 50,
        "startPage": 5,
        "endPage": 5
      }
    ]
  }
}
```

### Use Case 3: Ký với chữ ký xoay 90 độ

```json
{
  "documentInfo": {
    "id": "doc-003",
    "name": "landscape.pdf",
    "url": "https://storage.example.com/landscape.pdf"
  },
  "userSign": {
    "id": "user-001",
    "img": "https://storage.example.com/signature.png",
    "reason": "Approved",
    "rotate": 90,
    "userSignPositions": [
      {
        "coorX": 50,
        "coorY": 400,
        "width": 50,
        "height": 150,
        "startPage": 1,
        "endPage": 1
      }
    ]
  }
}
```

**Note:** Khi dùng `templateId`, vị trí ký sẽ được lấy từ template, không cần truyền `userSignPositions`