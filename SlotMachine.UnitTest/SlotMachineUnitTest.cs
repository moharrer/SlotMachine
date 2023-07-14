using NUnit.Framework;
using System;

namespace SlotMachine.UnitTests
{
    [TestFixture]
    public class SlotMachineUnitTest
    {
        [SetUp]
        public void SetUp()
        {

        }

        [TestCase(5, 3)]
        [TestCase(8, 10)]
        [TestCase(1, 2)]
        [Test]
        public void Spin_WhenCall_SuccessfullyGenerateMatrix(int heigth, int width)
        {
            var slotMachine = new Algo.SlotMachine();

            var matrix = slotMachine.Spin(heigth, width);

            Assert.That(matrix.GetLength(0), Is.EqualTo(heigth));
            Assert.That(matrix.GetLength(1), Is.EqualTo(width));

            // verify generated digits are between 0 and 9
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Assert.IsTrue(matrix[i, j] >= 0 && matrix[i, j] <= 9, "digit is not within the specified range.");
                }
            }

        }

        [TestCase(-1, -1)]
        [TestCase(-54, 0)]
        [TestCase(0, -10)]
        [Test]
        public void Spin_NegativeDimention_ShouldThrowException(int heigth, int width)
        {
            var slotMachine = new Algo.SlotMachine();

            Assert.Throws<Exception>(()=> slotMachine.Spin(heigth, width));
        }

    }
}
