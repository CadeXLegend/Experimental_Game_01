using System;
using static Generation.Tile;

public static class PositionOnGridExtension
{
    public static PositionOnGrid Previous(this PositionOnGrid position)
    {
        PositionOnGrid[] posArray = (PositionOnGrid[])Enum.GetValues(typeof(PositionOnGrid));
        int previousIndex = System.Array.IndexOf(posArray, position) - 1;
        return (previousIndex <= 0) ? posArray[0] : posArray[previousIndex];
    }

    public static PositionOnGrid Next(this PositionOnGrid position)
    {
        PositionOnGrid[] posArray = (PositionOnGrid[])Enum.GetValues(typeof(PositionOnGrid));
        int nextIndex = System.Array.IndexOf(posArray, position) + 1;
        return (nextIndex >= posArray.Length) ? posArray[0] : posArray[nextIndex];
    }

    public static int GetUnshiftedNumber(this PositionOnGrid position)
    {
        PositionOnGrid[] posArray = (PositionOnGrid[])Enum.GetValues(typeof(PositionOnGrid));
        int posInIndex = System.Array.IndexOf(posArray, position);
        return posInIndex;
    }
}
