using Models.Music.Interfaces;

namespace Models.Music;

public class MusicGroup : IMusicGroup
{
    public virtual Guid MusicGroupId { get; set; }
    public virtual string Name { get; set; }
    public virtual int EstablishedYear { get; set; }

    public virtual MusicGenre Genre { get; set; }

    //Model relationships
    public virtual List<IAlbum> Albums { get; set; } = new List<IAlbum>();
    public virtual List<IArtist> Artists { get; set; } = new List<IArtist>();

 
    #region Constructors
    public MusicGroup(){}
    public MusicGroup(MusicGroup org)
    {
        MusicGroupId = org.MusicGroupId;
        Name = org.Name;
        EstablishedYear = org.EstablishedYear;
        Genre = org.Genre;
    }
    #endregion
}


