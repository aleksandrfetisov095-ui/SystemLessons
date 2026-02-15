namespace Core;

public class Country
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}";
    }
}
