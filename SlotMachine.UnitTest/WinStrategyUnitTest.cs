using NUnit.Framework;
using SlotMachine.Algo;
using System.Collections.Generic;

namespace SlotMachine.UnitTests
{
    [TestFixture]
    internal class WinStrategyUnitTest
    {
        private IWinStrategy winStrategy;
        [SetUp]
        public void SetUp()
        {
            winStrategy = new WinStrategy();
        }

        public static object[] SuccessfullTestCases =
        {
             new TestCaseData(new int[,] { { 3, 3, 3, 4, 5 }, { 2, 3, 2, 3, 3 }, { 1, 2, 3, 3, 3 } }, 27),
             new TestCaseData(new int[,] { { 5, 2, 6, 4, 6 },{ 7, 5, 4, 4, 1 },{ 1, 5, 5, 5, 0 } }, 15),
             new TestCaseData(new int[,] { { 5, 0, 5, 5, 2 },{ 5, 8, 8, 1, 4 },{ 7, 2, 6, 3, 2 } }, 0),
             new TestCaseData(new int[,] { { 9, 9, 9, 4, 2 },{ 5, 1, 4, 2, 7 },{ 3, 9, 6, 8, 7 } } , 27),
             new TestCaseData(new int[,] { { 9, 2, 9, 8, 1, 2, 5, 8 },{ 8, 9, 8, 6, 1, 1, 0, 1 },{ 0, 2, 9, 6, 4, 7, 0, 9 },{ 9, 2, 8, 9, 0, 5, 8, 3 },{ 6, 1, 5, 0, 1, 5, 5, 3 } } , 36)
        };

        public static object[] TestCasesSeriesLengthLowerthanTwo =
        {
             new TestCaseData(new int[,] { { 5, 0, 5, 5, 2 },{ 5, 8, 8, 1, 4 },{ 7, 2, 6, 3, 2 } })
        };

        [TestCaseSource(nameof(SuccessfullTestCases))]
        [Test]
        public void CalculateWin_WhenCall_CalculateWinSuccessfully(int[,] matrix, int expectedValue)
        {
            var result = winStrategy.CalculateWin(matrix);

            Assert.AreEqual(result, expectedValue);
        }

        [TestCaseSource(nameof(TestCasesSeriesLengthLowerthanTwo))]
        [Test]
        public void CalculateWin_SeriesLengthLowerThan3_MustZeroWin(int[,] matrix)
        {
            var result = winStrategy.CalculateWin(matrix);

            Assert.AreEqual(result, 0);
        }


    }
}
