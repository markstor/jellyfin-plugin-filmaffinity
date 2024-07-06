using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Filmaffinity.Models;

/// <summary>
/// The Filmaffinity Artist dto.
/// </summary>
public class FilmaffinityArtist
{
    /// <summary>
    /// Gets or sets the artist id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the artist name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the artist url.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the discogs id.
    /// </summary>
    [JsonPropertyName("discogs_id")]
    public int DiscogsId { get; set; }
}
