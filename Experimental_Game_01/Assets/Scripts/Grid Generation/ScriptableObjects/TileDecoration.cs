using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject containing Sprites to be applied to a Grid.
/// </summary>
[CreateAssetMenu(fileName = "New Tile Decoration", menuName ="Grid/Tile Decoration", order = 0)]
public class TileDecoration : ScriptableObject
{
    public Sprite Decoration;
    private float Height;
}
