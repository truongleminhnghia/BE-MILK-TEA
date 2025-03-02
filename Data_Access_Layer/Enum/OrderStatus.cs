namespace Data_Access_Layer.Enum
{
    public enum OrderStatus
    {
        CONFIRM,
        PENDING_PAYMENT,    // Chờ thanh toán
        DELIVERED,         // Đã giao
        CANCELED,          // Đã hủy
        PENDING_SHIPMENT,  // Chờ giao hàng
        REFUNDED,          // Đã hoàn tiền
        PENDING_CONFIRMATION, // Chờ xác nhận
        FAILED,            // Không thành công
        PICKUP_AT_STORE    // Đến cửa hàng lấy
    }
}