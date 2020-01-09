using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FloodFillFromMousePosClick : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteToApplyTextureTo;
    [SerializeField] private GetTextureFromCamera getTexFromCamera;
    [SerializeField] private Color pixelColourToChange;
    [SerializeField] private Color colourToChangeTo;
    [SerializeField] private Color borderColour;
    [SerializeField] private Color32 transparent;
    [SerializeField] private float tolerance = 1f;
    private bool isCompletedWithCurrentFill = true;
    Texture2D tempReadTexture;
    Texture2D tempWriteTexture;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            if(isCompletedWithCurrentFill)
                StartCoroutine(FloodFillAtMousePoint());
    }

    public IEnumerator FloodFillAtMousePoint()
    {
        //destroy the old ones
        if(tempWriteTexture != null)
            Destroy(tempWriteTexture);
        if (tempReadTexture != null)
            Destroy(tempReadTexture);
        if (spriteToApplyTextureTo.sprite != null)
            Destroy(spriteToApplyTextureTo.sprite);

        yield return new WaitForEndOfFrame();

        isCompletedWithCurrentFill = false;
        tempReadTexture = getTexFromCamera.GetCameraViewAsTexture2D();
        tempWriteTexture = getTexFromCamera.GetCameraViewAsTexture2D();

        bool succeeded = FloodFill(
            tempReadTexture,
            tempWriteTexture,
            tolerance,
            (int)Input.mousePosition.x,
            (int)Input.mousePosition.y);

        for (int y = 0; y < tempReadTexture.height; ++y)
        {
            for (int x = 0; x < tempReadTexture.width; ++x)
            {
                if (tempWriteTexture.GetPixel(x, y) != colourToChangeTo)
                    tempWriteTexture.SetPixel(x, y, transparent);
            }
        }

        if (succeeded)
        {
            tempWriteTexture.Apply();
            Sprite renderedOverlay = Sprite.Create(
                tempWriteTexture,
                new Rect(0, 0, tempWriteTexture.width,
                tempWriteTexture.height),
                new Vector2(0.5f, 0.5f));
            spriteToApplyTextureTo.sprite = renderedOverlay;
        }

        isCompletedWithCurrentFill = true;
    }

    public bool FloodFill(Texture2D inputTexture, Texture2D outputTexture, float tolerance, int x, int y)
    {
        Queue<Vector2> pixelCoordinates = new Queue<Vector2>(inputTexture.width * inputTexture.height);
        pixelCoordinates.Enqueue(new Vector2(x, y));
        int iterations = 0;

        int width = inputTexture.width;
        int height = inputTexture.height;
        while (pixelCoordinates.Count > 0)
        {
            if (iterations > 80000)
                return false;

            Vector2 pixelCoordinate = pixelCoordinates.Dequeue();
            int x1 = (int)pixelCoordinate.x;
            int y1 = (int)pixelCoordinate.y;
            if (pixelCoordinates.Count > width * height)
            {
                throw new System.Exception($"<color=red><b>The algorithm is looping</b></color> <b>Queue Size: {pixelCoordinates.Count}</b>");
            }

            if (outputTexture.GetPixel(x1, y1) == colourToChangeTo)
                continue;

            outputTexture.SetPixel(x1, y1, colourToChangeTo);

            //1 -> right
            var newPoint = new Vector2(x1 + 1, y1);
            if (CheckValidity(inputTexture, inputTexture.width, inputTexture.height, newPoint, pixelColourToChange, tolerance))
                pixelCoordinates.Enqueue(newPoint);
            //2 -> left
            newPoint = new Vector2(x1 - 1, y1);
            if (CheckValidity(inputTexture, inputTexture.width, inputTexture.height, newPoint, pixelColourToChange, tolerance))
                pixelCoordinates.Enqueue(newPoint);
            //3 -> up
            newPoint = new Vector2(x1, y1 + 1);
            if (CheckValidity(inputTexture, inputTexture.width, inputTexture.height, newPoint, pixelColourToChange, tolerance))
                pixelCoordinates.Enqueue(newPoint);
            //4 -> down
            newPoint = new Vector2(x1, y1 - 1);
            if (CheckValidity(inputTexture, inputTexture.width, inputTexture.height, newPoint, pixelColourToChange, tolerance))
                pixelCoordinates.Enqueue(newPoint);

            iterations++;
        }
        return true;
    }

    bool CheckValidity(Texture2D texture, int width, int height, Vector2 pixelCoordinate, Color pixelColourToChange, float tolerance)
    {
        if (pixelCoordinate.x < 0 || pixelCoordinate.x >= width)
        {
            return false;
        }
        if (pixelCoordinate.y < 0 || pixelCoordinate.y >= height)
        {
            return false;
        }

        var color = texture.GetPixel((int)pixelCoordinate.x, (int)pixelCoordinate.y);
        
        float distance = Mathf.Abs(color.r - pixelColourToChange.r) + Mathf.Abs(color.g - pixelColourToChange.g) + Mathf.Abs(color.b - pixelColourToChange.b);
        return distance <= tolerance;
    }
}
