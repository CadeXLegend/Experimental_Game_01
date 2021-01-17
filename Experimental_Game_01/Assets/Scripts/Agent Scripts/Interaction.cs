using System;
using UnityEngine;
using UnityEngine.GameFoundation;

/// <summary>
/// A class made to handle and organize interactions from Agents.
/// </summary>
public static class Interaction
{
    public static Action OnGatherCompleted;
    public static Action OnInvestigateCompleted;
    public static Action OnTalkCompleted;
    public static Action OnAttackCompleted;

    /// <summary>
    /// Gather a resource.
    /// </summary>
    /// <param name="resourceType">The type of resource to gather and add.</param>
    /// <param name="gatherAmount">The amount gathered.</param>
    public static void Gather(string resourceType, int gatherAmount)
    {
        Inventory.main.AddItem(resourceType, gatherAmount);
        OnGatherCompleted?.Invoke();
    }

    public static void Investigate()
    {
        OnInvestigateCompleted?.Invoke();
    }

    public static void Talk()
    {
        OnTalkCompleted?.Invoke();
    }

    public static void Attack()
    {
        OnAttackCompleted?.Invoke();
    }
}
