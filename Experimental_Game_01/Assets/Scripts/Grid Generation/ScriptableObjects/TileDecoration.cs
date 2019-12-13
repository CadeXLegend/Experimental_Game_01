using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject containing Sprites to be applied to a Grid.
/// </summary>
[CreateAssetMenu(fileName = "New Tile Decoration", menuName ="Grid/Tile Decoration", order = 0)]
public class TileDecoration : ScriptableObject
{
    public Sprite Sprite;

    [Range(0.1f, 3f)]
    public float SpriteScale = 1f;

    public Vector2 ColliderSize = new Vector2(1f, 1f);
}
