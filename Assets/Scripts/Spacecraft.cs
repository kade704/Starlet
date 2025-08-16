using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class Spacecraft : MonoBehaviour, ISerializable, IDeserializable
{
    [ReadOnly] public float totalMass;

    [HideInInspector] public new Rigidbody2D rigidbody;
    [HideInInspector] public List<Module> modules = new List<Module>();
    [HideInInspector] public List<Battery> batteries = new List<Battery>();
    [HideInInspector] public List<Storage> storages = new List<Storage>();
    [HideInInspector] public bool running = true;

    [HideInInspector] public UnityEvent<SpacecraftEventInfo> onModuleAttached = new UnityEvent<SpacecraftEventInfo>();
    [HideInInspector] public UnityEvent<SpacecraftEventInfo> onModuleDetached = new UnityEvent<SpacecraftEventInfo>();
    [HideInInspector] public UnityEvent<Module[]> onModuleDetachedComplete = new UnityEvent<Module[]>();
    [HideInInspector] public UnityEvent onDestroyed = new UnityEvent();

    private Dictionary<string, float> signalValues = new();
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        foreach (var signal in signalValues.ToList())
        {
            signalValues[signal.Key] = 0;
        }
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, Vector2.zero, Time.fixedDeltaTime * 1.0f);
        rigidbody.angularVelocity = Mathf.LerpAngle(rigidbody.angularVelocity, 0f, Time.fixedDeltaTime * 1.0f);
    }

    public void AttachModule(Module module, Connector connector)
    {
        if (module.attached)
        {
            Debug.LogWarning("Already Attched Module");
            return;
        }

        if (connector == null)
        {
            module.owner = null;
            module.transform.localPosition = Vector2.zero;
            module.transform.localRotation = Quaternion.identity;
        }
        else
        {
            module.owner = connector;
            connector.dest = module;
            module.transform.localPosition = transform.InverseTransformPoint(connector.transform.position);
            module.transform.localRotation = Quaternion.Inverse(transform.rotation) * connector.transform.rotation;
        }
        module.transform.parent = transform;
       
        module.spacecraft = this;
        module.attached = true;

        modules.Add(module);

        totalMass += module.mass;
        rigidbody.mass = totalMass;

        module.onAttached.Invoke(this);

        var info = new SpacecraftEventInfo
        { 
            module = module,
            parent = connector != null ? connector.origin : null,
            connector = connector
        };
        onModuleAttached.Invoke(info);
    }

    public List<Module> DetachModule(Module module)
    {
        var queue = new Queue<Module>();
        var visited = new List<Module>();
        queue.Enqueue(module);
        while (queue.Count != 0)
        {
            var current = queue.Dequeue();
            visited.Add(current);
            foreach (var connector in current.connectors)
            {
                var child = connector.dest;
                if (child != null)
                    queue.Enqueue(child);
            }
        }

        visited.Reverse();

        foreach (var child in visited)
        {
            if (child.owner == null)
                continue;

            child.onDetached.Invoke(child.spacecraft);

            var info = new SpacecraftEventInfo
            {
                module = child,
                parent = child.owner.origin,
                connector = child.owner
            };
            onModuleDetached.Invoke(info);

            child.transform.parent = null;
            child.attached = false;
            child.owner.dest = null;
            child.owner = null;
            child.spacecraft = null;

            modules.Remove(child);

            totalMass -= child.mass;
            rigidbody.mass = totalMass;
        }

        onModuleDetachedComplete.Invoke(visited.ToArray());

        if (modules.Count == 1)
        {
            Destroy(gameObject);
            onDestroyed.Invoke();
            return new List<Module> { module };
        }

        return visited;
    }

    public bool IsPlayer()
    {
        return tag == "Player"; 
    }

    public float GetSignalValue(string signal)
    {
        return signalValues.ContainsKey(signal) ? signalValues[signal] : 0;
    }

    public void SetSignalValue(string signal, float value, bool force = false)
    {
        value = Mathf.Clamp01(value);

        if (signalValues.ContainsKey(signal))
        {
            if (force || !force && signalValues[signal] < value)
                signalValues[signal] = value;
        }
        else
        {
            if (signal != "")
            {
                signalValues.Add(signal, value);
            }
        }
    }

    public float GetEnergyCapacity()
    {
        float capatity = 0;
        foreach (var battery in batteries)
        {
            capatity += battery.batteryProperty.capacity;
        }
        return capatity;
    }

    public float GetEnergy()
    {
        float energy = 0;
        foreach (var battery in batteries)
        {
            energy += battery.batteryProperty.energy;
        }
        return energy;
    }

    public float GetBatteriesEnergyNorm()
    {
        return GetEnergy() / GetEnergyCapacity();
    }
    
    public bool HasEnergy(float energy)
    {
        return GetEnergy() >= energy;
    }

    public bool EmptyEnergy()
    {
        foreach (var battery in batteries)
        {
            if (battery.batteryProperty.energy > 0)
                return false;
        }
        return true;
    }

    public bool IsFullEnergy()
    {
        foreach(var battery in batteries)
        {
            if (battery.batteryProperty.energy < battery.batteryProperty.capacity) 
                return false;
        }
        return true;
    }

    public void GainEnergy(float energy)
    {
        int index = 0;
        float remainEnergy = energy;
        
        while(index < batteries.Count)
        {
            var battery = batteries[index];
            float diff = battery.batteryProperty.capacity - battery.batteryProperty.energy;
            if (remainEnergy > diff)
            {
                remainEnergy -= diff;
                battery.batteryProperty.energy = battery.batteryProperty.capacity;
            }
            else
            {
                battery.batteryProperty.energy += remainEnergy;
                return;
            }
            index++;
        }
    }

    public void UseEnergy(float energy)
    {
        int index = batteries.Count - 1;
        float remainEnergy = energy;

        while (index >= 0)
        {
            var battery = batteries[index];
            if (remainEnergy > battery.batteryProperty.energy)
            {
                remainEnergy -= battery.batteryProperty.energy;
                battery.batteryProperty.energy = 0;
            }
            else
            {
                battery.batteryProperty.energy -= remainEnergy;
                return;
            }
            index--;
        }
    }

    public void AddBattery(Battery battery)
    {
        batteries.Add(battery);
    }

    public void RemoveBattery(Battery battery)
    {
        batteries.Remove(battery);
    }

    public Storage StoreItem(ItemAsset item)
    {
        foreach (var storage in storages)
        {
            if (storage.storageProperty.inventory.totalAmount < storage.storageProperty.inventory.capacity)
            {
                storage.storageProperty.inventory.AddItem(item, 1);
                return storage;
            }
        }
        return null;
    }

    public void ReleaseItem(ItemAsset item)
    {

    }

    public void AddStorage(Storage storage)
    {
        storages.Add(storage);
    }

    public void RemoveStorage(Storage storage)
    {
        storages.Add(storage);
    }
    
    public void ClearModules()
    {
        foreach (var module in modules)
        {
            Destroy(module.gameObject);
        }
        modules.Clear();
        batteries.Clear();
        storages.Clear();
        totalMass = 0;
        rigidbody.mass = totalMass;
    }

    public JToken Serialize()
    {
        var moduleDatas = new JArray();
        var reservedModule = new List<Module>();

        var baseModule = modules[0];
        var properties = PropertyFunc.ConvertToDatas(baseModule.properties);
        moduleDatas.Add(new JObject
        {
            ["name"] = baseModule.itemAsset.name,
            ["parent"] = -1,
            ["connector"] = -1,
            ["properties"] = properties
        });
        reservedModule.Add(baseModule);

        var currIdx = 0;
        var maxIdx = 1;
        while (currIdx < maxIdx)
        {
            var connectors = reservedModule[currIdx].connectors;
            for (var i = 0; i < connectors.Count; i++)
            {
                var child = connectors[i].dest;
                if (child != null)
                {
                    properties = PropertyFunc.ConvertToDatas(child.properties);
                    moduleDatas.Add(new JObject
                    {
                        ["name"] = child.itemAsset.name,
                        ["parent"] = currIdx,
                        ["connector"] = i,
                        ["properties"] = properties
                    });
                    reservedModule.Add(child);
                    maxIdx++;
                }
            }
            currIdx++;
        }
        return moduleDatas;
    }

    public void Deserialize(JToken token)
    {
        if (token == null || !(token is JArray))
        {
            Debug.LogError("Invalid token format for Spacecraft deserialization.");
            return;
        }

        ClearModules();

        GameResource gameResource = GameManager.Instance.GetSystem<GameResource>();

        foreach (var moduleData in token.Children<JObject>())
        {
            var moduleItems = gameResource.GameResourceAsset.moduleItems;
            var moduleItem = moduleItems.Where(x => x.name == moduleData["name"].ToString()).FirstOrDefault();
            if (moduleItem == null)
            {
                Debug.LogError($"Deserializing Spacecraft Error. Module ID [{moduleData["name"]}] Not Found");
                return;
            }

            Module module;
            if ((int)moduleData["parent"] == -1)
            {
                module = Instantiate(moduleItem.prefab);
                AttachModule(module, null);
            }
            else
            {
                var parent = modules[(int)moduleData["parent"]].connectors[(int)moduleData["connector"]];
                module = Instantiate(moduleItem.prefab);
                AttachModule(module, parent);
            }
            module.itemAsset = moduleItem;
            PropertyFunc.OverwriteFromDatas(module.properties, (JArray)moduleData["properties"]);
        }
    }
}

public struct SpacecraftEventInfo
{
    public Module module;
    public Module parent;
    public Connector connector;
}