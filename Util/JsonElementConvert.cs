
using System.Reflection;
using System.Text.Json;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Util
{
    public static class JsonElementConvert
    {
        public static object ConvertToObject(JsonElement jsonElement)
        {
            return IdentifierTypeToProcesss(jsonElement);
        }

        private static object IdentifierTypeToProcesss(JsonElement jsonElement)
        {
            if(JsonMatchesType<ClientMine>(jsonElement))
            {
                var obj = jsonElement.Deserialize<ClientMine>();
                return obj!;
            }
            else if(JsonMatchesType<LogEntry>(jsonElement))
            {
                var obj = jsonElement.Deserialize<LogEntry>();
                return obj!;
            }
            else if(JsonMatchesType<ServerListener>(jsonElement))
            {
                var obj = jsonElement.Deserialize<ServerListener>();
                return obj!;
            }
            else if(JsonMatchesType<SendMessageDefault>(jsonElement))
            {
                var obj = jsonElement.Deserialize<SendMessageDefault>();
                return obj!;
            }
            else if(JsonMatchesType<string>(jsonElement))
            {
                var obj = jsonElement.Deserialize<string>();
                return obj!;
            }
            else
            {
                throw new ArgumentException("Unsupported data type", nameof(jsonElement));
            }
        }
        private static bool JsonMatchesType<T>(JsonElement element)
        {
            var propertyJson = element.EnumerateObject().Select(p => p.Name).ToHashSet();
            var propertyClass = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name).ToHashSet();

            return propertyJson.SetEquals(propertyClass);
        }
    }
}