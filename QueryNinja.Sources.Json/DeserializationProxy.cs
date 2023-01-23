using Newtonsoft.Json.Linq;

namespace QueryNinja.Sources.Json;

internal class DeserializationProxy
{
    public JObject? Components { get; set; }
    public JObject? Selectors { get; set; }
}