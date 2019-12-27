using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTicker : MonoBehaviour
{
    private static int ticks;
    public static int Ticks { get => ticks; }

    public delegate void TurnTickEvents();
    public static event TurnTickEvents OnTick;

    public static void Tick()
    {
        ticks++;
        OnTick?.Invoke();
    }
}
