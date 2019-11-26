using UnityEngine;

/// <summary>
/// ScriptableObject containing Sprites to be applied to a Grid.
/// </summary>
[CreateAssetMenu(fileName = "New Grid Asset Theme", menuName = "Grid/Theme", order = 0)]
public class GridAssetTheme : ScriptableObject
{
    public enum Theme
    {
        Forest = 0,
        Desert = 1,
        Eldritch = 2,
    }

    [Header("Theme Information")]
    public Theme SelectedTheme;

    [Header("Theme Data")]
    public Sprite[] Sprites = new Sprite[9];
    public TileDecoration[] TileDecorations;
}
