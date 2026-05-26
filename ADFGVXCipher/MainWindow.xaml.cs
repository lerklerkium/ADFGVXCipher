using System.Windows;
using System.Windows.Controls;

namespace ADFGVXCipher
{
    /// <summary>
    /// Главное окно приложения для шифрования/дешифрования ADFGVX
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ToolTipService.SetShowDuration(this, 30000);
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Зашифровать"
        /// </summary>
        private void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEncryptInput.Text))
                {
                    MessageBox.Show("Пожалуйста, введите сообщение для шифрования.",
                        "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEncryptInput.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEncryptTranspositionKey.Text))
                {
                    MessageBox.Show("Пожалуйста, введите ключ перестановки.",
                        "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEncryptTranspositionKey.Focus();
                    return;
                }

                char[,] matrix = MatrixGenerator.GenerateMatrix(txtEncryptMatrixKey.Text ?? "");

                string encrypted = ADFGVXEncryptor.Encrypt(
                    txtEncryptInput.Text,
                    matrix,
                    txtEncryptTranspositionKey.Text
                );

                txtResult.Text = $"Зашифрованное сообщение:\n{FormatOutput(encrypted)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при шифровании: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Дешифровать"
        /// </summary>
        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDecryptInput.Text))
                {
                    MessageBox.Show("Пожалуйста, введите зашифрованное сообщение.",
                        "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDecryptInput.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDecryptTranspositionKey.Text))
                {
                    MessageBox.Show("Пожалуйста, введите ключ перестановки.",
                        "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDecryptTranspositionKey.Focus();
                    return;
                }

                char[,] matrix = MatrixGenerator.GenerateMatrix(txtDecryptMatrixKey.Text ?? "");

                string decrypted = ADFGVXDecryptor.Decrypt(
                    txtDecryptInput.Text,
                    matrix,
                    txtDecryptTranspositionKey.Text
                );

                txtResult.Text = $"Расшифрованное сообщение:\n{decrypted}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при дешифровании: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Показать матрицу"
        /// </summary>
        private void BtnShowMatrix_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                char[,] matrix = MatrixGenerator.GenerateMatrix(txtMatrixPreviewKey.Text ?? "");
                txtMatrixDisplay.Text = MatrixGenerator.MatrixToString(matrix);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации матрицы: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Форматирует выходную строку, добавляя пробелы для читаемости
        /// </summary>
        /// <param name="text">Текст для форматирования</param>
        /// <returns>Отформатированный текст</returns>
        private string FormatOutput(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                sb.Append(text[i]);
                if ((i + 1) % 5 == 0 && i < text.Length - 1)
                {
                    sb.Append(' ');
                }
            }
            return sb.ToString();
        }
    }
}