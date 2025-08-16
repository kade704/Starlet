using UnityEngine;

[CreateAssetMenu(fileName = "Controller Asset", menuName = "Custom Asset/Controller Asset")]
public class ControllerAsset : ScriptableObject
{
    public string title;
    public Controller prefab;
    public Sprite icon;
}
