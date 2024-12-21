using System.Windows.Documents;

namespace Tetris
{
    public class Grid
    {
        private readonly int[,] grid;
        public int Rows { get; } // screen height
        public int Columns { get; } // screen width
        public int this[int row, int column]
        {
            get => grid[row, column];
            set => grid[row, column] = value;
        }

        public Grid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            grid = new int[rows, columns];
        }

        public bool IsEmpty(int row, int column)
        {
            if(row < 0 || row >= Rows || column < 0 || column >= Columns)
            {
                return false;
            }
            return grid[row, column] == 0;
        }

        public bool IsRowEmpty(int row)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (!IsEmpty(row, column))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsDanger()
        {
            if(IsRowEmpty(0) && IsRowEmpty(1))
            {
                return false;
            }
            return true;
        }

        public bool IsRowFull(int row)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (grid[row, column] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void ClearRow(int row)
        {
            for (int column = 0; column < Columns; column++)
            {
                grid[row, column] = 0;
            }
        }
        
        public void MoveUpperRowsDown(int row)
        {
            for (int r = row; r >= 0; r--)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (grid[r, column] != 0)
                    {
                        grid[r + 1, column] = grid[r, column];
                        grid[r, column] = 0;
                    }
                }
            }
        }

        public int CheckRowComplete()
        {
            int clearedrows = 0;
            int score = 0;
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (IsRowFull(row))
                {
                    ClearRow(row);
                    MoveUpperRowsDown(row - 1);
                    row += 1;
                    clearedrows++;
                }
            }

            if (clearedrows >= 1)
            {
                score = (int)Math.Pow(2, clearedrows) * 100;
            }
            
            return score;
        }

    }
}
