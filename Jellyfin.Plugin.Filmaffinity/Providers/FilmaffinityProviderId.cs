using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Filmaffinity.Providers;

/// <inheritdoc />
public class FilmaffinityProviderId : IExternalId
{
    /// <inheritdoc />
    public string ProviderName
        => FilmaffinityPlugin.ProviderName;

    /// <inheritdoc />
    public string Key
        => FilmaffinityPlugin.ProviderName + "_slug";

    /// <inheritdoc />
    public ExternalIdMediaType? Type
        => ExternalIdMediaType.ReleaseGroup;

    /// <summary>
    /// Gets the url format string.
    /// </summary>
    /// <remarks>
    /// Filmaffinity's url is /{artist}/{song}, so we just store the entire url as the id.
    /// </remarks>
    public string UrlFormatString
        => "{0}";

    /// <inheritdoc />
    public bool Supports(IHasProviderIds item)
        => item is MusicVideo
           || item is MusicArtist
           || item is Person;
}
