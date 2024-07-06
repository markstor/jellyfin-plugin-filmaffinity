using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jellyfin.Plugin.Filmaffinity.Models;

namespace Jellyfin.Plugin.Filmaffinity;

/// <summary>
///     Json converter to ignore empty array when response should be an object.
/// </summary>
public class JsonImageResponseConverter : JsonConverter<FilmaffinityImage?>
{
    /// <inheritdoc />
    public override FilmaffinityImage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // For some reason Filmaffinity returns an empty array instead of an empty object or null when no results found.
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            // Read end array.
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException("Found actual data, expected empty array");
            }

            return null;
        }

        return JsonSerializer.Deserialize<FilmaffinityImage>(ref reader, options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, FilmaffinityImage? value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
