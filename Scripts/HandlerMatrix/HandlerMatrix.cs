using System.Collections.Generic;
using System.Linq;


public static class HandlerMatrix<T>
{
    public static T[] GetColumn(T[,] matrix, int numberColumn)
    {
        return Enumerable.Range(0, matrix.GetLength(0)).Select(x => matrix[x, numberColumn]).ToArray();
    }

    public static T[] GetRow(T[,] matrix, int numberRow)
    {
        return Enumerable.Range(0, matrix.GetLength(1)).Select(y => matrix[numberRow, y]).ToArray();
    }

    public static List<T> GetListColumn(T[,] matrix, int numberColumn)
    {
        return Enumerable.Range(0, matrix.GetLength(0)).Select(x => matrix[x, numberColumn]).ToList();
    }

    public static List<T> GetListRow(T[,] matrix, int numberRow)
    {
        return Enumerable.Range(0, matrix.GetLength(1)).Select(y => matrix[numberRow, y]).ToList();
    }

    public static List<T> GetElementMatrix3X3(T[,] matrix, int numberRow, int numberColumn)
    {
        List<T> matrix3x3 = new List<T>();

        for (int x = numberColumn; x < numberColumn + 3; x++)
        {
            for (int y = numberRow; y < numberRow + 3; y++)
            {
                if ((y >= 0 && y < matrix.GetLength(1)) && (x >= 0 && x < matrix.GetLength(0)))
                    matrix3x3.Add(matrix[y, x]);
            }
        }

        return matrix3x3;
    }
}

