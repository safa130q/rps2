// MaxSubarrayFinder.cs
// Практическая работа №2. Процесс проектирования.
// Вариант 6: Нахождение подпоследовательности массива с максимальной суммой.
// Алгоритм: Кадане (Kadane's algorithm), O(n).
// Автор: Студент группы 445, Сафонова Елена Андреевна. 2026 год.

using System;

namespace rps2.core
{
    /// <summary>
    /// Результат поиска подпоследовательности с максимальной суммой.
    /// </summary>
    public class MaxSubarrayResult
    {
        /// <summary>Максимальная сумма подпоследовательности.</summary>
        public double MaxSum { get; }

        /// <summary>Индекс начального элемента (включительно).</summary>
        public int StartIndex { get; }

        /// <summary>Индекс конечного элемента (включительно).</summary>
        public int EndIndex { get; }

        /// <summary>Значение начального элемента подпоследовательности.</summary>
        public double StartValue { get; }

        /// <summary>Значение конечного элемента подпоследовательности.</summary>
        public double EndValue { get; }

        /// <summary>
        /// Создаёт объект результата.
        /// </summary>
        public MaxSubarrayResult(double maxSum, int startIndex, int endIndex,
                                  double startValue, double endValue)
        {
            MaxSum = maxSum;
            StartIndex = startIndex;
            EndIndex = endIndex;
            StartValue = startValue;
            EndValue = endValue;
        }
    }

    /// <summary>
    /// Содержит метод поиска подпоследовательности массива с максимальной суммой
    /// (задача Максимального подмассива).
    /// </summary>
    public static class MaxSubarrayFinder
    {
        /// <summary>
        /// Находит непрерывную подпоследовательность массива с максимальной суммой.
        ///
        /// Алгоритм Кадане:
        ///   Проходим массив слева направо, накапливая текущую сумму.
        ///   Если текущая сумма становится меньше текущего элемента,
        ///   начинаем новую подпоследовательность с текущего элемента.
        ///   Фиксируем максимум на каждом шаге.
        ///
        ///   Корректно работает с массивами, содержащими все отрицательные числа:
        ///   в этом случае возвращает максимальный (наименее отрицательный) элемент.
        ///
        /// Сложность: O(n) по времени, O(1) по дополнительной памяти.
        ///
        /// Предусловие: массив не null и содержит хотя бы один элемент.
        /// Постусловие: возвращает корректный MaxSubarrayResult с индексами
        ///              startIndex &lt;= endIndex внутри границ массива.
        /// </summary>
        /// <param name="array">Массив вещественных чисел.</param>
        /// <returns>Результат с суммой, начальным и конечным элементами.</returns>
        /// <exception cref="ArgumentNullException">Если массив равен null.</exception>
        /// <exception cref="ArgumentException">Если массив пуст.</exception>
        public static MaxSubarrayResult Find(double[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Массив не может быть null.");
            if (array.Length == 0)
                throw new ArgumentException("Массив не может быть пустым.", nameof(array));

            double maxSum = array[0];
            double currentSum = array[0];
            int bestStart = 0;
            int bestEnd = 0;
            int tempStart = 0;  // кандидат на начало текущей подпоследовательности

            for (int i = 1; i < array.Length; i++)
            {
                // Если продолжать прежнюю подпоследовательность невыгодно — начать новую
                if (currentSum + array[i] < array[i])
                {
                    currentSum = array[i];
                    tempStart = i;
                }
                else
                {
                    currentSum += array[i];
                }

                // Обновить лучший результат
                if (currentSum > maxSum)
                {
                    maxSum = currentSum;
                    bestStart = tempStart;
                    bestEnd = i;
                }
            }

            return new MaxSubarrayResult(
                maxSum,
                bestStart,
                bestEnd,
                array[bestStart],
                array[bestEnd]);
        }
    }
}
