using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Filmaffinity.Providers;

/// <summary>
/// Filmaffinity artist provider.
/// </summary>
public class FilmaffinityArtistProvider : IRemoteMetadataProvider<MusicArtist, ArtistInfo>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FilmaffinityArtistProvider> _logger;
    private readonly IFilmaffinityClient _FilmaffinityClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilmaffinityArtistProvider"/> class.
    /// </summary>
    /// <param name="httpClientFactory">Instance of the <see cref="IHttpClientFactory"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{FilmaffinityArtistProvider}"/> interface.</param>
    /// <param name="FilmaffinityClient">Instance of the <see cref="IFilmaffinityClient"/> interface.</param>
    public FilmaffinityArtistProvider(
        IHttpClientFactory httpClientFactory,
        ILogger<FilmaffinityArtistProvider> logger,
        IFilmaffinityClient FilmaffinityClient)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _FilmaffinityClient = FilmaffinityClient;
    }

    /// <inheritdoc />
    public string Name
        => FilmaffinityPlugin.ProviderName;

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(ArtistInfo searchInfo, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Get search result for {Name}", searchInfo.Name);

        var searchResults = await _FilmaffinityClient.GetSearchResponseAsync(searchInfo, cancellationToken)
            .ConfigureAwait(false);
        if (searchResults == null)
        {
            return Enumerable.Empty<RemoteSearchResult>();
        }

        return searchResults.Results.Select(
            r =>
            {
                var result = new RemoteSearchResult
                {
                    Name = r.Name
                };

                result.SetProviderId(FilmaffinityPlugin.ProviderName, r.Id.ToString(CultureInfo.InvariantCulture));

                return result;
            });
    }

    /// <inheritdoc />
    public async Task<MetadataResult<MusicArtist>> GetMetadata(ArtistInfo info, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Get metadata result for {Name}", info.Name);
        var FilmaffinityId = info.GetProviderId(FilmaffinityPlugin.ProviderName);
        var result = new MetadataResult<MusicArtist>
        {
            HasMetadata = false
        };

        // Filmaffinity id not provided, find first result.
        if (string.IsNullOrEmpty(FilmaffinityId))
        {
            var searchResults = await GetSearchResults(info, cancellationToken)
                .ConfigureAwait(false);
            searchResults.FirstOrDefault()?.TryGetProviderId(FilmaffinityPlugin.ProviderName, out FilmaffinityId);
        }

        // No results found, return without populating metadata.
        if (string.IsNullOrEmpty(FilmaffinityId))
        {
            return result;
        }

        // do lookup here by Filmaffinity id
        var releaseResult = await _FilmaffinityClient.GetArtistIdResultAsync(FilmaffinityId, cancellationToken)
            .ConfigureAwait(false);
        if (releaseResult != null)
        {
            result.HasMetadata = true;
            // set properties from data
            result.Item = new MusicArtist
            {
                Name = releaseResult.Name
            };

            result.Item.SetProviderId(FilmaffinityPlugin.ProviderName, FilmaffinityId);

            if (!string.IsNullOrEmpty(releaseResult.Url))
            {
                result.Item.SetProviderId(FilmaffinityPlugin.ProviderName + "_slug", releaseResult.Url);
            }
        }

        return result;
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        return _httpClientFactory.CreateClient(NamedClient.Default)
            .GetAsync(new Uri(url), cancellationToken);
    }
}
