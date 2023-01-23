using Newtonsoft.Json;
using QueryNinja.Core.Extensibility;

namespace QueryNinja.Sources.Json;

public static class RegistrationExtensions
{
    /// <summary>
    /// Extends Global <see cref="JsonConvert.DefaultSettings"/> with specialized converters.
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static IExtensionsSettings WithJsonSource(this IExtensionsSettings settings)
    {
        var @default = JsonConvert.DefaultSettings;
        JsonConvert.DefaultSettings = () =>
        {
            var jsonSerializerSettings = @default?.Invoke() ?? new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new QueryNinjaConverter());
            return jsonSerializerSettings;
        };

        return settings;
    }
}