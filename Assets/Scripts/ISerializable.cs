using Newtonsoft.Json.Linq;

public interface ISerializable
{
    public JToken Serialize();
}