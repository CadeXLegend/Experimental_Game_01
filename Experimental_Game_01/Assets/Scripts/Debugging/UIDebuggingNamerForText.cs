using Generation;
using TMPro;
using UnityEngine;
public class UIDebuggingNamerForText : MonoBehaviour
{
    [SerializeField]
    private GridContainer container;
    [SerializeField]
    private GenerateMapDebug genMapDebug;
    [SerializeField]
    private TMP_Text mapDebuggingDescription;

    private void Start()
    {
        mapDebuggingDescription.text = $"Generate Map: {genMapDebug.Keycode}\n" +
            $"Theme Change: {container.ThemeCycleKeycode}\n" +
            $"Grid Sectors: {container.GridRegionToggleKeycode}";
    }
}
