namespace Core;

public class Author
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}";
    }
}