using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agents
{
    /// <summary>
    /// ScriptableObject containing Entities.
    /// </summary>
    [CreateAssetMenu(fileName = "New Entity Collection", menuName = "Entities/New Entity Collection", order = 0)]
    public class EntityCollection : ScriptableObject
    {
        [Header("Put your Entities here.")]
        public GameObject[] Entities;
    }
}
