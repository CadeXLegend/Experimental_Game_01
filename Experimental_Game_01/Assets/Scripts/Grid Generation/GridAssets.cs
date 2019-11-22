using UnityEngine;

/// <summary>
/// The container for Grid Assets to Spawn.
/// </summary>
public class GridAssets : MonoBehaviour
{
    [SerializeField]
    private GameObject assetPrefab;
    public GameObject AssetPrefab { get => assetPrefab; }
    [SerializeField]
    private Sprite[] sprites;
    public Sprite[] Sprites { get => sprites; }
}
