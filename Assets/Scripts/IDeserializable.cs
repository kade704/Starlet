using Newtonsoft.Json.Linq;

public interface IDeserializable
{
    public void Deserialize(JToken token);
}