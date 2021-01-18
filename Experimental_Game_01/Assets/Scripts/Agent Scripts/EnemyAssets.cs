using Agents;
using Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Let's pretend this class doesn't exist until I solidify my approach to doing this stuff.
/// I am writing this while being very tired...
/// </summary>
public class EnemyAssets : MonoBehaviour
{
    [SerializeField]
    private GameObject enemySpawner;
    public GameObject EnemySpawner { get => enemySpawner; }
    [SerializeField]
    private EntityCollection enemies;
    public EntityCollection Enemies { get => enemies; }
}
