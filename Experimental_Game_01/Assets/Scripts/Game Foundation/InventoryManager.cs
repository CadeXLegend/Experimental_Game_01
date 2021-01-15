using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GameFoundation;

public class InventoryManager : MonoBehaviour
{
    private void Awake()
    {
        GameFoundation.Initialize();
    }
}
