using System.Text;

namespace ADFGVXCipher
{
    /// <summary>
    /// Класс для шифрования сообщений шифром ADFGVX
    /// </summary>
    public static class ADFGVXEncryptor
    {
        /// <summary>
        /// Символы, используемые для кодирования позиций в матрице
        /// </summary>
        private static readonly char[] Coordinates = { 'A', 'D', 'F', 'G', 'V', 'X' };

        /// <summary>
        /// Шифрует сообщение с использованием шифра ADFGVX
        /// </summary>
        /// <param name="message">Исходное сообщение</param>
        /// <param name="matrix">Матрица подстановки</param>
        /// <param name="transpositionKey">Ключ перестановки</param>
        /// <returns>Зашифрованное сообщение</returns>
        /// <exception cref="ArgumentNullException">Если параметры null</exception>
        /// <exception cref="ArgumentException">Если параметры пустые</exception>
        public static string Encrypt(string message, char[,] matrix, string transpositionKey)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (transpositionKey == null)
                throw new ArgumentNullException(nameof(transpositionKey));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Сообщение не может быть пустым", nameof(message));
            if (string.IsNullOrWhiteSpace(transpositionKey))
                throw new ArgumentException("Ключ перестановки не может быть пустым", nameof(transpositionKey));

            message = NormalizeInput(message);
            transpositionKey = NormalizeInput(transpositionKey);

            string firstStage = FirstStageEncrypt(message, matrix);

            string secondStage = SecondStageEncrypt(firstStage, transpositionKey);

            return secondStage;
        }

        /// <summary>
        /// Первый этап шифрования: замена символов на координаты в матрице
        /// </summary>
        /// <param name="message">Сообщение для шифрования</param>
        /// <param name="matrix">Матрица подстановки</param>
        /// <returns>Строка координат</returns>
        public static string FirstStageEncrypt(string message, char[,] matrix)
        {
            StringBuilder result = new StringBuilder();

            foreach (char c in message)
            {
                bool found = false;
                for (int i = 0; i < 6 && !found; i++)
                {
                    for (int j = 0; j < 6 && !found; j++)
                    {
                        if (matrix[i, j] == c)
                        {
                            result.Append(Coordinates[i]);
                            result.Append(Coordinates[j]);
                            found = true;
                        }
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Второй этап шифрования: перестановка по ключу
        /// </summary>
        /// <param name="text">Текст после первого этапа</param>
        /// <param name="key">Ключ перестановки</param>
        /// <returns>Зашифрованный текст</returns>
        public static string SecondStageEncrypt(string text, string key)
        {
            var keyOrder = GetKeyOrder(key);

            int rows = (int)Math.Ceiling((double)text.Length / key.Length);
            char[,] table = new char[rows, key.Length];

            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    if (index < text.Length)
                    {
                        table[i, j] = text[index++];
                    }
                    else
                    {
                        table[i, j] = '`'; // заполнитель
                    }
                }
            }

            // чтение по столбцам в порядке ключа
            StringBuilder result = new StringBuilder();
            foreach (var order in keyOrder.OrderBy(x => x.Value))
            {
                int col = order.Key;
                for (int row = 0; row < rows; row++)
                {
                    result.Append(table[row, col]);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Получает порядок символов ключа для перестановки
        /// </summary>
        /// <param name="key">Ключ перестановки</param>
        /// <returns>Словарь с порядком позиций</returns>
        private static Dictionary<int, int> GetKeyOrder(string key)
        {
            var sorted = key.Select((c, i) => new { Char = c, Index = i })
                            .OrderBy(x => x.Char)
                            .ToList();

            Dictionary<int, int> order = new Dictionary<int, int>();
            for (int i = 0; i < sorted.Count; i++)
            {
                order[sorted[i].Index] = i;
            }

            return order;
        }

        /// <summary>
        /// Нормализует входную строку (удаляет пробелы, приводит к верхнему регистру, оставляет только допустимые символы)
        /// </summary>
        /// <param name="input">Входная строка</param>
        /// <returns>Нормализованная строка</returns>
        private static string NormalizeInput(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input.ToUpper())
            {
                if ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }
    }
}