using HttpServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Только регистрация для Product
builder.Services.AddSingleton<List<Product>>();
builder.Services.AddSingleton<ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Только Product endpoints
app.MapGet("/products", (ProductService productService) =>
{
    var products = productService.GetAll();
    return Results.Ok(products);
});

app.MapGet("/products/{id}", (int id, ProductService productService) =>
{
    var product = productService.GetById(id);
    return product != null ? Results.Ok(product) : Results.NotFound();
});

app.MapPost("/products/", (Product product, ProductService productService) =>
{
    productService.Create(product);
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id}", (int id, Product product, ProductService productService) =>
{
    var existingProduct = productService.GetById(id);
    if (existingProduct == null)
    {
        return Results.NotFound();
    }

    productService.Update(id, product);
    return Results.Ok(product);
});

app.MapDelete("/products/{id}", (int id, ProductService productService) =>
{
    var existingProduct = productService.GetById(id);
    if (existingProduct == null)
    {
        return Results.NotFound();
    }

    productService.Delete(id);
    return Results.Ok();
});

app.Run();