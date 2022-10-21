using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethods
{
    public static Vector2 GetPositionWithinScreen(this Transform transform,CanvasScaler canvasScaler)
    {
        float canvasXResolution = canvasScaler.referenceResolution.x;
        float canvasYResolution = canvasScaler.referenceResolution.y;
        float randomXPosition = Random.Range(-canvasXResolution / 2, canvasXResolution / 2);
        float randomYPosition = Random.Range(-canvasYResolution / 2, canvasYResolution / 2);
        return new Vector2(randomXPosition, randomYPosition);
    }
}
