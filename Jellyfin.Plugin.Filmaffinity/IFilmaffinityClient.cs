using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Filmaffinity.Models;
using MediaBrowser.Controller.Providers;

namespace Jellyfin.Plugin.Filmaffinity;

/// <summary>
/// The Filmaffinity client interface.
/// </summary>
public interface IFilmaffinityClient
{
    /// <summary>
    /// Gets the search result.
    /// </summary>
    /// <param name="searchInfo">The search info.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The Filmaffinity search response.</returns>
    public Task<FilmaffinitySearchResponse<FilmaffinityVideo>?> GetSearchResponseAsync(
        MusicVideoInfo searchInfo,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the artist search result.
    /// </summary>
    /// <param name="searchInfo">The search info.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The Filmaffinity search response.</returns>
    public Task<FilmaffinitySearchResponse<FilmaffinityArtist>?> GetSearchResponseAsync(
        ArtistInfo searchInfo,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get result by id.
    /// </summary>
    /// <param name="filmaffinityId">The IMBDb id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The Filmaffinity video.</returns>
    public Task<FilmaffinityVideo?> GetVideoIdResultAsync(
        string filmaffinityId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get result by id.
    /// </summary>
    /// <param name="filmaffinityId">The IMBDb id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The Filmaffinity video.</returns>
    public Task<FilmaffinityArtist?> GetArtistIdResultAsync(
        string filmaffinityId,
        CancellationToken cancellationToken);
}
