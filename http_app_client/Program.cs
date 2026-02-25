using http_app_client;
using System.Text;
using System.Text.Json;

var client = new InteractivePersonClient("https://localhost:5001");
await client.RunAsync();

public class InteractivePersonClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public InteractivePersonClient(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== МЕНЮ УПРАВЛЕНИЯ  ===");
            Console.WriteLine("1. Показать всех людей");
            Console.WriteLine("2. Получить человека по ID");
            Console.WriteLine("3. Создать нового человека");
            Console.WriteLine("4. Обновить человека");
            Console.WriteLine("5. Удалить человека");
            Console.WriteLine("6. Удалить всех людей");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите опцию: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowAllPersons();
                    break;
                case "2":
                    await GetPersonById();
                    break;
                case "3":
                    await CreatePerson();
                    break;
                case "4":
                    await UpdatePerson();
                    break;
                case "5":
                    await DeletePerson();
                    break;
                case "6":
                    await DeleteAllPersons();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }

    private async Task ShowAllPersons()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/person");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var persons = JsonSerializer.Deserialize<List<Person>>(content, _jsonOptions);

            Console.WriteLine("\n=== Список людей ===");
            if (persons != null && persons.Any())
            {
                foreach (var person in persons)
                {
                    Console.WriteLine(person);
                }
            }
            else
            {
                Console.WriteLine("Список пуст");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private async Task GetPersonById()
    {
        Console.Write("Введите ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/person/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var person = JsonSerializer.Deserialize<Person>(content, _jsonOptions);
                    Console.WriteLine($"\nНайден: {person}");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Человек с ID {id} не найден");
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private async Task CreatePerson()
    {
        Console.Write("Введите имя: ");
        var firstName = Console.ReadLine();

        Console.Write("Введите фамилию: ");
        var secondName = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(secondName))
        {
            var person = new Person
            {
                FirstName = firstName,
                SecondName = secondName
            };

            var json = JsonSerializer.Serialize(person);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("/api/person", content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var createdPerson = JsonSerializer.Deserialize<Person>(responseJson, _jsonOptions);
                Console.WriteLine($"\n Создан: {createdPerson}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private async Task UpdatePerson()
    {
        Console.Write("Введите ID для обновления: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Console.Write("Новое имя: ");
            var firstName = Console.ReadLine();

            Console.Write("Новая фамилия: ");
            var secondName = Console.ReadLine();

            var person = new Person
            {
                Id = id,
                FirstName = firstName ?? "",
                SecondName = secondName ?? ""
            };

            var json = JsonSerializer.Serialize(person);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PutAsync("/api/person", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var updatedPerson = JsonSerializer.Deserialize<Person>(responseJson, _jsonOptions);
                    Console.WriteLine($"\n Обновлен: {updatedPerson}");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"\n Человек с ID {id} не найден");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка: {ex.Message}");
            }
        }
    }

    private async Task DeletePerson()
    {
        Console.Write("Введите ID для удаления: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/person/{id}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"\n Человек с ID {id} удален");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"\n Человек с ID {id} не найден");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка: {ex.Message}");
            }
        }
    }

    private async Task DeleteAllPersons()
    {
        Console.Write("Вы уверены? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            try
            {
                var response = await _httpClient.DeleteAsync("/api/person");
                response.EnsureSuccessStatusCode();
                Console.WriteLine("\n Все люди удалены");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка: {ex.Message}");
            }
        }
    }
}