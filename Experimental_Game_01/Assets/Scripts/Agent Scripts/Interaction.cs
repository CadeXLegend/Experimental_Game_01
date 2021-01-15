using System;
using UnityEngine;
using UnityEngine.GameFoundation;

public static class Interaction
{
    public static Action OnHarvestCompleted;

    public static void Harvest(int harvestAmount)
    {
        Inventory.main.AddItem("lumber", harvestAmount);
        OnHarvestCompleted?.Invoke();
    }
}
