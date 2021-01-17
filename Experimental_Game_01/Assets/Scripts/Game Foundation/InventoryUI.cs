using UnityEngine;
using UnityEngine.GameFoundation;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text lumberText;

    private void Start()
    {
        Interaction.OnGatherCompleted += () => { UpdateUI(); };
    }

    private void UpdateUI()
    {
        lumberText.text = $"{Inventory.main.GetQuantity(inventoryItemDefinitionId: "lumber")}";
    }
}
