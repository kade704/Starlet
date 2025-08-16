using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour
{
    [HideInInspector] public ControllerAsset asset;
    [HideInInspector] public List<Property> properties = new List<Property>();
    [HideInInspector] public RectTransform rect;

    [HideInInspector] public UnityEvent onSelected = new UnityEvent();
    [HideInInspector] public UnityEvent onDeselected = new UnityEvent();
    [HideInInspector] public UnityEvent onDragBegin = new UnityEvent();
    [HideInInspector] public UnityEvent onDragged = new UnityEvent();
    [HideInInspector] public UnityEvent onDragEnd = new UnityEvent();

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void RegisterProperty(Property property)
    {
        properties.Add(property);
    }
}
