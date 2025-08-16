using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Module))]
public class ModuleEditor : Editor
{
    private Module module;

    private void OnEnable()
    {
        module = (Module)target;
    }

    private void OnSceneGUI()
    {
        var x = (Vector2)module.transform.right;
        var y = (Vector2)module.transform.up;

        var centre = (Vector2)module.transform.position;
        centre += x * module.offset.x + y * module.offset.y;

        var halfSizeX = module.size.x / 2;
        var halfSizeY = module.size.y / 2;

        var linePoints = new Vector3[4];
        linePoints[0] = centre + x * halfSizeX + y * halfSizeY;
        linePoints[1] = centre - x * halfSizeX + y * halfSizeY;
        linePoints[2] = centre - x * halfSizeX - y * halfSizeY;
        linePoints[3] = centre + x * halfSizeX - y * halfSizeY;

        var lineIndices = new int[8] { 0, 1, 1, 2, 2, 3, 3, 0 };
        Handles.DrawLines(linePoints, lineIndices);
    }
}
