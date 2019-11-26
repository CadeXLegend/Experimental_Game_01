using UnityEngine;

namespace Generation
{
    /// <summary>
    /// Data Object containing Assets for a Grid.
    /// </summary>
    public class GridAssets : MonoBehaviour
    {
        [SerializeField]
        private GameObject assetPrefab;
        public GameObject AssetPrefab { get => assetPrefab; }
        [SerializeField]
        private GridAssetTheme[] themes;
        public GridAssetTheme[] Themes { get => themes; }

        public void Init(GameObject _assetPrefab, GridAssetTheme[] _themes)
        {
            assetPrefab = _assetPrefab;
            themes = _themes;
        }
    }
}
