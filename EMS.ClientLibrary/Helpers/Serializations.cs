using System.Text.Json;

namespace EMS.ClientLibrary.Helpers;

public static class Serializations
{
    public static string SerializationObj<T>(T modelObject) => JsonSerializer.Serialize(modelObject);

    public static T? DeserializeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString);

    public static IList<T>? DesetializeJsonStringList<T>(string jsonString) => JsonSerializer.Deserialize < IList<T>>(jsonString);
}