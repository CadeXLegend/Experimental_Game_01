//Thanks so much to: https://github.com/UnityCommunity/UnityLibrary/blob/master/Assets/Scripts/2D/Colliders/ScreenEdgeColliders.cs
//for the awesome source code!
using UnityEngine;

namespace CustomExtensions.Bounds
{
    public class ScreenEdgeColliders : MonoBehaviour
    {
        private void Awake()
        {
            AddCollider();
        }

        public virtual EdgeCollider2D AddCollider()
        {
            if (Camera.main == null) { Debug.LogError("Camera.main not found, failed to create edge colliders"); return null; }

            Camera cam = Camera.main;
            if (!cam.orthographic) { Debug.LogError("Camera.main is not Orthographic, failed to create edge colliders"); return null; }

            transform.position = Camera.main.transform.position;
            Vector2 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            Vector2 topLeft = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane));
            Vector2 topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
            Vector2 bottomRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, cam.nearClipPlane));

            // add or use existing EdgeCollider2D
            EdgeCollider2D edge = GetComponent<EdgeCollider2D>() == null ? gameObject.AddComponent<EdgeCollider2D>() : GetComponent<EdgeCollider2D>();

            Vector2[] edgePoints = new[] { bottomLeft, topLeft, topRight, bottomRight, bottomLeft };
            edge.points = edgePoints;
            edge.edgeRadius = 0.5f;
            return edge;
        }
    }
}
