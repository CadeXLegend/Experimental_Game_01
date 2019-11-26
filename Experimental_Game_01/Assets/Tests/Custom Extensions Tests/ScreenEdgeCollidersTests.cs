using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using CustomExtensions.Bounds;

namespace Tests
{
    public class ScreenEdgeCollidersTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void generate_screen_edge_colliders_from_camera_and_screen_size()
        {
            ScreenEdgeColliders collider = new GameObject().AddComponent<ScreenEdgeColliders>();
            EdgeCollider2D generatedCollider = collider.AddCollider();

            Assert.That(generatedCollider.edgeCount == 4);
        }
    }
}
