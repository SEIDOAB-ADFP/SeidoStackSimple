using Models.Music.Interfaces;

namespace Models.Music;
public class Artist : IArtist
{
    public virtual Guid ArtistId { get; set; }

    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }

    public virtual DateTime? BirthDay { get; set; }

    //Model relationships
    public virtual List<IMusicGroup> MusicGroups { get; set; } = new List<IMusicGroup>();

    #region Constructors
    public Artist(){}
    public Artist(Artist org)
    {
        this.ArtistId = org.ArtistId;
        this.FirstName = org.FirstName;
        this.LastName = org.LastName;
        this.BirthDay = org.BirthDay;
    }
    #endregion
}


