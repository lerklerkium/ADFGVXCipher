namespace ADFGVXCipher
{
    /// <summary>
    /// Генератор матрицы для шифра ADFGVX
    /// </summary>
    public static class MatrixGenerator
    {
        /// <summary>
        /// Набор символов по умолчанию для матрицы 6x6
        /// </summary>
        private const string DefaultCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Генерирует матрицу 6x6 на основе ключевой фразы
        /// </summary>
        /// <param name="keyPhrase">Ключевая фраза для генерации матрицы (может быть пустой)</param>
        /// <returns>Матрица символов 6x6</returns>
        public static char[,] GenerateMatrix(string keyPhrase)
        {
            char[,] matrix = new char[6, 6];
            HashSet<char> usedCharacters = new HashSet<char>();
            List<char> matrixCharacters = new List<char>();

            // Обработка ключевой фразы
            if (!string.IsNullOrEmpty(keyPhrase))
            {
                foreach (char c in keyPhrase.ToUpper())
                {
                    if (IsValidCharacter(c) && !usedCharacters.Contains(c))
                    {
                        matrixCharacters.Add(c);
                        usedCharacters.Add(c);
                    }
                }
            }

            // Добавление оставшихся символов из алфавита
            foreach (char c in DefaultCharacters)
            {
                if (!usedCharacters.Contains(c))
                {
                    matrixCharacters.Add(c);
                }
            }

            // Заполнение матрицы
            int index = 0;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    matrix[i, j] = matrixCharacters[index++];
                }
            }

            return matrix;
        }

        /// <summary>
        /// Проверяет, является ли символ допустимым (буква латинского алфавита или цифра)
        /// </summary>
        /// <param name="c">Проверяемый символ</param>
        /// <returns>True, если символ допустим</returns>
        private static bool IsValidCharacter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9');
        }

        /// <summary>
        /// Возвращает строковое представление матрицы для отображения
        /// </summary>
        /// <param name="matrix">Матрица символов</param>
        /// <returns>Строковое представление матрицы</returns>
        public static string MatrixToString(char[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("     A D F G V X");
            sb.AppendLine("   ┌─────────────┐");

            char[] headers = { 'A', 'D', 'F', 'G', 'V', 'X' };

            for (int i = 0; i < 6; i++)
            {
                sb.Append($" {headers[i]} │ ");
                for (int j = 0; j < 6; j++)
                {
                    sb.Append(matrix[i, j]);
                    if (j < 5) sb.Append(" ");
                }
                sb.AppendLine(" │");
            }

            sb.AppendLine("   └─────────────┘");
            return sb.ToString();
        }
    }
}