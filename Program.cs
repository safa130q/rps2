// Program.cs
// Практическая работа №2. Процесс проектирования.
// Вариант 6: Найти в массиве вещественных чисел непрерывную подпоследовательность
//            с максимальной суммой. Вывести начальный и конечный элемент.
// Язык: C#
// Автор: Студент группы 445, Сафонова Елена Андреевна. 2026 год.

using System;
using System.Globalization;
using System.IO;
using System.Text;
using rps2.core;

namespace rps2
{
    /// <summary>
    /// Точка входа в программу. Содержит главный цикл, меню и ввод/вывод.
    /// </summary>
    class Program
    {
        // Минимально допустимая длина массива
        private const int MinArrayLength = 1;

        // Максимально допустимая длина массива (защита от бесконечно долгого ввода)
        private const int MaxArrayLength = 100_000;

        // Максимальное количество элементов подпоследовательности для вывода на экран
        private const int MaxSubarrayDisplayLength = 20;

        // Максимальное количество элементов массива для вывода на экран
        private const int MaxArrayDisplayLength = 30;

        static void Main(string[] args)
        {
            PrintWelcome();

            bool running = true;
            while (running)
            {
                PrintMenu();
                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        RunConsoleInput();
                        break;
                    case "2":
                        RunFileInput();
                        break;
                    case "3":
                        PrintWelcome();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Программа завершена.");
                        break;
                    default:
                        Console.WriteLine("Ошибка: неверный пункт меню. Введите 0, 1, 2 или 3.");
                        break;
                }
            }
        }

        // ─────────────────────────── UI ───────────────────────────

        /// <summary>
        /// Выводит приветственное сообщение с описанием программы.
        /// </summary>
        static void PrintWelcome()
        {
            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     Практическая работа №2. Процесс проектирования.      ║");
            Console.WriteLine("║  Вариант 6: Максимальная подпоследовательность массива.  ║");
            Console.WriteLine("║  Автор: Студент группы 445, Сафонова Елена Андреевна.    ║");
            Console.WriteLine("║  Задача: найти непрерывную подпоследовательность         ║");
            Console.WriteLine("║  вещественных чисел с максимальной суммой,               ║");
            Console.WriteLine("║  вывести начальный и конечный элементы.                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        }

        /// <summary>
        /// Выводит главное меню.
        /// </summary>
        static void PrintMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Меню:");
            Console.WriteLine("  1 - Ввод массива с клавиатуры");
            Console.WriteLine("  2 - Ввод массива из файла");
            Console.WriteLine("  3 - О программе");
            Console.WriteLine("  0 - Выход");
            Console.Write("Ваш выбор: ");
        }

        // ─────────────────────────── Режимы работы ───────────────────────────

        /// <summary>
        /// Режим ввода данных с клавиатуры.
        /// </summary>
        static void RunConsoleInput()
        {
            Console.WriteLine();
            Console.WriteLine("=== Ввод с клавиатуры ===");

            int length = InputArrayLength();
            double[] array = new double[length];

            Console.WriteLine($"Введите {length} вещественных чисел (по одному на строку или через пробел):");
            int filled = 0;
            while (filled < length)
            {
                Console.Write($"  [{filled + 1}/{length}]: ");
                string raw = Console.ReadLine()?.Trim() ?? string.Empty;

                // Поддержка ввода нескольких чисел через пробел в одной строке
                string[] tokens = raw.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string token in tokens)
                {
                    if (filled >= length) break;
                    if (!double.TryParse(token,
                        NumberStyles.Float | NumberStyles.AllowLeadingSign,
                        CultureInfo.InvariantCulture, out double val))
                    {
                        Console.WriteLine($"  Ошибка: '{token}' не является числом. Пропущено.");
                        continue;
                    }
                    if (double.IsInfinity(val) || double.IsNaN(val))
                    {
                        Console.WriteLine($"  Ошибка: '{token}' выходит за пределы типа double. Пропущено.");
                        continue;
                    }
                    array[filled++] = val;
                }
            }

            ProcessAndPrint(array, outputPath: null);
        }

        /// <summary>
        /// Режим ввода данных из файла.
        /// Формат файла: числа через пробел или перенос строки.
        /// </summary>
        static void RunFileInput()
        {
            Console.WriteLine();
            Console.WriteLine("=== Ввод из файла ===");

            string inputPath = InputFilePath("Путь к входному файлу: ", forWriting: false);

            try
            {
                string content = File.ReadAllText(inputPath);
                double[] array = ParseArrayFromText(content);

                if (array.Length == 0)
                    throw new FormatException("Файл не содержит ни одного числа.");

                if (array.Length > MaxArrayLength)
                    throw new FormatException(
                        $"Массив слишком большой ({array.Length} элементов). " +
                        $"Максимум: {MaxArrayLength}.");

                Console.WriteLine($"Считано {array.Length} элементов.");
                PrintArray(array);

                string outputPath = InputFilePath("Путь к файлу для сохранения результата: ", forWriting: true);

                ProcessAndPrint(array, outputPath);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Ошибка: файл '{inputPath}' не найден.");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Ошибка формата данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        // ─────────────────────────── Обработка ───────────────────────────

        /// <summary>
        /// Выполняет поиск и вывод результата на экран и опционально в файл.
        /// </summary>
        /// <param name="array">Исходный массив.</param>
        /// <param name="outputPath">Путь к файлу вывода или null.</param>
        static void ProcessAndPrint(double[] array, string outputPath)
        {
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            string resultText = FormatResult(array, result);

            Console.WriteLine();
            Console.WriteLine(resultText);

            if (!string.IsNullOrEmpty(outputPath))
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Исходный массив:");
                    sb.AppendLine(ArrayToString(array));
                    sb.AppendLine();
                    sb.AppendLine(resultText);

                    File.WriteAllText(outputPath, sb.ToString());
                    Console.WriteLine($"Результат сохранён в файл: {outputPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Форматирует результат поиска в читаемую строку.
        /// </summary>
        static string FormatResult(double[] array, MaxSubarrayResult result)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Максимальная сумма подпоследовательности: {result.MaxSum:F4}");
            sb.AppendLine($"Индекс начального элемента : {result.StartIndex} " +
                          $"(значение: {result.StartValue:F4})");
            sb.AppendLine($"Индекс конечного элемента  : {result.EndIndex} " +
                          $"(значение: {result.EndValue:F4})");
            sb.AppendLine($"Длина подпоследовательности: {result.EndIndex - result.StartIndex + 1}");

            // Вывод самой подпоследовательности
            int subLen = result.EndIndex - result.StartIndex + 1;
            if (subLen <= MaxSubarrayDisplayLength)
            {
                sb.Append("Подпоследовательность      : [");
                for (int i = result.StartIndex; i <= result.EndIndex; i++)
                {
                    sb.Append(array[i].ToString("F4", CultureInfo.InvariantCulture));
                    if (i < result.EndIndex) sb.Append(", ");
                }
                sb.AppendLine("]");
            }
            else
            {
                sb.AppendLine($"(подпоследовательность содержит {subLen} элементов — слишком много для вывода)");
            }

            return sb.ToString().TrimEnd();
        }

        // ─────────────────────────── Ввод с клавиатуры ───────────────────────────

        /// <summary>
        /// Запрашивает у пользователя длину массива с проверками допустимого диапазона.
        /// </summary>
        static int InputArrayLength()
        {
            while (true)
            {
                Console.Write($"Введите количество элементов массива ({MinArrayLength}–{MaxArrayLength}): ");
                string raw = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!int.TryParse(raw, out int length))
                {
                    Console.WriteLine("  Ошибка: введите целое число.");
                    continue;
                }

                if (length < MinArrayLength)
                {
                    Console.WriteLine($"  Ошибка: массив должен содержать не менее {MinArrayLength} элемента.");
                    continue;
                }

                if (length > MaxArrayLength)
                {
                    Console.WriteLine($"  Ошибка: слишком большой массив. Максимум: {MaxArrayLength}.");
                    continue;
                }

                return length;
            }
        }

        // ─────────────────────────── Вспомогательные методы ───────────────────────────

        /// <summary>
        /// Читает непустую строку пути к файлу с консоли и проверяет её корректность.
        /// Повторяет запрос при пустом вводе, запрещённых символах или если путь — директория.
        /// </summary>
        /// <param name="prompt">Строка-подсказка.</param>
        /// <param name="forWriting">Если true — также проверяет, что файл не read-only.</param>
        static string InputFilePath(string prompt, bool forWriting = false)
        {
            while (true)
            {
                Console.Write(prompt);
                string path = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("  Ошибка: путь не может быть пустым.");
                    continue;
                }

                // Проверка на запрещённые символы — GetFullPath бросает исключение
                try
                {
                    path = Path.GetFullPath(path);
                }
                catch
                {
                    Console.WriteLine("  Ошибка: путь содержит недопустимые символы.");
                    continue;
                }

                // Проверка: не является ли путь директорией
                if (Directory.Exists(path))
                {
                    Console.WriteLine("  Ошибка: указан путь к папке, а не к файлу.");
                    continue;
                }

                // Для записи — проверить read-only (если файл уже существует)
                if (forWriting && File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    if (fi.IsReadOnly)
                    {
                        Console.WriteLine("  Ошибка: файл доступен только для чтения.");
                        continue;
                    }
                }

                return path;
            }
        }

        /// <summary>
        /// Разбирает текст (числа через пробел/перенос строки) в массив double.
        /// Токены с переполнением или NaN пропускаются с предупреждением.
        /// </summary>
        static double[] ParseArrayFromText(string text)
        {
            string[] tokens = text.Split(
                new[] { ' ', '\t', '\r', '\n', ',' },
                StringSplitOptions.RemoveEmptyEntries);

            var values = new System.Collections.Generic.List<double>();
            foreach (string token in tokens)
            {
                if (!double.TryParse(token,
                    NumberStyles.Float | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture, out double val))
                {
                    Console.WriteLine($"  Предупреждение: '{token}' не является числом, пропущено.");
                    continue;
                }

                if (double.IsInfinity(val) || double.IsNaN(val))
                {
                    Console.WriteLine($"  Предупреждение: '{token}' выходит за пределы типа double, пропущено.");
                    continue;
                }

                values.Add(val);
            }

            return values.ToArray();
        }

        /// <summary>
        /// Выводит массив на экран (до 30 элементов).
        /// </summary>
        static void PrintArray(double[] array)
        {
            Console.Write("Массив: [");
            int count = Math.Min(array.Length, MaxArrayDisplayLength);
            for (int i = 0; i < count; i++)
            {
                Console.Write(array[i].ToString("F4", CultureInfo.InvariantCulture));
                if (i < count - 1) Console.Write(", ");
            }
            if (array.Length > MaxArrayDisplayLength)
                Console.Write($", ... (+{array.Length - MaxArrayDisplayLength} ещё)");
            Console.WriteLine("]");
        }

        /// <summary>
        /// Преобразует массив в строку для записи в файл.
        /// </summary>
        static string ArrayToString(double[] array)
        {
            return string.Join(" ", Array.ConvertAll(
                array, x => x.ToString("F4", CultureInfo.InvariantCulture)));
        }
    }
}