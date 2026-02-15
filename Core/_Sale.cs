namespace Core;

public class Sale
{
    public int Id { get; set; }
    public required DateTime SaleDate { get; set; }
    public required int BookId { get; set; }
    public required int ShopId { get; set; }
    public required int Quantity { get; set; }
    public decimal? TotalPrice { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, SaleDate: {SaleDate}, BookId: {BookId}, ShopId: {ShopId}, Quantity: {Quantity}, TotalPrice: {TotalPrice}";
    }
}
