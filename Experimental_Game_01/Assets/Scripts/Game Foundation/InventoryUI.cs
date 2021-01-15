using UnityEngine;
using UnityEngine.GameFoundation;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text lumberText;

    private void Start()
    {
        Interaction.OnHarvestCompleted += () => { UpdateUI(); };
    }

    private void UpdateUI()
    {
        lumberText.text = $"{Inventory.main.GetItem("lumber").quantity}";
    }
}
