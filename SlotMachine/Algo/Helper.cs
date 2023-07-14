namespace SlotMachine.Algo
{
    public static class Helper
    {
        public static int[] GetRow(this int[,] matrix, int rowIndex)
        {
            int rowLength = matrix.GetLength(1);
            int[] row = new int[rowLength];

            for (int i = 0; i < rowLength; i++)
            {
                row[i] = matrix[rowIndex, i];
            }

            return row;
        }

    }
}
