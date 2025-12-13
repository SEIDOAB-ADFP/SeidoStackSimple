using Newtonsoft.Json;

namespace Configuration;

public class AbstractConverter<TInterface, TInstance> : JsonConverter where TInstance : TInterface
{
    public override Boolean CanConvert(Type objectType)
        => objectType == typeof(TInterface);

    public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        => serializer.Deserialize<TInstance>(reader);

    public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        => serializer.Serialize(writer, value);
}

