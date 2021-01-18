using UnityEngine;

public class TileSettings : MonoBehaviour
{
    [SerializeField]
    private Color32 defaultTileColour;
    public Color32 DefaultTileColour { get => defaultTileColour; }
    [SerializeField]
    private Color32 walkableTileColour;
    public Color32 WalkableTileColour { get => walkableTileColour; }
    [SerializeField]
    private Color32 enemyTileColour;
    public Color32 EnemyTileColour { get => enemyTileColour; }
    [SerializeField]
    private Color32 resourceTileColour;
    public Color32 ResourceTileColour { get => resourceTileColour; }
    [SerializeField]
    private Color32 playerTileColour;
    public Color32 PlayerTileColour { get => playerTileColour; }

    #region Singleton
    public static TileSettings instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log($"More than one {instance} instance found!");
            Destroy(this);
            return;
        }
        instance = this;
    }
    #endregion
}
