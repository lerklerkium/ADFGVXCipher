using System.Text;

namespace ADFGVXCipher
{
    /// <summary>
    /// Класс для дешифрования сообщений шифра ADFGVX
    /// </summary>
    public static class ADFGVXDecryptor
    {
        /// <summary>
        /// Символы, используемые для кодирования позиций в матрице
        /// </summary>
        private static readonly char[] Coordinates = { 'A', 'D', 'F', 'G', 'V', 'X' };

        /// <summary>
        /// Дешифрует сообщение, зашифрованное шифром ADFGVX
        /// </summary>
        /// <param name="encryptedMessage">Зашифрованное сообщение</param>
        /// <param name="matrix">Матрица подстановки</param>
        /// <param name="transpositionKey">Ключ перестановки</param>
        /// <returns>Расшифрованное сообщение</returns>
        /// <exception cref="ArgumentNullException">Если параметры null</exception>
        /// <exception cref="ArgumentException">Если параметры некорректны</exception>
        public static string Decrypt(string encryptedMessage, char[,] matrix, string transpositionKey)
        {
            if (encryptedMessage == null)
                throw new ArgumentNullException(nameof(encryptedMessage));
            if (transpositionKey == null)
                throw new ArgumentNullException(nameof(transpositionKey));
            if (string.IsNullOrWhiteSpace(encryptedMessage))
                throw new ArgumentException("Зашифрованное сообщение не может быть пустым", nameof(encryptedMessage));
            if (string.IsNullOrWhiteSpace(transpositionKey))
                throw new ArgumentException("Ключ перестановки не может быть пустым", nameof(transpositionKey));

            // Нормализация входных данных
            encryptedMessage = NormalizeEncryptedInput(encryptedMessage);
            transpositionKey = NormalizeInput(transpositionKey);

            // Первый этап: обратная перестановка
            string firstStageDecrypted = SecondStageDecrypt(encryptedMessage, transpositionKey);

            // Второй этап: обратная замена координат на символы
            string decrypted = FirstStageDecrypt(firstStageDecrypted, matrix);

            return decrypted;
        }

        /// <summary>
        /// Обратное преобразование первого этапа: из координат в символы
        /// </summary>
        /// <param name="coordinates">Строка координат</param>
        /// <param name="matrix">Матрица подстановки</param>
        /// <returns>Расшифрованное сообщение</returns>
        public static string FirstStageDecrypt(string coordinates, char[,] matrix)
        {
            if (coordinates.Length % 2 != 0)
                throw new ArgumentException("Длина зашифрованного текста должна быть чётной", nameof(coordinates));

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < coordinates.Length; i += 2)
            {
                int row = Array.IndexOf(Coordinates, coordinates[i]);
                int col = Array.IndexOf(Coordinates, coordinates[i + 1]);

                if (row == -1 || col == -1)
                    throw new ArgumentException($"Недопустимый символ в зашифрованном тексте: {coordinates[i]} или {coordinates[i + 1]}");

                result.Append(matrix[row, col]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Обратное преобразование второго этапа: обратная перестановка
        /// </summary>
        /// <param name="text">Текст для дешифрования</param>
        /// <param name="key">Ключ перестановки</param>
        /// <returns>Текст после обратной перестановки</returns>
        public static string SecondStageDecrypt(string text, string key)
        {
            // Создание упорядоченного ключа
            var keyOrder = GetKeyOrder(key);

            // Вычисление размеров таблицы
            int cols = key.Length;
            int rows = (int)Math.Ceiling((double)text.Length / cols);
            int totalCells = rows * cols;
            int extraCells = totalCells - text.Length;

            // Определение длины каждого столбца
            int[] columnLengths = new int[cols];
            for (int i = 0; i < cols; i++)
            {
                columnLengths[i] = rows;
            }

            // Уменьшение длины последних столбцов, если есть лишние ячейки
            if (extraCells > 0)
            {
                var sortedColumns = keyOrder.OrderByDescending(x => x.Value).Take(extraCells);
                foreach (var col in sortedColumns)
                {
                    columnLengths[col.Key]--;
                }
            }

            // Создание таблицы
            char[,] table = new char[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    table[i, j] = '\0';
                }
            }

            // Заполнение таблицы по столбцам в порядке ключа
            int index = 0;
            foreach (var order in keyOrder.OrderBy(x => x.Value))
            {
                int col = order.Key;
                for (int row = 0; row < columnLengths[col]; row++)
                {
                    table[row, col] = text[index++];
                }
            }

            // Чтение таблицы по строкам
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (table[i, j] != '`')
                    {
                        result.Append(table[i, j]);
                    }
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
        /// Нормализует входную строку (удаляет пробелы, приводит к верхнему регистру)
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

        /// <summary>
        /// Нормализует зашифрованный текст (удаляет пробелы, приводит к верхнему регистру)
        /// </summary>
        /// <param name="input">Зашифрованный текст</param>
        /// <returns>Нормализованная строка</returns>
        private static string NormalizeEncryptedInput(string input)
        {
            return input.Replace(" ", "").ToUpper();
        }
    }
}