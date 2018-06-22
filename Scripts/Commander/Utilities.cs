using UnityEngine;
using System.Collections;

public static class Utilities
{
    //stores white colour texture
    private static Texture2D whiteTexture;
    public static Texture2D WhiteTexture
    {
        get 
        {
            if (whiteTexture == null) //if white texture not created yet, creat it
            {
                whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();
            }

            return whiteTexture;
        }
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
    }

    public static void DrawScreenRectBorder( Rect rect, float thickness, Color color)
    {
        //Top
        Utilities.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);

        //Left
        Utilities.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);

        //Right
        Utilities.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);

        //Bottom
        Utilities.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        //Move origin from bottom left to top right.
        //This is because the screen in unity has the origin in the bottom left, while the Rect struct has the origin at the top left.
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;

        //Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);

        //Create rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    //To determine which objects are within bounds of the selection box, the selectable objects and selection box need to be in the same space
    //this brings the selectable objects into the view space along with the selection box
    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = camera.ScreenToViewportPoint(screenPosition1);
        var v2 = camera.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    //Used to draw pathfinding path
    public static void DrawPath(UnityEngine.AI.NavMeshPath path, LineRenderer line)
    {
        if (path.corners.Length < 2) //if the path has one or no corners, there is no need
            return;

        line.SetVertexCount(path.corners.Length); //set the array of positions to the amount of corners

        for (int i = 0; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]); //go through each corner and set that to the line renderer's position
        }
    }
}
