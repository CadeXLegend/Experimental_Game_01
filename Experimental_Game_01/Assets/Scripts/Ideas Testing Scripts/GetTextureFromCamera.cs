using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTextureFromCamera : MonoBehaviour
{
    [SerializeField] private RenderTexture cameraTexture;
    [SerializeField] private Camera targetCamera;
    public Camera TargetCamera { get => targetCamera; }

    public Texture2D GetCameraViewAsTexture2D()
    {
        // get the camera's render texture
        RenderTexture rendText = RenderTexture.active;
        RenderTexture.active = targetCamera.targetTexture;
        // render the texture
        targetCamera.Render();
        // create a new Texture2D with the camera's texture, using its height and width
        Texture2D cameraImage = new Texture2D(targetCamera.targetTexture.width, targetCamera.targetTexture.height, TextureFormat.ARGB32, false);
        cameraImage.ReadPixels(new Rect(0, 0, targetCamera.targetTexture.width, targetCamera.targetTexture.height), 0, 0);
        cameraImage.Apply();
        RenderTexture.active = rendText;
        return cameraImage;
    }
}
