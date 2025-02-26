namespace Business_Logic_Layer.DTO;

public class CartDetailDTO
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}