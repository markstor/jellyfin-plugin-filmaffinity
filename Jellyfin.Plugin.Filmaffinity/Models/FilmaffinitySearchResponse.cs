using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Filmaffinity.Models;

/// <summary>
/// The Filmaffinity search response.
/// </summary>
/// <typeparam name="T">The type of response object.</typeparam>
public class FilmaffinitySearchResponse<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilmaffinitySearchResponse{T}"/> class.
    /// </summary>
    public FilmaffinitySearchResponse()
    {
        Results = Array.Empty<T>();
    }

    /// <summary>
    /// Gets or sets the list of results.
    /// </summary>
    [JsonPropertyName("results")]
    public IReadOnlyList<T> Results { get; set; }
}
