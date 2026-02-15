namespace Core;

public class Shop
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Address { get; set; } // Если есть в базе
    public int? CountryId { get; set; }  // Если есть связь со страной

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Address: {Address ?? "N/A"}, CountryId: {CountryId}";
    }
}