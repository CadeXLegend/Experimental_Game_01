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
        private GridAssetThemes[] themes;
        public GridAssetThemes[] Themes { get => themes; }
    }
}
