using System.Reflection;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ADFGVXCipher.Tests
{
    /// <summary>
    /// Тесты для графического интерфейса с использованием рефлексии
    /// </summary>
    [TestClass]
    public class MainWindowTests
    {
        /// <summary>
        /// Тест наличия всех необходимых элементов управления
        /// </summary>
        [TestMethod]
        public void TestMainWindowHasRequiredControls()
        {
            RunInSTA(() =>
            {
                var window = new MainWindow();

                var txtEncryptInput = GetPrivateField<TextBox>(window, "txtEncryptInput");
                var txtDecryptInput = GetPrivateField<TextBox>(window, "txtDecryptInput");
                var txtEncryptMatrixKey = GetPrivateField<TextBox>(window, "txtEncryptMatrixKey");
                var txtDecryptMatrixKey = GetPrivateField<TextBox>(window, "txtDecryptMatrixKey");
                var txtEncryptTranspositionKey = GetPrivateField<TextBox>(window, "txtEncryptTranspositionKey");
                var txtDecryptTranspositionKey = GetPrivateField<TextBox>(window, "txtDecryptTranspositionKey");
                var txtResult = GetPrivateField<TextBox>(window, "txtResult");
                var btnEncrypt = GetPrivateField<Button>(window, "btnEncrypt");
                var btnDecrypt = GetPrivateField<Button>(window, "btnDecrypt");

                Assert.IsNotNull(txtEncryptInput);
                Assert.IsNotNull(txtDecryptInput);
                Assert.IsNotNull(txtEncryptMatrixKey);
                Assert.IsNotNull(txtDecryptMatrixKey);
                Assert.IsNotNull(txtEncryptTranspositionKey);
                Assert.IsNotNull(txtDecryptTranspositionKey);
                Assert.IsNotNull(txtResult);
                Assert.IsNotNull(btnEncrypt);
                Assert.IsNotNull(btnDecrypt);
            });
        }

        /// <summary>
        /// Тест метода форматирования вывода
        /// </summary>
        [TestMethod]
        public void TestFormatOutputMethod()
        {
            RunInSTA(() =>
            {
                var window = new MainWindow();
                var method = typeof(MainWindow).GetMethod("FormatOutput",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.IsNotNull(method);

                string input = "ABCDEFGHIJ";
                string result = (string)method.Invoke(window, new object[] { input });

                Assert.AreEqual("ABCDE FGHIJ", result);
            });
        }

        /// <summary>
        /// Вспомогательный метод для получения приватного поля через рефлексию
        /// </summary>
        private T GetPrivateField<T>(object obj, string fieldName) where T : class
        {
            var field = obj.GetType().GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            return field?.GetValue(obj) as T;
        }

        /// <summary>
        /// Вспомогательный метод для выполнения кода в STA потоке
        /// </summary>
        private void RunInSTA(Action action)
        {
            var isStaThread = Thread.CurrentThread.GetApartmentState() == ApartmentState.STA;

            if (isStaThread)
            {
                action();
                return;
            }

            var exception = default(Exception);
            var thread = new Thread(() =>
            {
                try
                {
                    var dispatcher = Dispatcher.CurrentDispatcher;

                    action();

                    dispatcher.Invoke(() => { }, DispatcherPriority.Background);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null)
            {
                throw exception;
            }
        }
    }
}