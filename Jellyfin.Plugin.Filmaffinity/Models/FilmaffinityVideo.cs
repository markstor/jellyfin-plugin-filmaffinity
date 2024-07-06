using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Filmaffinity.Models;

/// <summary>
/// Filmaffinity Video dto.
/// </summary>
public class FilmaffinityVideo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilmaffinityVideo"/> class.
    /// </summary>
    public FilmaffinityVideo()
    {
        Artists = Array.Empty<FilmaffinityArtist>();
        Directors = Array.Empty<FilmaffinityDirector>();
    }

    /// <summary>
    /// Gets or sets the item id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the song title.
    /// </summary>
    [JsonPropertyName("song_title")]
    public string? SongTitle { get; set; }

    /// <summary>
    /// Gets or sets the Filmaffinity url.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the year.
    /// </summary>
    [JsonPropertyName("year")]
    public int? Year { get; set; }

    /// <summary>
    /// Gets or sets the list of artists.
    /// </summary>
    [JsonPropertyName("artists")]
    public IReadOnlyList<FilmaffinityArtist> Artists { get; set; }

    /// <summary>
    /// Gets or sets the images.
    /// </summary>
    [JsonPropertyName("image")]
    [JsonConverter(typeof(JsonImageResponseConverter))]
    public FilmaffinityImage? Image { get; set; }

    /// <summary>
    /// Gets or sets the directors.
    /// </summary>
    [JsonPropertyName("directors")]
    public IReadOnlyList<FilmaffinityDirector> Directors { get; set; }
}
