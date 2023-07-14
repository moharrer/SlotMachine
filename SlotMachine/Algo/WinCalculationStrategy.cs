namespace SlotMachine.Algo
{
    public interface IWinStrategy
    {
        int CalculateWin(int[,] matrix);
    }

    public class WinStrategy : IWinStrategy
    {

        public int CalculateWin(int[,] matrix)
        {
            int win = 0;

            var rowsCount = matrix.GetLength(0);
            for (int i = 0; i < rowsCount; i++)
            {
                win += StraightTraverse(matrix, i);
                win += DiagonallyTraverse(matrix, i);
            }

            return win;
        }

        #region Utillity
        private int StraightTraverse(int[,] matrix, int rowIndex)
        {
            var row = matrix.GetRow(rowIndex);
            var (number, length) = GetMaxConsecutive(row);

            return number * length;
        }

        private int DiagonallyTraverse(int[,] matrix, int row)
        {
            var result = new List<int>();

            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);

            if (row >= rowLength - 1)
                return 0;

            int i = row;
            int j = 0;
            bool traverseDown = true;

            while (i >= 0 && i < rowLength && j < colLength)
            {
                result.Add(matrix[i, j]);
                if (traverseDown)
                {
                    if (i + 1 < rowLength)
                    {
                        i++;
                        j++;
                    }
                    else
                    {
                        traverseDown = false;
                        i--;
                        j++;
                    }
                }
                else
                {
                    if (i - 1 >= 0)
                    {
                        i--;
                        j++;
                    }
                    else
                    {
                        traverseDown = true;
                        i++;
                        j++;
                    }
                }
            }

            var (number, length) = GetMaxConsecutive(result.ToArray());

            return number * length;
        }

        private (int, int) GetMaxConsecutive(int[] arr)
        {
            int number = 0;
            int maxCount = 0;
            int currentCount = 1;

            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] == arr[i - 1])
                {
                    currentCount++;
                    number = arr[i];
                }
                else
                {
                    break;
                    //maxCount = Math.Max(maxCount, currentCount);
                    //currentCount = 1;
                }
            }

            var max = Math.Max(maxCount, currentCount);

            if (max <= 2)
                max = 0;

            return (number, max);

        }

        #endregion
    }
}
