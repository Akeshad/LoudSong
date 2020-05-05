using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoudSong
{
    // This enum symbolizes the different song genres there are for the app.
    public enum Genre
    {
        Lofi,
        Jazz,
        Techno,
        Pop,
        NewWave,
        Other
    }

    // This class represents the data structure which composes a Song for this app. It is used mainly in order to retrieve information from the database when a query is requested.
    class Song
    {

        #region Properties

        public string Title { get; set; } // Property for a Song's Title.

        public string Lyrics { get; set; } // Property for a Song's Lyrics.

        public string Duration { get; set; } // Property for a Song's Duration.

        public string Artist { get; set; } // Property for a Song's Artist.

        public string Album { get; set; } // Property for a Song's Album.

        public int Year { get; set; } // Property for a Song's Year.

        public bool Favourites { get; set; } // Property for a Song's Favourite Status. True if it is, False if it isn't.

        public Genre Genre { get; set; } // Property for a Song's Genre enum.

        #endregion

        #region Constructors

        // Main Song constructor, uses all its fields.
        public Song(string title, string lyrics, string duration, string artist, string album, int year, bool favourites, Genre genre)
        {
            Title = title;
            Lyrics = lyrics;
            Duration = duration;
            Artist = artist;
            Album = album;
            Year = year;
            Favourites = favourites;
            Genre = genre;
        }

        // An empty Song constructor.
        public Song() { }

        #endregion
    }
}
