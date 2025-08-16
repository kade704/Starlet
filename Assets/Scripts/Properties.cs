using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SavableProperty : System.Attribute { }
public class SavableField : System.Attribute { }

public abstract class Property
{
    public abstract string GetID();
}

[System.Serializable]
[SavableProperty]
public class SignalProperty : Property
{
    public string name;
    [SavableField] public string value;

    public override string GetID() 
        => $"SignalProperty:{name}";
}

[System.Serializable]
[SavableProperty]
public class FloatProperty : Property
{
    public string name;
    public float min;
    public float max;
    [SavableField] public float value;

    public override string GetID()
        => $"FloatProperty:{name}";
}

[System.Serializable]
public class BatteryProperty : Property
{
    public float energy;
    public float capacity;

    public override string GetID()
        => $"BatteryProperty";
}

[System.Serializable]
public class StorageProperty : Property
{
    public Inventory inventory = new Inventory();

    public override string GetID()
        => $"StorageProperty";
}

public class PropertyFunc
{
    public static void OverwriteFromDatas(List<Property> properties, JArray datas)
    {
        foreach (var property in properties)
        {
            var type = property.GetType();
            if (type.GetCustomAttributes(typeof(SavableProperty), false).Length == 0)
                continue;

            var data = datas.Where(x => x["id"].ToString() == property.GetID()).FirstOrDefault();
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.GetCustomAttributes(typeof(SavableField), false).Length == 0)
                    continue;

                var valueString = data[field.Name].ToString();
                if (field.FieldType == typeof(string))
                {
                    field.SetValue(property, valueString);
                }
                else if (field.FieldType == typeof(float)) 
                {
                    field.SetValue(property, float.Parse(valueString));
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(property, int.Parse(valueString));
                }
            }
        }
    }

    public static JArray ConvertToDatas(List<Property> properties)
    {
        var datas = new JArray();
        foreach (var property in properties)
        {
            var type = property.GetType();
            if (type.GetCustomAttributes(typeof(SavableProperty), false).Length == 0)
                continue;

            var data = new JObject();
            data["id"] = property.GetID();
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.GetCustomAttributes(typeof(SavableField), false).Length == 0)
                    continue;

                var valueString = field.GetValue(property).ToString();
                if (field.FieldType == typeof(string))
                {
                    data[field.Name] = valueString;
                }
                else if (field.FieldType == typeof(float))
                {
                    data[field.Name] = float.Parse(valueString);
                }
                else if (field.FieldType == typeof(int))
                {
                    data[field.Name] = int.Parse(valueString);
                }
            }
            datas.Add(data);
        }
        return datas;
    }
}
