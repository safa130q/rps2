// MaxSubarrayTests.cs
// Модульные тесты для алгоритма MaxSubarrayFinder.
// Покрывают: обычный случай, все отрицательные, один элемент,
//            вся последовательность, граничные значения, null/пустой массив.

using rps2.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace rps2.tests
{
    [TestClass]
    public class MaxSubarrayTests
    {
        // ───── Обычные случаи ─────

        [TestMethod]
        public void Find_ClassicExample_ReturnsCorrectSubarray()
        {
            // Классический пример: максимальный подмассив [4, -1, 2, 1] с суммой 6
            double[] array = { -2, 1, -3, 4, -1, 2, 1, -5, 4 };
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            Assert.AreEqual(6.0, result.MaxSum, 1e-9);
            Assert.AreEqual(3, result.StartIndex);
            Assert.AreEqual(6, result.EndIndex);
        }

        [TestMethod]
        public void Find_AllPositive_ReturnsWholeArray()
        {
            double[] array = { 1, 2, 3, 4, 5 };
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            Assert.AreEqual(15.0, result.MaxSum, 1e-9);
            Assert.AreEqual(0, result.StartIndex);
            Assert.AreEqual(4, result.EndIndex);
        }

        [TestMethod]
        public void Find_MaxAtEnd_CorrectIndices()
        {
            double[] array = { -5, -3, 1, 2, 10 };
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            Assert.AreEqual(13.0, result.MaxSum, 1e-9);
            Assert.AreEqual(2, result.StartIndex);
            Assert.AreEqual(4, result.EndIndex);
        }

        [TestMethod]
        public void Find_MaxAtBeginning_CorrectIndices()
        {
            double[] array = { 10, 2, -1, -5, -3 };
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            Assert.AreEqual(11.0, result.MaxSum, 1e-9);
            Assert.AreEqual(0, result.StartIndex);
            Assert.AreEqual(1, result.EndIndex);
        }

        // ───── Все отрицательные ─────

        [TestMethod]
        public void Find_AllNegative_ReturnsMaxElement()
        {
            // Максимальная подпоследовательность — один наименее отрицательный элемент
            double[] array = { -5, -1, -3, -2 };
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            Assert.AreEqual(-1.0, result.MaxSum, 1e-9);
            Assert.AreEqual(1, result.StartIndex);
            Assert.AreEqual(1, result.EndIndex);
        }

        [TestMethod]
        public void Find_SingleNegative_ReturnsThatElement()
        {
            double[] array = { -42 };
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            Assert.AreEqual(-42.0, result.MaxSum, 1e-9);
            Assert.AreEqual(0, result.StartIndex);
            Assert.AreEqual(0, result.EndIndex);
        }

        // ───── Один элемент ─────

        [TestMethod]
        public void Find_SinglePositive_ReturnsThatElement()
        {
            double[] array = { 7.5 };
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            Assert.AreEqual(7.5, result.MaxSum, 1e-9);
            Assert.AreEqual(0, result.StartIndex);
            Assert.AreEqual(0, result.EndIndex);
        }

        // ───── Вещественные числа ─────

        [TestMethod]
        public void Find_FloatingPointValues_CorrectResult()
        {
            double[] array = { -0.5, 1.2, 3.4, -0.1, 2.0, -10.0, 0.3 };
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            // Максимальный подмассив: [1.2, 3.4, -0.1, 2.0] = 6.5
            Assert.AreEqual(6.5, result.MaxSum, 1e-9);
            Assert.AreEqual(1, result.StartIndex);
            Assert.AreEqual(4, result.EndIndex);
        }

        // ───── Значения начального и конечного элементов ─────

        [TestMethod]
        public void Find_StartAndEndValues_AreCorrect()
        {
            double[] array = { -2, 3, 5, -1, 2 };
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            Assert.AreEqual(array[result.StartIndex], result.StartValue, 1e-9);
            Assert.AreEqual(array[result.EndIndex], result.EndValue, 1e-9);
        }

        // ───── Особые ситуации ─────

        [TestMethod]
        public void Find_NullArray_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                MaxSubarrayFinder.Find(null!)
            );
        }

        [TestMethod]
        public void Find_EmptyArray_ThrowsArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                MaxSubarrayFinder.Find(new double[0])
            );
        }

        // ───── Нули в массиве ─────

        [TestMethod]
        public void Find_ArrayWithZeros_HandledCorrectly()
        {
            double[] array = { 0, 0, 0, 1, 0, 0 };
            MaxSubarrayResult result = MaxSubarrayFinder.Find(array);
            Assert.AreEqual(1.0, result.MaxSum, 1e-9);
        }
    }
}