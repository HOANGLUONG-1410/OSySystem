namespace Services.DTOs.Common
{
    /// <summary>
    /// DTO chuẩn dùng cho các thông báo đơn giản từ API.
    /// Sử dụng class thay cho anonymous object để hỗ trợ đầy đủ XML Content Negotiation (XmlSerializer).
    /// </summary>
    public class ApiMessageResponse
    {
        public string Message { get; set; } = string.Empty;

        public ApiMessageResponse()
        {
        }

        public ApiMessageResponse(string message)
        {
            Message = message;
        }
    }
}
