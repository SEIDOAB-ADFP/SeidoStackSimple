using Models;
using Models.Music;
using Models.Music.Interfaces;
using Seido.Utilities.SeedGenerator;


namespace AppWorker.Workers;

public class UsingSeeder
{
    private readonly ILogger<UsingSeeder> _logger;

    public UsingSeeder(ILogger<UsingSeeder> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var _seeder = new SeedGenerator();
        var mockMusicGroup = new MusicGroup().Seed(_seeder);
        var mockAlbum = new Album().Seed(_seeder);
        var mockArtist = new Artist().Seed(_seeder);

        var mockArtists = _seeder.ItemsToList<Artist>(5);
        var latins = _seeder.ItemsToList<LatinSentence>(5);
        var quotes = _seeder.ItemsToList<FamousQuote>(5);

        _logger.LogInformation("Mocked MusicGroup: {MusicGroupName}", mockMusicGroup.Name);
        _logger.LogInformation("Mocked Artist: {FirstName}", mockArtist.FirstName);
        _logger.LogInformation("Mocked Album: {AlbumName}", mockAlbum.Name);

        _logger.LogInformation("Mocked Album: {Artists}", string.Join("\r\n", mockArtists.Select(a => a.FirstName).ToList()));
        _logger.LogInformation("Mocked Latin Sentences: {Sentences}", string.Join("\r\n", latins.Select(l => l.Sentence).ToList()));
        _logger.LogInformation("Mocked Quotes: {Quotes}", string.Join("\r\n", quotes.Select(q => q.Quote).ToList()));

        await Task.CompletedTask;
    }
}
