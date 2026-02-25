using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace http_app_client
{
    public class Person
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string SecondName { get; set; }

        public override string ToString()
        {
            return $"ID: {Id}, Имя: {FirstName}, Фамилия: {SecondName}";
        }
    }
}
