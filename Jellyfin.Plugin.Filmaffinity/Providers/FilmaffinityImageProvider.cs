using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Filmaffinity.Providers;

/// <summary>
/// The Filmaffinity image provider.
/// </summary>
public class FilmaffinityImageProvider : IRemoteImageProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFilmaffinityClient _FilmaffinityClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilmaffinityImageProvider"/> class.
    /// </summary>
    /// <param name="httpClientFactory">Instance of the <see cref="IHttpClientFactory"/> interface.</param>
    /// <param name="FilmaffinityClient">Instance of the <see cref="IFilmaffinityClient"/> interface.</param>
    public FilmaffinityImageProvider(
        IHttpClientFactory httpClientFactory,
        IFilmaffinityClient FilmaffinityClient)
    {
        _httpClientFactory = httpClientFactory;
        _FilmaffinityClient = FilmaffinityClient;
    }

    /// <inheritdoc />
    public string Name => "Filmaffinity";

    /// <inheritdoc />
    public bool Supports(BaseItem item)
        => item is MusicVideo;

    /// <inheritdoc />
    public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
    {
        yield return ImageType.Primary;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
    {
        if (!item.TryGetProviderId(FilmaffinityPlugin.ProviderName, out var FilmaffinityId))
        {
            return Enumerable.Empty<RemoteImageInfo>();
        }

        var FilmaffinityVideo = await _FilmaffinityClient.GetVideoIdResultAsync(FilmaffinityId, cancellationToken)
            .ConfigureAwait(false);
        if (string.IsNullOrEmpty(FilmaffinityVideo?.Image?.Size1))
        {
            return Enumerable.Empty<RemoteImageInfo>();
        }

        return new[]
        {
            new RemoteImageInfo
            {
                ProviderName = FilmaffinityPlugin.ProviderName,
                Url = FilmaffinityVideo.Image.Size1,
                Type = ImageType.Primary
            }
        };
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        return _httpClientFactory.CreateClient(NamedClient.Default)
            .GetAsync(new Uri(url), cancellationToken);
    }
}
