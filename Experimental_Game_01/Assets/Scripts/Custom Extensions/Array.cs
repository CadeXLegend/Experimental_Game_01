using System;
using System.Collections;

/// <summary>
/// Custom class for Multidimensional Array Traversals.
/// </summary>
public static class Array
{
    /// <summary>
    /// Traverse a Multidimensional Array and run code inside giving acccess to Column & Row definition.
    /// </summary>
    /// <typeparam name="T">The Type of Array.</typeparam>
    /// <param name="MultidimensionalArray">The Array itself.</param>
    /// <param name="action">The action to run inside of the Traversal.</param>
    public static void ForEach<T>(T[,] MultidimensionalArray, Action<int, int> action)
    {
        int columns = MultidimensionalArray.GetLength(0);
        int rows = MultidimensionalArray.GetLength(1);
        for (int column = 0; column < columns; ++column)
        {
            for (int row = 0; row < rows; ++row)
            {
                action(column, row);
            }
        }
    }
}
