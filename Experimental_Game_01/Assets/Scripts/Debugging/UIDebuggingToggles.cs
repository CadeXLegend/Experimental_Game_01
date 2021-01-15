using UnityEngine;

public class UIDebuggingToggles : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject MapDebuggingPanel, DruidAbilityDebuggingPanel;
    [SerializeField] private string mapDebugKeycode, druidAbilityKeycode;
    private KeyCode mapdebkc, druidabilkc;
    private void Awake()
    {
        parent.SetActive(false);
        MapDebuggingPanel.SetActive(false);
        DruidAbilityDebuggingPanel.SetActive(false);
        mapdebkc = (KeyCode)System.Enum.Parse(typeof(KeyCode), mapDebugKeycode);
        druidabilkc = (KeyCode)System.Enum.Parse(typeof(KeyCode), druidAbilityKeycode);
    }

    private void Update()
    {
        if (!parent) return;

        if (Input.GetKeyDown(mapdebkc)) 
            MapDebuggingPanel.SetActive(!MapDebuggingPanel.activeSelf);
        if (Input.GetKeyDown(druidabilkc))
            DruidAbilityDebuggingPanel.SetActive(!DruidAbilityDebuggingPanel.activeSelf);
    }
}
