using Services.Music.Interfaces;

namespace AppWorker.Workers;

public class UsingWebApi
{
    private readonly ILogger<UsingWebApi> _logger;
    private readonly IMusicGroupsService _musicGroupsService;
    private readonly IAlbumsService _albumsService;
    private readonly IArtistsService _artistsService;

    public UsingWebApi(ILogger<UsingWebApi> logger, IMusicGroupsService musicGroupsService,
    IAlbumsService albumsService, IArtistsService artistsService)
    {
        _logger = logger;
        _musicGroupsService = musicGroupsService;
        _albumsService = albumsService;
        _artistsService = artistsService;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            var musicgroups = await _musicGroupsService.ReadMusicGroupsAsync(true, false, null, 1, 10);
            var albums = await _albumsService.ReadAlbumsAsync(true, false, null, 1, 10);
            var artists = await _artistsService.ReadArtistsAsync(true, false, null, 1, 10);

            _logger.LogInformation("Musicgroups: {Musicgroups}", string.Join("\r\n", musicgroups.PageItems.Select(a => a.Name).ToList()));
            _logger.LogInformation("Albums: {Albums}", string.Join("\r\n", albums.PageItems.Select(a => a.Name).ToList()));
            _logger.LogInformation("Artists: {Artists}", string.Join("\r\n", artists.PageItems.Select(a => a.FirstName + " " + a.LastName).ToList()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in UsingWebApi");
        }
        await Task.CompletedTask;
    }
}
