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
        private Sprite[] sprites;
        public Sprite[] Sprites { get => sprites; }
    }
}
