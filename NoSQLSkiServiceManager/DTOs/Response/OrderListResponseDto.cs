using NoSQLSkiServiceManager.Interfaces;

namespace NoSQLSkiServiceManager.DTOs.Response
{
    public class OrderListResponseDto
    {
        public List<OrderResponseDto> Orders { get; set; }
    }
}
