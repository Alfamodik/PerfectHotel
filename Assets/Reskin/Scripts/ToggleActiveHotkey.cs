using UnityEditor;
using UnityEngine;

public static class ToggleActiveHotkey
{
    [MenuItem("Tools/Toggle Active %e")] // Ctrl + Shift + E
    public static void ToggleActive()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            bool isActive = selectedObject.activeSelf;
            selectedObject.SetActive(!isActive);
        }
    }
}