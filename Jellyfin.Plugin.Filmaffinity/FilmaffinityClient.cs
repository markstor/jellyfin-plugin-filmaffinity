using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Extensions.Json;
using Jellyfin.Plugin.Filmaffinity.Models;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Filmaffinity;

/// <summary>
/// The Filmaffinity client.
/// </summary>
public class FilmaffinityClient : IFilmaffinityClient
{
    private const string BaseUrl = "https://Filmaffinity.com/api/v1";
    private static readonly JsonSerializerOptions _jsonSerializerOptions = JsonDefaults.Options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FilmaffinityClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilmaffinityClient"/> class.
    /// </summary>
    /// <param name="httpClientFactory">Instance of the <see cref="IHttpClientFactory"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{FilmaffinityClient}"/> interface.</param>
    public FilmaffinityClient(
        IHttpClientFactory httpClientFactory,
        ILogger<FilmaffinityClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<FilmaffinitySearchResponse<FilmaffinityVideo>?> GetSearchResponseAsync(MusicVideoInfo searchInfo, CancellationToken cancellationToken)
    {
        var queryValue = new StringBuilder();
        queryValue.Append(searchInfo.Name);
        foreach (var artist in searchInfo.Artists)
        {
            queryValue.Append('+')
                .Append(artist);
        }

        var url = $"{BaseUrl}/search/videos?q={queryValue}";
        return await GetResponseAsync<FilmaffinitySearchResponse<FilmaffinityVideo>>(url, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<FilmaffinitySearchResponse<FilmaffinityArtist>?> GetSearchResponseAsync(ArtistInfo searchInfo, CancellationToken cancellationToken)
    {
        var url = $"{BaseUrl}/search/entities?q={searchInfo.Name}";
        return await GetResponseAsync<FilmaffinitySearchResponse<FilmaffinityArtist>>(url, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<FilmaffinityVideo?> GetVideoIdResultAsync(string FilmaffinityId, CancellationToken cancellationToken)
    {
        var apiKey = GetApiKey();
        if (apiKey == null)
        {
            return null;
        }

        var url = $"{BaseUrl}/video/{FilmaffinityId}";
        return await GetResponseAsync<FilmaffinityVideo>(url, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<FilmaffinityArtist?> GetArtistIdResultAsync(string FilmaffinityId, CancellationToken cancellationToken)
    {
        var apiKey = GetApiKey();
        if (apiKey == null)
        {
            return null;
        }

        var url = $"{BaseUrl}/video/{FilmaffinityId}";
        return await GetResponseAsync<FilmaffinityArtist>(url, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<T?> GetResponseAsync<T>(string url, CancellationToken cancellationToken)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        requestMessage.Headers.TryAddWithoutValidation("Filmaffinity-APP-KEY", GetApiKey());
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        var response = await _httpClientFactory.CreateClient(NamedClient.Default)
            .SendAsync(requestMessage, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    private string? GetApiKey()
    {
        var apiKey = FilmaffinityPlugin.Instance?.Configuration.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("ApiKey is unset");
        }

        return apiKey;
    }
}
