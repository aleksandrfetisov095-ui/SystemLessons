using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EightLesson
{
    public static class EmailValidator
    {
        public static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            email = email.Trim();

            // 1 Ровно один символ @
            int atCount = email.Count(c => c == '@');
            if (atCount != 1)
                return false;

            // 2 Разделение на части
            string[] parts = email.Split('@');
            if (parts.Length != 2)
                return false;

            string localPart = parts[0];
            string domainPart = parts[1];

            // 3 Валидация local-part
            if (!IsValidLocalPart(localPart))
                return false;

            // 4 Валидация domain-part
            if (!IsValidDomainPart(domainPart))
                return false;

            return true;
        }

        private static bool IsValidLocalPart(string localPart)
        {
            // Длина 1-64
            if (localPart.Length < 1 || localPart.Length > 64)
                return false;

            // Допустимые символы
            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!#$%&'*+-/=?^_{|}~";

            foreach (char c in localPart)
            {
                if (!allowedChars.Contains(c) && c != '.') // Точка отдельно
                    return false;
            }

            // Точка не в начале/конце
            if (localPart.StartsWith(".") || localPart.EndsWith("."))
                return false;

            // Не две точки подряд
            if (localPart.Contains(".."))
                return false;

            return true;
        }

        private static bool IsValidDomainPart(string domainPart)
        {
            // Длина 1-255
            if (domainPart.Length < 1 || domainPart.Length > 255)
                return false;

            domainPart = domainPart.ToLower(); // Регистр не важен

            // Должен состоять из меток, разделенных точками
            string[] labels = domainPart.Split('.');
            if (labels.Length < 2) // Хотя бы одна точка
                return false;

            // Проверка каждой метки
            foreach (string label in labels)
            {
                if (!IsValidDomainLabel(label))
                    return false;
            }

            // Последняя метка  минимум 2 символа, только буквы
            string tld = labels.Last();
            if (tld.Length < 2)
                return false;

            if (!tld.All(char.IsLetter))
                return false;

            return true;
        }

        private static bool IsValidDomainLabel(string label)
        {
            // Метка: 1-63 символа
            if (label.Length < 1 || label.Length > 63)
                return false;

            // Начинается и заканчивается буквой или цифрой
            if (!char.IsLetterOrDigit(label.First()) || !char.IsLetterOrDigit(label.Last()))
                return false;

            // Допустимые символы: A-Z, a-z, 0-9, дефис (-)
            foreach (char c in label)
            {
                if (!(char.IsLetterOrDigit(c) || c == '-'))
                    return false;
            }

            // Дефис не может быть в начале или конце
            if (label.StartsWith("-") || label.EndsWith("-"))
                return false;

            return true;
        }

        public static string CheckEmail(string email)
        {
            // дополняем все проверки
            if (string.IsNullOrWhiteSpace(email))
                return " Введите email";

            email = email.Trim();

            // Проверка @
            int atCount = email.Count(c => c == '@');
            if (atCount == 0)
                return " Нет символа @";
            if (atCount > 1)
                return " Слишком много символов @";

            
            return IsValid(email) ? " Email правильный!" : " Email неправильный";
        }


    }
}
