using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoudSong
{
    public enum Genre
    {
        Lofi,
        Jazz,
        Techno,
        Pop,
        NewWave,
        Other
    }

    class Song
    {
        public string Title { get; set; }
        public string Lyrics { get; set; }
        public string Duration { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int Year { get; set; }
        public bool Favourites { get; set; }
        public Genre Genre { get; set; }

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

        public Song() { }
    }
}
