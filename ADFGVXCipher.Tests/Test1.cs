namespace ADFGVXCipher.Tests
{
    /// <summary>
    /// Набор автоматизированных тестов для шифра ADFGVX
    /// </summary>
    [TestClass]
    public class ADFGVXCipherTests
    {
        /// <summary>
        /// Тест генерации матрицы по умолчанию
        /// </summary>
        [TestMethod]
        public void TestDefaultMatrixGeneration()
        {
            var matrix = MatrixGenerator.GenerateMatrix("");
            Assert.AreEqual(6, matrix.GetLength(0));
            Assert.AreEqual(6, matrix.GetLength(1));
            Assert.AreEqual('A', matrix[0, 0]);
            Assert.AreEqual('9', matrix[5, 5]);
        }

        /// <summary>
        /// Тест генерации матрицы с ключевой фразой
        /// </summary>
        [TestMethod]
        public void TestMatrixGenerationWithKeyPhrase()
        {
            var matrix = MatrixGenerator.GenerateMatrix("7YUT");
            Assert.AreEqual('7', matrix[0, 0]);
            Assert.AreEqual('Y', matrix[0, 1]);
            Assert.AreEqual('U', matrix[0, 2]);
            Assert.AreEqual('T', matrix[0, 3]);
            Assert.AreEqual('A', matrix[0, 4]);
            Assert.AreEqual('B', matrix[0, 5]);
        }

        /// <summary>
        /// Тест игнорирования дубликатов в ключевой фразе
        /// </summary>
        [TestMethod]
        public void TestMatrixGenerationIgnoresDuplicates()
        {
            var matrix = MatrixGenerator.GenerateMatrix("AABBCC");
            Assert.AreEqual('A', matrix[0, 0]);
            Assert.AreEqual('B', matrix[0, 1]);
            Assert.AreEqual('C', matrix[0, 2]);
            Assert.AreEqual('D', matrix[0, 3]);
        }

        /// <summary>
        /// Тест игнорирования недопустимых символов
        /// </summary>
        [TestMethod]
        public void TestMatrixGenerationIgnoresInvalidCharacters()
        {
            var matrix = MatrixGenerator.GenerateMatrix("AB CD!@#");
            Assert.AreEqual('A', matrix[0, 0]);
            Assert.AreEqual('B', matrix[0, 1]);
            Assert.AreEqual('C', matrix[0, 2]);
            Assert.AreEqual('D', matrix[0, 3]);
            Assert.AreEqual('E', matrix[0, 4]);
        }

        /// <summary>
        /// Тест исторической матрицы
        /// </summary>
        [TestMethod]
        public void TestHistoricalMatrixGeneration()
        {
            var matrix = MatrixGenerator.GenerateMatrix("1JR4HDE2AV9M8PINKZBYUF6T5GXS3OWLQ7C0");
            Assert.AreEqual('1', matrix[0, 0]);
            Assert.AreEqual('J', matrix[0, 1]);
            Assert.AreEqual('R', matrix[0, 2]);
            Assert.AreEqual('0', matrix[5, 5]);
        }

        /// <summary>
        /// Тест первого этапа шифрования
        /// </summary>
        [TestMethod]
        public void TestFirstStageEncryption()
        {
            var matrix = MatrixGenerator.GenerateMatrix("");
            var result = ADFGVXEncryptor.FirstStageEncrypt("A", matrix);
            Assert.AreEqual("AA", result);
        }

        /// <summary>
        /// Тест второго этапа шифрования
        /// </summary>
        [TestMethod]
        public void TestSecondStageEncryption()
        {
            string firstStage = "ADFGVX";
            string key = "ABC";
            var result = ADFGVXEncryptor.SecondStageEncrypt(firstStage, key);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        /// <summary>
        /// Тест полного цикла шифрования и дешифрования
        /// </summary>
        [TestMethod]
        public void TestEncryptDecryptCycle()
        {
            string original = "HELLO";
            string matrixKey = "";
            string transpositionKey = "SECRET";

            var matrix = MatrixGenerator.GenerateMatrix(matrixKey);
            string encrypted = ADFGVXEncryptor.Encrypt(original, matrix, transpositionKey);
            string decrypted = ADFGVXDecryptor.Decrypt(encrypted, matrix, transpositionKey);

            Assert.AreEqual(original, decrypted);
        }

        /// <summary>
        /// Тест исторического примера - шифрование
        /// </summary>
        [TestMethod]
        public void TestHistoricalExampleEncryption()
        {
            string message = "ATTACKWILLBEGININ11AM";
            string matrixKey = "1JR4HDE2AV9M8PINKZBYUF6T5GXS3OWLQ7C0";
            string transpositionKey = "SECRET";

            var matrix = MatrixGenerator.GenerateMatrix(matrixKey);
            string encrypted = ADFGVXEncryptor.Encrypt(message, matrix, transpositionKey);

            string expected = "GXFGFFDFFADDFAGFXDFADXVFAFGFDDXXVFAXVDAGAX";

            Assert.AreEqual(expected, encrypted.Replace(" ", ""));
        }

        /// <summary>
        /// Тест исторического примера - дешифрование
        /// </summary>
        [TestMethod]
        public void TestHistoricalExampleDecryption()
        {
            string encrypted = "GXFGFFDFFADDFAGFXDFADXVFAFGFDDXXVFAXVDAGAX";
            string matrixKey = "1JR4HDE2AV9M8PINKZBYUF6T5GXS3OWLQ7C0";
            string transpositionKey = "SECRET";

            var matrix = MatrixGenerator.GenerateMatrix(matrixKey);
            string decrypted = ADFGVXDecryptor.Decrypt(encrypted, matrix, transpositionKey);

            Assert.AreEqual("ATTACKWILLBEGININ11AM", decrypted);
        }

        /// <summary>
        /// Тест обработки пустого сообщения
        /// </summary>
        [TestMethod]
        public void TestEmptyMessageThrowsException()
        {
            var matrix = MatrixGenerator.GenerateMatrix("");
            try
            {
                ADFGVXEncryptor.Encrypt("", matrix, "KEY");
                Assert.Fail("Ожидалось исключение ArgumentException, но оно не было выброшено");
            }
            catch (ArgumentException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ожидалось ArgumentException, но было выброшено {ex.GetType().Name}");
            }
        }

        /// <summary>
        /// Тест обработки null сообщения
        /// </summary>
        [TestMethod]
        public void TestNullMessageThrowsException()
        {
            var matrix = MatrixGenerator.GenerateMatrix("");
            try
            {
                ADFGVXEncryptor.Encrypt(null, matrix, "KEY");
                Assert.Fail("Ожидалось исключение ArgumentNullException, но оно не было выброшено");
            }
            catch (ArgumentNullException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ожидалось ArgumentNullException, но было выброшено {ex.GetType().Name}");
            }
        }

        /// <summary>
        /// Тест обработки пустого ключа перестановки
        /// </summary>
        [TestMethod]
        public void TestEmptyTranspositionKeyThrowsException()
        {
            var matrix = MatrixGenerator.GenerateMatrix("");
            try
            {
                ADFGVXEncryptor.Encrypt("MESSAGE", matrix, "");
                Assert.Fail("Ожидалось исключение ArgumentException, но оно не было выброшено");
            }
            catch (ArgumentException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ожидалось ArgumentException, но было выброшено {ex.GetType().Name}");
            }
        }

        /// <summary>
        /// Тест обработки null ключа перестановки
        /// </summary>
        [TestMethod]
        public void TestNullTranspositionKeyThrowsException()
        {
            var matrix = MatrixGenerator.GenerateMatrix("");
            try
            {
                ADFGVXEncryptor.Encrypt("MESSAGE", matrix, null);
                Assert.Fail("Ожидалось исключение ArgumentNullException, но оно не было выброшено");
            }
            catch (ArgumentNullException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ожидалось ArgumentNullException, но было выброшено {ex.GetType().Name}");
            }
        }

        /// <summary>
        /// Тест обработки некорректного зашифрованного текста (нечётная длина)
        /// </summary>
        [TestMethod]
        public void TestInvalidEncryptedTextOddLength()
        {
            var matrix = MatrixGenerator.GenerateMatrix("");
            try
            {
                ADFGVXDecryptor.Decrypt("ABC", matrix, "KEY");
                Assert.Fail("Ожидалось исключение ArgumentException, но оно не было выброшено");
            }
            catch (ArgumentException)
            {
                // Ожидаемое исключение - тест пройден
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ожидалось ArgumentException, но было выброшено {ex.GetType().Name}");
            }
        }

        /// <summary>
        /// Тест обработки некорректных символов в зашифрованном тексте
        /// </summary>
        [TestMethod]
        public void TestInvalidCharactersInEncryptedText()
        {
            var matrix = MatrixGenerator.GenerateMatrix("");
            try
            {
                ADFGVXDecryptor.Decrypt("ABCD", matrix, "KEY");
                Assert.Fail("Ожидалось исключение ArgumentException, но оно не было выброшено");
            }
            catch (ArgumentException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail($"Ожидалось ArgumentException, но было выброшено {ex.GetType().Name}");
            }
        }

        /// <summary>
        /// Тест шифрования с цифрами
        /// </summary>
        [TestMethod]
        public void TestEncryptionWithNumbers()
        {
            string message = "HELLO123";
            var matrix = MatrixGenerator.GenerateMatrix("");
            string encrypted = ADFGVXEncryptor.Encrypt(message, matrix, "KEY");
            string decrypted = ADFGVXDecryptor.Decrypt(encrypted, matrix, "KEY");

            Assert.AreEqual(message, decrypted);
        }

        /// <summary>
        /// Тест нормализации входного текста (удаление пробелов и приведение к верхнему регистру)
        /// </summary>
        [TestMethod]
        public void TestInputNormalization()
        {
            string message = "hello world";
            var matrix = MatrixGenerator.GenerateMatrix("");
            string encrypted = ADFGVXEncryptor.Encrypt(message, matrix, "KEY");
            string decrypted = ADFGVXDecryptor.Decrypt(encrypted, matrix, "KEY");

            Assert.AreEqual("HELLOWORLD", decrypted);
        }
    }
}