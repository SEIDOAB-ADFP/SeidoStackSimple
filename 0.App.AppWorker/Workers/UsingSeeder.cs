using Models;
using Models.Music.Interfaces;
using Services.Seeder;

namespace AppWorker.Workers;

public class UsingSeeder
{
    private readonly ILogger<UsingSeeder> _logger;
    private readonly SeederService _seedService;

    public UsingSeeder(ILogger<UsingSeeder> logger, SeederService seedService)
    {
        _logger = logger;
        _seedService = seedService;
    }

    public async Task ExecuteAsync()
    {
        var mockMusicGroup = _seedService.Mock<IMusicGroup>();
        var mockAlbum = _seedService.Mock<IAlbum>();
        var mockArtist = _seedService.Mock<IArtist>();

        var mockArtists = _seedService.MockMany<IArtist>(5);
        var latins = _seedService.MockMany<LatinSentence>(5);
        var quotes = _seedService.MockMany<FamousQuote>(5);

        _logger.LogInformation("Mocked MusicGroup: {MusicGroupName}", mockMusicGroup.Name);
        _logger.LogInformation("Mocked Artist: {FirstName}", mockArtist.FirstName);
        _logger.LogInformation("Mocked Album: {AlbumName}", mockAlbum.Name);

        _logger.LogInformation("Mocked Album: {Artists}", string.Join("\r\n", mockArtists.Select(a => a.FirstName).ToList()));
        _logger.LogInformation("Mocked Latin Sentences: {Sentences}", string.Join("\r\n", latins.Select(l => l.Sentence).ToList()));
        _logger.LogInformation("Mocked Quotes: {Quotes}", string.Join("\r\n", quotes.Select(q => q.Quote).ToList()));

        await Task.CompletedTask;
    }
}
