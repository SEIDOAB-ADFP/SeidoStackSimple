using Models;
using System.Linq;
using Seido.Utilities.SeedGenerator;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Services.Seeder;
using Models.Music.Interfaces;
using Models.Music;

namespace AppWorker.Mocking;

public static partial class SeederMocking
{
    public static SeederBuilder MockMusic(this SeederBuilder seedBuilder)
    {       
        seedBuilder.Configure(options =>
        {
            options.AddMocker<IArtist, Artist>((seeder, artist) =>
            {
                artist.ArtistId = Guid.NewGuid();
                artist.FirstName = seeder.FirstName;
                artist.LastName = seeder.LastName;
                artist.BirthDay = (seeder.Bool) ? seeder.DateAndTime(1940, 1990) : null;
                return artist;
            });

            options.AddMocker<IAlbum, Album>((seeder, album) =>
            {
                album.AlbumId = Guid.NewGuid();
                album.Name = seeder.MusicAlbumName;
                album.CopiesSold = seeder.Next(1_000, 1_000_000);
                album.ReleaseYear = seeder.Next(1970, 2024);
                return album;       
            });

            options.AddMocker<IMusicGroup, MusicGroup>((seeder, musicGroup) =>
            {
                musicGroup.MusicGroupId = Guid.NewGuid();
                musicGroup.Name = seeder.MusicGroupName;
                musicGroup.EstablishedYear = seeder.Next(1970, 2024);
                musicGroup.Genre = seeder.FromEnum<MusicGenre>();
                return musicGroup;
            });
        });
        return seedBuilder;
    }
}