using Models.Music.Interfaces;

namespace Models.Music;

public class Album : IAlbum
{
    public virtual Guid AlbumId { get; set; }

    public virtual string Name { get; set; }
    public virtual int ReleaseYear { get; set; }
    public virtual long CopiesSold { get; set; }

    //Model relationships
    public virtual IMusicGroup MusicGroup { get; set; } = null;

    #region Constructors
    public Album(){}
    public Album(Album org)
    {
        this.AlbumId = org.AlbumId;
        this.Name = org.Name;
        this.ReleaseYear = org.ReleaseYear;
        this.CopiesSold = org.CopiesSold;
    }
    #endregion
}


