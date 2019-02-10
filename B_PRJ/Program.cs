using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_PRJ
{
    class Cell : IComparable
    {
        public List<int> possibleValues;
        public int value;

        public Cell()
        {
            value = 0;
            possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        public Cell(int Value)
        {
            value = Value;
            if (Value == 0)
            {
                possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            }
            else
            {
                possibleValues = new List<int>() { };
            }

        }

        public int CompareTo(object obj)
        {
            Cell a = (Cell)obj;
            if (this.value >= a.value)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }

    class JigsawCell : Cell
    {
        int set;
        public JigsawCell(int Value, int Set)
        {
            value = Value;
            if (Value == 0)
            {
                possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            }
            else
            {
                possibleValues = new List<int>() { };
            }
            set = Set;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int mode = 0;
            int iteration = 0;
            string sudokuData1 = "000061090, 000907206, 007403180, 904000321, 070000040, " +
                "125000608, 058104900, 403705000, 010680000"; //very easy
            string sudokuData2 = "051007068, 060095013, 200800074, 000100000, 120350046, " +
                "040602090, 500000000, 910004007, 608903052"; //easy
            string sudokuData3 = "100050460, 006073050, 090200000, 000730128, 278000346, " +
                "314082000, 000008010, 080510700, 051040009"; //easy
            string sudokuData4 = "005000100, 061000200, 000380000, 020000004, 000030009, " +
                "013500002, 900002040, 000000070, 400059003"; //very hard
            string jigsawSudokuData1 = "3A0A0A0A0B0B0C0C0C, 0A0A0A9A0B0B0B0C0C, 6A0D0D8D0D0B4B0C3C, " +
                "4D6D0E0D7E8B0B2C0C, 0D0D0E0E5E0E0E0F0F, 0G0G0E6G2E0H0F3F0F, 8G0G1G0G0G6H0H0F0F, " +
                "0G0I0I7I0I2H0H0H0F, 0I0I0I0I0I0H0H0H4F"; //jigsaw sudoku normal
            if (mode == 0)
            {
                Cell[][] matrix = InitializeSudoku(sudokuData1);
                PrintSudoku(matrix);
                Console.WriteLine("-------------------\n");
                int maxPossibleValues;
                int numberOfZerosLastIteration = 0;

                while (!CheckIfCorrect(matrix))
                {
                    CheckCandidates(ref matrix);
                    maxPossibleValues = EnterValues(ref matrix);

                    if (maxPossibleValues >= 4)
                    {
                        FindPreemptiveSets(ref matrix, 4);
                        maxPossibleValues = EnterValues(ref matrix);
                    }

                    if (maxPossibleValues >= 3)
                    {
                        FindPreemptiveSets(ref matrix, 3);
                        maxPossibleValues = EnterValues(ref matrix);
                    }

                    FindPreemptiveSets(ref matrix, 2);
                    EnterValues(ref matrix);

                    if (numberOfZerosLastIteration == CountEmptyCells(matrix))
                    {
                        //uložení pole + náhodné doplnění
                    }
                    else
                    {
                        numberOfZerosLastIteration = CountEmptyCells(matrix);
                    }

                    Console.WriteLine(++iteration + ". iterace");
                    Console.WriteLine("Prázdné buňky: " + numberOfZerosLastIteration);

                    if (SudokuRuleViolated(matrix))
                    {
                        Console.WriteLine("Pravidlo sudoku porušeno!");
                        break;
                    }
                }
                Console.WriteLine("-------------------");

                PrintSudoku(matrix);
                Console.WriteLine("Konec programu, počet iterací: " + iteration);
            }
            else if (mode == 1)
            {
                JigsawCell[][] jigsawMatrix = InitializeJigsawSudoku(jigsawSudokuData1);
            }
            string input = Console.ReadLine();
        }

        static bool SudokuRuleViolated(Cell[][] matrix)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int checkedCellRow = matrix[i][j].value;
                    int checkedCellColumn = matrix[j][i].value;
                    int checkedCellBox = matrix[i % 3 * 3 + j / 3][i / 3 * 3 + j % 3].value;
                    for (int k = 0; k < 9; k++)
                    {
                        //Row check
                        if (checkedCellRow != 0)
                        {
                            if (checkedCellRow == matrix[i][k].value && k != j)
                            {
                                return true;
                            }
                        }
                        //Column check
                        if (checkedCellColumn != 0)
                        {
                            if (checkedCellColumn == matrix[k][i].value && k != j)
                            {
                                return true;
                            }
                        }
                        //Box check
                        if (checkedCellBox != 0)
                        {
                            if (checkedCellBox == matrix[i % 3 * 3 + k / 3][i / 3 * 3 + k % 3].value && k != j)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        static void FindPreemptiveSets(ref Cell[][] matrix, int setSize)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9 - setSize + 1; j++)
                {
                    //checking rows
                    if (matrix[i][j].value == 0 && matrix[i][j].possibleValues.Count <= setSize)
                    {
                        switch (setSize)
                        {
                            case 2:
                                for (int k = j + 1; k < 9; k++)
                                {
                                    if (matrix[i][k].value == 0 && matrix[i][k].possibleValues.Count <= setSize)
                                    {
                                        List<int> unionList = matrix[i][j].possibleValues.Union(matrix[i][k].possibleValues).ToList();
                                        if (unionList.Count <= setSize)
                                        {
                                            for (int l = 0; l < 9; l++)
                                            {
                                                if (matrix[i][l] != matrix[i][j] && matrix[i][l] != matrix[i][k] && matrix[i][l].value == 0)
                                                {
                                                    matrix[i][l].possibleValues = matrix[i][l].possibleValues.Except(unionList).ToList();
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case 3:
                                for (int k = j + 1; k < 9 - 1; k++)
                                {
                                    if (matrix[i][k].value == 0 && matrix[i][k].possibleValues.Count <= setSize)
                                    {
                                        for (int l = k + 1; l < 9; l++)
                                        {
                                            if (matrix[i][l].value == 0 && matrix[i][l].possibleValues.Count <= setSize)
                                            {
                                                List<int> unionList = matrix[i][j].possibleValues.Union(matrix[i][k].possibleValues).Union(matrix[i][l].possibleValues).ToList();
                                                if (unionList.Count <= setSize)
                                                {
                                                    for (int m = 0; m < 9; m++)
                                                    {
                                                        if (matrix[i][m] != matrix[i][j] && matrix[i][m] != matrix[i][k] && matrix[i][m] != matrix[i][l] && matrix[i][m].value == 0)
                                                        {
                                                            matrix[i][m].possibleValues = matrix[i][m].possibleValues.Except(unionList).ToList();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case 4:
                                for (int k = j + 1; k < 9 - 2; k++)
                                {
                                    if (matrix[i][k].value == 0 && matrix[i][k].possibleValues.Count <= setSize)
                                    {
                                        for (int l = k + 1; l < 9 - 1; l++)
                                        {
                                            if (matrix[i][l].value == 0 && matrix[i][l].possibleValues.Count <= setSize)
                                            {
                                                for (int m = l + 1; m < 9; m++)
                                                {
                                                    if (matrix[i][m].value == 0 && matrix[i][m].possibleValues.Count <= setSize)
                                                    {
                                                        List<int> unionList = matrix[i][j].possibleValues.Union(matrix[i][k].possibleValues).Union(matrix[i][l].possibleValues).Union(matrix[i][m].possibleValues).ToList();
                                                        if (unionList.Count <= setSize)
                                                        {
                                                            for (int n = 0; n < 9; n++)
                                                            {
                                                                if (matrix[i][n] != matrix[i][j] && matrix[i][n] != matrix[i][k] && matrix[i][n] != matrix[i][l] && matrix[i][n] != matrix[i][m] && matrix[i][n].value == 0)
                                                                {
                                                                    matrix[i][n].possibleValues = matrix[i][n].possibleValues.Except(unionList).ToList();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    //checking columns
                    if (matrix[j][i].value == 0 && matrix[j][i].possibleValues.Count <= setSize)
                    {
                        switch (setSize)
                        {
                            case 2:
                                for (int k = j + 1; k < 9; k++)
                                {
                                    if (matrix[k][i].value == 0 && matrix[k][i].possibleValues.Count <= setSize)
                                    {
                                        List<int> unionList = matrix[j][i].possibleValues.Union(matrix[k][i].possibleValues).ToList();
                                        if (unionList.Count <= setSize)
                                        {
                                            for (int l = 0; l < 9; l++)
                                            {
                                                if (matrix[l][i] != matrix[j][i] && matrix[l][i] != matrix[k][i] && matrix[l][i].value == 0)
                                                {
                                                    matrix[l][i].possibleValues = matrix[l][i].possibleValues.Except(unionList).ToList();
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case 3:
                                for (int k = j + 1; k < 9 - 1; k++)
                                {
                                    if (matrix[k][i].value == 0 && matrix[k][i].possibleValues.Count <= setSize)
                                    {
                                        for (int l = k + 1; l < 9; l++)
                                        {
                                            if (matrix[l][i].value == 0 && matrix[l][i].possibleValues.Count <= setSize)
                                            {
                                                List<int> unionList = matrix[j][i].possibleValues.Union(matrix[k][i].possibleValues).Union(matrix[l][i].possibleValues).ToList();
                                                if (unionList.Count <= setSize)
                                                {
                                                    for (int m = 0; m < 9; m++)
                                                    {
                                                        if (matrix[m][i] != matrix[j][i] && matrix[m][i] != matrix[k][i] && matrix[m][i] != matrix[l][i] && matrix[m][i].value == 0)
                                                        {
                                                            matrix[m][i].possibleValues = matrix[m][i].possibleValues.Except(unionList).ToList();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case 4:
                                for (int k = j + 1; k < 9 - 2; k++)
                                {
                                    if (matrix[k][i].value == 0 && matrix[k][i].possibleValues.Count <= setSize)
                                    {
                                        for (int l = k + 1; l < 9 - 1; l++)
                                        {
                                            if (matrix[l][i].value == 0 && matrix[l][i].possibleValues.Count <= setSize)
                                            {
                                                for (int m = l + 1; m < 9; m++)
                                                {
                                                    if (matrix[m][i].value == 0 && matrix[m][i].possibleValues.Count <= setSize)
                                                    {
                                                        List<int> unionList = matrix[j][i].possibleValues.Union(matrix[k][i].possibleValues).Union(matrix[l][i].possibleValues).Union(matrix[m][i].possibleValues).ToList();
                                                        if (unionList.Count <= setSize)
                                                        {
                                                            for (int n = 0; n < 9; n++)
                                                            {
                                                                if (matrix[n][i] != matrix[j][i] && matrix[n][i] != matrix[k][i] && matrix[n][i] != matrix[l][i] && matrix[n][i] != matrix[m][i] && matrix[n][i].value == 0)
                                                                {
                                                                    matrix[n][i].possibleValues = matrix[n][i].possibleValues.Except(unionList).ToList();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    //checking boxes
                    if (matrix[i / 3 * 3 + j / 3][j % 3 + i % 3 * 3].value == 0 && matrix[i / 3 * 3 + j / 3][j % 3 + i % 3 * 3].possibleValues.Count <= setSize)
                    {
                        switch (setSize)
                        {
                            case 2:
                                for (int k = j + 1; k < 9; k++)
                                {
                                    if (matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3].value == 0 && matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3].possibleValues.Count <= setSize)
                                    {
                                        List<int> unionList = matrix[i / 3 * 3 + j / 3][j % 3 + i % 3 * 3].possibleValues.Union(matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3].possibleValues).ToList();
                                        if (unionList.Count <= setSize)
                                        {
                                            for (int l = 0; l < 9; l++)
                                            {
                                                if (matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3] != matrix[i / 3 * 3 + j / 3][j % 3 + i % 3 * 3] && matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3] != matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3] && matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3].value == 0)
                                                {
                                                    matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3].possibleValues = matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3].possibleValues.Except(unionList).ToList();
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case 3:
                                for (int k = j + 1; k < 9 - 1; k++)
                                {
                                    if (matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3].value == 0 && matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3].possibleValues.Count <= setSize)
                                    {
                                        for (int l = k + 1; l < 9; l++)
                                        {
                                            if (matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3].value == 0 && matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3].possibleValues.Count <= setSize)
                                            {
                                                List<int> unionList = matrix[i / 3 * 3 + j / 3][j % 3 + i % 3 * 3].possibleValues.Union(matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3].possibleValues).Union(matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3].possibleValues).ToList();
                                                if (unionList.Count <= setSize)
                                                {
                                                    for (int m = 0; m < 9; m++)
                                                    {
                                                        if (matrix[i / 3 * 3 + m / 3][m % 3 + i % 3 * 3] != matrix[i / 3 * 3 + j / 3][j % 3 + i % 3 * 3] && matrix[i / 3 * 3 + m / 3][m % 3 + i % 3 * 3] != matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3] && matrix[i / 3 * 3 + m / 3][m % 3 + i % 3 * 3] != matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3] && matrix[i / 3 * 3 + m / 3][m % 3 + i % 3 * 3].value == 0)
                                                        {
                                                            matrix[i / 3 * 3 + m / 3][m % 3 + i % 3 * 3].possibleValues = matrix[i / 3 * 3 + m / 3][m % 3 + i % 3 * 3].possibleValues.Except(unionList).ToList();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case 4:
                                for (int k = j + 1; k < 9 - 2; k++)
                                {
                                    if (matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3].value == 0 && matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3].possibleValues.Count <= setSize)
                                    {
                                        for (int l = k + 1; l < 9 - 1; l++)
                                        {
                                            if (matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3].value == 0 && matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3].possibleValues.Count <= setSize)
                                            {
                                                for (int m = l + 1; m < 9; m++)
                                                {
                                                    if (matrix[i / 3 * 3 + m / 3][m % 3 + i % 3 * 3].value == 0 && matrix[i / 3 * 3 + m / 3][m % 3 + i % 3 * 3].possibleValues.Count <= setSize)
                                                    {
                                                        List<int> unionList = matrix[i / 3 * 3 + j / 3][j % 3 + i % 3 * 3].possibleValues.Union(matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3].possibleValues).Union(matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3].possibleValues).Union(matrix[i / 3 * 3 + m / 3][m % 3 + i % 3 * 3].possibleValues).ToList();
                                                        if (unionList.Count <= setSize)
                                                        {
                                                            for (int n = 0; n < 9; n++)
                                                            {
                                                                if (matrix[i / 3 * 3 + n / 3][n % 3 + i % 3 * 3] != matrix[i / 3 * 3 + j / 3][j % 3 + i % 3 * 3] && matrix[i / 3 * 3 + n / 3][n % 3 + i % 3 * 3] != matrix[i / 3 * 3 + k / 3][k % 3 + i % 3 * 3] && matrix[i / 3 * 3 + n / 3][n % 3 + i % 3 * 3] != matrix[i / 3 * 3 + l / 3][l % 3 + i % 3 * 3] && matrix[i / 3 * 3 + n / 3][n % 3 + i % 3 * 3] != matrix[i / 3 * 3 + m / 3][m % 3 + i % 3 * 3] && matrix[i / 3 * 3 + n / 3][n % 3 + i % 3 * 3].value == 0)
                                                                {
                                                                    matrix[i / 3 * 3 + n / 3][n % 3 + i % 3 * 3].possibleValues = matrix[i / 3 * 3 + n / 3][n % 3 + i % 3 * 3].possibleValues.Except(unionList).ToList();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        static int EnterValues(ref Cell[][] matrix)
        {
            int maxPossibleValuesCount = 2;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (matrix[i][j].possibleValues.Count == 1)
                    {
                        matrix[i][j].value = matrix[i][j].possibleValues.ElementAt(0);
                        matrix[i][j].possibleValues.Remove(matrix[i][j].value);
                    }
                    if (matrix[i][j].possibleValues.Count > maxPossibleValuesCount)
                    {
                        maxPossibleValuesCount = matrix[i][j].possibleValues.Count;
                    }
                }
            }
            return maxPossibleValuesCount;
        }

        static int CountEmptyCells(Cell[][] matrix)
        {
            int count = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (matrix[i][j].value == 0)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        static void CheckCandidates(ref Cell[][] matrix)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (matrix[i][j].value > 0)
                    {
                        continue;
                    }
                    for (int k = 0; k < 9; k++)
                    {
                        //check for columns in row
                        if (matrix[i][k].value > 0 && matrix[i][j] != matrix[i][k])
                        {
                            matrix[i][j].possibleValues.Remove(matrix[i][k].value);
                        }
                        //check for rows in column
                        if (matrix[k][j].value > 0 && matrix[i][j] != matrix[k][j])
                        {
                            matrix[i][j].possibleValues.Remove(matrix[k][j].value);
                        }
                        //check for boxes
                        if (matrix[i / 3 * 3 + k / 3][j / 3 * 3 + k % 3].value > 0 && matrix[i][j] != matrix[i / 3 * 3 + k / 3][j / 3 * 3 + k % 3])
                        {
                            matrix[i][j].possibleValues.Remove(matrix[i / 3 * 3 + k / 3][j / 3 * 3 + k % 3].value);
                        }
                    }
                }
            }
        }

        static Cell[][] InitializeSudoku(string data)
        {
            string[] rowList = data.Split(',');
            Cell[][] matrix = new Cell[9][];
            for (int i = 0; i < 9; i++)
            {
                matrix[i] = new Cell[9];
                rowList[i] = rowList[i].Trim();
                for (int j = 0; j < 9; j++)
                {
                    matrix[i][j] = new Cell((int)rowList[i][j] - 48);
                }
            }
            return matrix;
        }

        static JigsawCell[][] InitializeJigsawSudoku(string data)
        {
            string[] rowList = data.Split(',');
            JigsawCell[][] matrix = new JigsawCell[9][];
            for (int i = 0; i < 9; i++)
            {
                matrix[i] = new JigsawCell[9];
                rowList[i] = rowList[i].Trim();
                for (int j = 0; j < 9; j++)
                {
                    matrix[i][j] = new JigsawCell((int)rowList[i][j*2] - 48, rowList[i][j*2+1] - 'A');
                }
            }
            return matrix;
        }

        static Cell[][] InitializeSudoku()
        {
            Cell[][] matrix = new Cell[9][];
            for (int i = 0; i < 9; i++)
            {
                matrix[i] = new Cell[9];
                for (int j = 0; j < 9; j++)
                {
                    matrix[i][j] = new Cell();
                }
            }
            return matrix;
        }

        static void AddValue(ref Cell[][] matrix, int row, int column, int value)
        {
            matrix[row][column].value = value;
            matrix[row][column].possibleValues.Clear();
        }

        static void PrintSudoku(Cell[][] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    Console.Write("|" + matrix[i][j].value);
                }
                Console.WriteLine("|");
            }
            Console.WriteLine();
        }

        static bool CheckIfCorrect(Cell[][] matrix)
        {
            Cell[] sortedCopy = new Cell[9];
            // Checking rows
            for (int i = 0; i < 9; i++)
            {
                Array.Copy(matrix[i], sortedCopy, 9);
                Array.Sort(sortedCopy);
                for (int j = 0; j < 9; j++)
                {
                    if (sortedCopy[j].value != j + 1)
                    {
                        return false;
                    }
                }
            }
            //Checking columns
            for (int i = 0; i < 9; i++)
            {
                sortedCopy = new Cell[9];
                for (int j = 0; j < 9; j++)
                {
                    sortedCopy[j] = matrix[j][i];
                }
                Array.Sort(sortedCopy);
                for (int j = 0; j < 9; j++)
                {
                    if (sortedCopy[j].value != j + 1)
                    {
                        return false;
                    }
                }
            }
            //Checking squares
            for (int i = 0; i < 9; i++)
            {
                sortedCopy = new Cell[9];
                for (int j = 0; j < 9; j++)
                {
                    sortedCopy[j] = matrix[i % 3 * 3 + j / 3][i / 3 * 3 + j % 3];
                }
                Array.Sort(sortedCopy);
                for (int j = 0; j < 9; j++)
                {
                    if (sortedCopy[j].value != j + 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
