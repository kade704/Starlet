using UnityEngine;

[RequireComponent(typeof(Module))]
public class Storage : MonoBehaviour
{
    public StorageProperty storageProperty;

    private Module module;

    private void Awake()
    {
        module = GetComponent<Module>();
        module.RegisterProperty(storageProperty);
        module.onAttached.AddListener(OnAttached);
        module.onDetached.AddListener(OnDetached);
    }

    private void OnAttached(Spacecraft spacecraft)
    {
        module.spacecraft.AddStorage(this);
    }

    private void OnDetached(Spacecraft spacecraft)
    {
        module.spacecraft.RemoveStorage(this);
    }
}
