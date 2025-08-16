using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Utils
{
    public static List<GameObject> GetUIElementsAtPosition(Vector2 position)
    {
        var pointerData = new PointerEventData(EventSystem.current);
        pointerData.pointerId = -1;
        pointerData.position = position;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Select(x => x.gameObject).ToList();
    }

    public static bool ExistUIElementAtPosition(Vector2 position)
    {
        return GetUIElementsAtPosition(position).Count > 0;
    }

    public static bool CanvasGroupVisible(CanvasGroup canvasGroup)
    {
        return canvasGroup.blocksRaycasts;
    }

    public static void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public static void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public static void CopyAll(string sourcePath, string targetPath)
    {
        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);

        string[] files = Directory.GetFiles(sourcePath);
        string[] folders = Directory.GetDirectories(sourcePath);

        foreach (string file in files)
        {
            if (file.Contains(".meta")) continue;

            string name = Path.GetFileName(file);
            string target = Path.Combine(targetPath, name);
            File.Copy(file, target);
        }
        foreach (string folder in folders)
        {
            string name = Path.GetFileName(folder);
            string target = Path.Combine(targetPath, name);
            CopyAll(folder, target);
        }
    }
}
