namespace Core;

public class Book
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int Pages { get; set; }
    public required decimal Price { get; set; }
    public required DateTime PublishDate { get; set; }
    public required int AuthorId { get; set; }
    public required int Themeld { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Pages: {Pages}, Price: {Price}, PublishDate: {PublishDate}, AuthorId: {AuthorId}, ThemeId: {Themeld}";
    }
}