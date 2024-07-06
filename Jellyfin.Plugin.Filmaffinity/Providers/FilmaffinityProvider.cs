using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Data.Enums;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Filmaffinity.Providers;

/// <summary>
/// Filmaffinity Provider.
/// </summary>
public class FilmaffinityProvider : IRemoteMetadataProvider<MusicVideo, MusicVideoInfo>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FilmaffinityProvider> _logger;
    private readonly IFilmaffinityClient _FilmaffinityClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilmaffinityProvider"/> class.
    /// </summary>
    /// <param name="httpClientFactory">Instance of the <see cref="IHttpClientFactory"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{FilmaffinityProvider}"/> interface.</param>
    /// <param name="FilmaffinityClient">Instance of the <see cref="IFilmaffinityClient"/> interface.</param>
    public FilmaffinityProvider(
        IHttpClientFactory httpClientFactory,
        ILogger<FilmaffinityProvider> logger,
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
    public async Task<MetadataResult<MusicVideo>> GetMetadata(MusicVideoInfo info, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Get metadata result for {Name}", info.Name);
        var FilmaffinityId = info.GetProviderId(FilmaffinityPlugin.ProviderName);
        var result = new MetadataResult<MusicVideo>
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
        var releaseResult = await _FilmaffinityClient.GetVideoIdResultAsync(FilmaffinityId, cancellationToken)
            .ConfigureAwait(false);
        if (releaseResult != null)
        {
            result.HasMetadata = true;
            // set properties from data
            result.Item = new MusicVideo
            {
                Name = releaseResult.SongTitle,
                ProductionYear = releaseResult.Year,
                Artists = releaseResult.Artists.Select(i => i.Name).ToArray()
            };

            if (!string.IsNullOrEmpty(releaseResult.Image?.Size1))
            {
                result.Item.ImageInfos = [new ItemImageInfo { Path = releaseResult.Image.Size1 }];
            }

            foreach (var director in releaseResult.Directors)
            {
                result.AddPerson(new PersonInfo
                {
                    Name = director.Name,
                    ProviderIds = new Dictionary<string, string>
                    {
                        { FilmaffinityPlugin.ProviderName, director.Id.ToString(CultureInfo.InvariantCulture) },
                        { FilmaffinityPlugin.ProviderName + "_slug", director.Url },
                    },
                    Type = PersonKind.Director
                });
            }

            result.Item.SetProviderId(FilmaffinityPlugin.ProviderName, FilmaffinityId);

            if (!string.IsNullOrEmpty(releaseResult.Url))
            {
                result.Item.SetProviderId(FilmaffinityPlugin.ProviderName + "_slug", releaseResult.Url);
            }
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MusicVideoInfo searchInfo, CancellationToken cancellationToken)
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
                    Name = r.SongTitle,
                    ProductionYear = r.Year,
                    Artists = r.Artists.Select(a => new RemoteSearchResult { Name = a.Name }).ToArray(),
                    ImageUrl = r.Image?.Size1,
                };

                result.SetProviderId(FilmaffinityPlugin.ProviderName, r.Id.ToString(CultureInfo.InvariantCulture));

                return result;
            });
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        return _httpClientFactory.CreateClient(NamedClient.Default)
            .GetAsync(new Uri(url), cancellationToken);
    }
}
