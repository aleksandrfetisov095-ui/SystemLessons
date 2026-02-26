using HttpServer;
using System.Xml.Linq;

public class ProductService
{
    private int maxId = 0;
    private readonly List<Product> _products;

    public ProductService(List<Product> products)
    {
        _products = products;

        // несколько тестовых продуктов
        _products.Add(new Product
        {
            Id = ++maxId,
            Name = "Ноутбук",
            Description = "Мощный игровой ноутбук",
            Price = 75000.00m
        });

        _products.Add(new Product
        {
            Id = ++maxId,
            Name = "Смартфон",
            Description = "Современный смартфон с отличной камерой",
            Price = 45000.00m
        });

        _products.Add(new Product
        {
            Id = ++maxId,
            Name = "Наушники",
            Description = "Беспроводные наушники с шумоподавлением",
            Price = 8990.00m
        });
    }

    public List<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(int id)
    {
        return _products.FirstOrDefault(x => x.Id == id);
    }

    public void Create(Product product)
    {
        product.Id = ++maxId;
        _products.Add(product);
    }

    public void Update(int id, Product product)
    {
        var updatedProduct = GetById(id);
        if (updatedProduct != null)
        {
            updatedProduct.Name = product.Name;
            updatedProduct.Description = product.Description;
            updatedProduct.Price = product.Price;
        }
    }

    public void Delete(int id)
    {
        var deletedProduct = GetById(id);
        if (deletedProduct != null)
        {
            _products.Remove(deletedProduct);
        }
    }
}