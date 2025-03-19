using Business_Logic_Layer.Models.Responses;

namespace Business_Logic_Layer.DTO;

public class CartItemResponse
{
    public Guid Id { get; set; }
    public CartIngredientProductResponse? IngredientProductResponse { get; set; }
    public int Quantity { get; set; }
    public double Price;
    public double TotalPrice;
}