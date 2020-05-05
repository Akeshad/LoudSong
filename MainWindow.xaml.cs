using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using MySql.Data.MySqlClient;

namespace LoudSong
{
    // MainWindow represents the app's window. This class contains private fields and event handlers which manages all the user's interface. Extends from WPF Window.
    public partial class MainWindow : Window
    {

        #region Private Fields

        private MySqlConnection connection; // MySQL Database connection.
        private ListBoxItem currentTrack; // Current song displayed at the ListBox.
        private ListBoxItem previousTrack; // Previous song displayed at the ListBox.
        private Brush currentTrackIndicator; // Foreground color for the current playing song in the ListBox.
        private Brush trackColor; // Foreground color for the majority of song in the ListBox.
        private DispatcherTimer timer; // Timer for calculating spans of time.
        private CustomDialogWindow customDialogWindow; // Custom DialogWindow for user inputs.
        private CustomDialogWindowGenre windowsGenre; // Custom DialogWindow for choosing the Song's Genre.
        private Song song; // Current song being played after retrieving information from the database.

        private SolidColorBrush toggleOff = new SolidColorBrush(Color.FromRgb(160, 160, 160)); // Color when the Favourite toggle option is 'Off'.  
        private SolidColorBrush toggleOn = new SolidColorBrush(Color.FromRgb(130, 190, 125)); // Color when the Favourite toggle option is 'On'. 

        private bool isDragging; // Boolean which represents if a the user is dragging the left slider. True = it IS being dragged | False = it IS NOT being dragged.
        private bool MediaElementIsPaused = false; // Boolean which represents if the song is paused. True = Paused | False = NOT Paused.
        private string songGenre; // The Song's Genre for storing into the database.
        private string songTitle; // The Song's Title for storing into the database.
        private string songLyrics; // The Song's Lyrics for storing into the database.
        private string songDuration; // The Song's Duration for storing into the database.
        private string songArtist; // The Song's Artist for storing into the database.
        private string songAlbum; // The Song's Album for storing into the database.
        private bool songFavourite; // The Song's bool for stating if it's an user's favourite song. True = it IS a favourite song | False = it IS NOT a favourite song.
        private int songYear; // The Song's Year for storing into the database.

        #endregion

        #region Constructor

        // Main and only constructor for building a MainWindow for the app.
        public MainWindow()
        {
            this.InitializeComponent(); // Initializes the WPF Window. Mandatory.

            currentTrack = null;
            previousTrack = null;
            currentTrackIndicator = Brushes.DarkBlue; // Sets the current song in a purple foreground.
            trackColor = listSongsReproduction.Foreground;  // Sets the rest of songs in a different foreground color.

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Sets the tick span of time: 1 second.
            timer.Tick += new EventHandler(timer_Tick); // Sets the event handler to happen each 1 passed second.

            isDragging = false;
        }

        #endregion

        #region Event Handlers

        #region Window's Button Controls

        // Event handler which closes the app.
        private void btnClose_CloseApp(object sender, RoutedEventArgs e) 
        {
            try
            {
                Close(); // Closes the app.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Event handler which minimizes the window.
        private void btnMin_MinApp(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowState = WindowState.Minimized; // Minimizes the window.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        #endregion

        #region Window's MediaElement

        // Event handler which activates when a song is opened in the app.
        private void musicPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                TimeSpan ts = musicPlayer.NaturalDuration.TimeSpan; // Sets a TimeSpan based on the song's length being played at the music player MediaElement.
                sliderTimeSong.Maximum = ts.TotalSeconds; // Sets the Slider's maximum duration to the previous TimeSpan.
                timer.Start(); // Starts the timer.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Event handler which activates when a song has naturally ended without pressing the 'Stop' Button.
        private void musicPlayer_MediaEnded(object sender, RoutedEventArgs e) 
        {
            try
            {
                sliderTimeSong.Value = 0; // When the song has ended, sets the Slider's current value to 0 to indicate it has finished.
                moveToNextTrack(); // Calls to the function which plays the next track (if there's one).
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        #endregion

        #region Timer

        // Event handler which activates for each 'tick' or set span of time for the DispatcherTimer.
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!isDragging)
                {
                    sliderTimeSong.Value = musicPlayer.Position.TotalSeconds; // Slider's position is updated each second.
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        #endregion

        #region ListBox

        // Event handler which activates when a song is being dragged into the ListBox.
        private void listBox_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) // Occurs when the dragged file extension is supported by the system.
                {
                    e.Effects = DragDropEffects.Copy; // Allows the drag and drop option for such file.
                }
                else // Occurs when the dragged file extension is NOT supported by the system.
                {
                    e.Effects = DragDropEffects.None; // Nothing happens.
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Event handler which activates when a song is being dropped in the ListBox.
        private void listBox1_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false); // String Array for one or several music files.

                foreach (string fileName in s) // Loops every item within the previous String array.
                {
                    if (System.IO.Path.GetExtension(fileName) == ".mp3" || System.IO.Path.GetExtension(fileName) == ".MP3" || // Music file extensions.
                        System.IO.Path.GetExtension(fileName) == ".wav" || System.IO.Path.GetExtension(fileName) == ".WAV" ||
                        System.IO.Path.GetExtension(fileName) == ".wma" || System.IO.Path.GetExtension(fileName) == ".WMA" ||
                        System.IO.Path.GetExtension(fileName) == ".aac" || System.IO.Path.GetExtension(fileName) == ".AAC" ||
                        System.IO.Path.GetExtension(fileName) == ".wmv" || System.IO.Path.GetExtension(fileName) == ".WMV" || // Video file extensions.
                        System.IO.Path.GetExtension(fileName) == ".mp4" || System.IO.Path.GetExtension(fileName) == ".MP4") // Checks if the dropped file has one of previous stated extensions.
                    {
                        ListBoxItem lstItem = new ListBoxItem();
                        lstItem.Content = System.IO.Path.GetFileNameWithoutExtension(fileName); // Prepares a ListItem for the ListBox with the current file's name.
                        lstItem.Tag = fileName; // Sets the Tag for said file.
                        listSongsReproduction.Items.Add(lstItem); // Adds the prepared ListItem to the ListBox.
                    }
                }
                if (currentTrack == null) // Checks if it's the first dragged and dropped file.
                {
                    listSongsReproduction.SelectedIndex = 0; // Sets the ListBox Index to the first one.
                    playOrPauseTrack(); // Calls this method for playing or pausing the song.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Event handler which activates when the user double clicks on the ListBox playlist.
        private void playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            openFiles(); // Function which allows to select one or many music files in order to be added to the playlist.
        }

        // Event handler which allows the user to move the window while left clicking and dragging on a invisible TextBlock situated at the top border of the window.
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // Allows to move the window.
        }

        #endregion

        #region CheckBox

        // Event handler which activates when the toggle button is being checked or unchecked.
        private void Bu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (toggleFav.Toggled1 == true) // Toggle On.
            {
                Light.Fill = toggleOn; // Sets the color On to the toggle.
                songFavourite = true;
            }
            else // Toggle Off.
            {
                Light.Fill = toggleOff; // Sets the color Off to the toggle.
                songFavourite = false;
            }
        }

        #endregion

        #region Buttons

        // Event handler which activates the 'Play' Button is pressed with a mouse's left click. It changes from the 'Play' to the 'Pause' Button and viceversa.
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listSongsReproduction.HasItems) // Checks if there is at least one song in the playlist.
                {
                    playOrPauseTrack(); // Function which plays or pauses the current song.
                }
                else
                {
                    MessageBox.Show("There are no songs in the playlist!", "Error - Play / Pause Song"); // This is displayed if the user clicks on the button with no songs in the playlist.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Event handler which activates the the 'Stop' Button is pressed with a mouse's left click. Resets the song to its beginning.
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listSongsReproduction.HasItems) // Checks if there is at least one song in the playlist.
                {
                    stop(); // Function which stops the current song.
                }
                else
                {
                    MessageBox.Show("There are no songs in the playlist!", "Error - Stop Song"); // This is displayed if the user clicks on the button with no songs in the playlist.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Event handler which activates the next song in the playlist. If there's just one, it reproduces the same one.
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listSongsReproduction.HasItems) // Checks if there is at least one song in the playlist.
                {
                    moveToNextTrack(); // Function which plays the next song in the playlist.
                }
                else
                {
                    MessageBox.Show("There are no songs in the playlist!", "Error - Next Song"); // This is displayed if the user clicks on the button with no songs in the playlist.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Event handler which activates the previous song in the playlist. If there's just one, it reproduces the same one.
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listSongsReproduction.HasItems) // Checks if there is at least one song in the playlist.
                {
                    moveToPreviousTrack(); // Function which plays the previous song in the playlist.
                }
                else
                {
                    MessageBox.Show("There are no songs in the playlist!", "Error - Previous Song"); // This is displayed if the user clicks on the button with no songs in the playlist.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Event handler which activates the selection of music files in the user's Windows OS. You can select one or many files at once in order to add to the playlist.
        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                openFiles(); // Function which allows to select one or many music files in order to be added to the playlist.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Event handler which activates the process to save information about a song into the database.
        private void btnSetInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listSongsReproduction.HasItems) // Checks if there's at least one song in the playlist before attempting to save information.
                {
                    connection = new MySqlConnection(DataBaseUtil.MySQLDatabaseConnection); // Initializing the connection's settings.
                    connection.Open(); // Opening said MySQL Connection.

                    MySqlCommand queryCreateDB = new MySqlCommand(DataBaseUtil.MySQLCreateDatabase, connection); // Preparing the command which creates the MySQL database.
                    MySqlCommand queryUseDB = new MySqlCommand(DataBaseUtil.MySQLUseDatabase, connection); // Preparing the command which selects the just created database.
                    MySqlCommand queryCreateTable = new MySqlCommand(DataBaseUtil.MySQLCreateTable, connection); // Preparing the command which creates the MySQL Table which saves the songs' information.
                    MySqlCommand queryInsertSong; // Preparing the command which inserts a Song in the MySQL Table.

                    queryCreateDB.ExecuteNonQuery(); // Creating the MySQL Database.
                    queryUseDB.ExecuteNonQuery(); // Selecting the just created database.
                    queryCreateTable.ExecuteNonQuery(); // Preparing the command which creates the MySQL Table which saves the songs' information.

                    customDialogWindow = new CustomDialogWindow("Write the song's title. Please, check you're writing it right!", 
                        "You wrote an invalid song's title! Please, check the following hints:\n\n- You didn't write anything in the box below.\n- The song's title exceeds 50 characters.", 50, false, false); // Assigning CustomDialogWindow for the Song's Title.
                    customDialogWindow.ShowDialog(); // Displaying the CustomDialogWindow.
                    songTitle = customDialogWindow.returnInfo(); // Storing the Song's Title into its variable thanks to the user's input in the CustomDialogWindow.

                    windowsGenre = new CustomDialogWindowGenre(); // Assigning CustomDialogWindowGenre for the Song's Genre.
                    windowsGenre.ShowDialog(); // Displaying the CustomDialogWindowGenre.
                    songGenre = CustomDialogWindowGenre.genreString; // Storing the Song's Genre into its variable thanks to the user's input in the CustomDialogWindowGenre.

                    customDialogWindow = new CustomDialogWindow("Paste the song's lyrics. If the song has no lyrics, just write a hyphen (-)!",
                        "Oh, something went wrong! Please, check the following hints:\n\n- You didn't write the lyrics or a hyphen (-).", 4000, false, false); // Assigning CustomDialogWindow for the Song's Lyrics.
                    customDialogWindow.ShowDialog(); // Displaying the CustomDialogWindow.
                    songLyrics = customDialogWindow.returnInfo(); // Storing the Song's Lyrics into its variable thanks to the user's input in the CustomDialogWindow.

                    customDialogWindow = new CustomDialogWindow("Write the song's artist. It doesn't matter if it's just a person or a band!",
                      "You wrote an invalid artist name! Please, check the following hints:\n\n- You didn't write anything in the box below.\n- The song's artist exceeds 50 characters.", 50, false, false); // Assigning CustomDialogWindow for the Song's Artist.
                    customDialogWindow.ShowDialog(); // Displaying the CustomDialogWindow.
                    songArtist = customDialogWindow.returnInfo(); // Storing the Song's Artist into its variable thanks to the user's input in the CustomDialogWindow.

                    customDialogWindow = new CustomDialogWindow("Write the song's album. If the song has no album, just write a hyphen (-)!",
                      "You wrote an invalid song's album! Please, check the following hints:\n\n- You didn't write the hyphen (-) in the box below.\n- The song's album exceeds 50 characters.", 50, false, false); // Assigning CustomDialogWindow for the Song's Album.
                    customDialogWindow.ShowDialog(); // Displaying the CustomDialogWindow.
                    songAlbum = customDialogWindow.returnInfo(); // Storing the Song's Album into its variable thanks to the user's input in the CustomDialogWindow.

                    customDialogWindow = new CustomDialogWindow("Write the song's duration. Please, make sure to write the following pattern: 03:29\nDon't forget to write the first 0!",
                      "You wrote an invalid song's duration! Please, check the following hints:\n\n- You didn't write a valid duration like: 03:29.\n- The song's duration exceeds 5 characters.", 5, true, false); // Assigning CustomDialogWindow for the Song's Duration.
                    customDialogWindow.ShowDialog(); // Displaying the CustomDialogWindow.
                    songDuration = customDialogWindow.returnInfo(); // Storing the Song's Duration into its variable thanks to the user's input in the CustomDialogWindow.

                    customDialogWindow = new CustomDialogWindow("Write the song's release year! Please, make sure to write the following pattern: 1994",
                      "You wrote an invalid song's duration! Please, check the following hints:\n\n- You didn't a valid year like: 1993.\n- The song's year is below or above 4 numbers.", 5, false, true); // Assigning CustomDialogWindow for the Song's Year.
                    customDialogWindow.ShowDialog(); // Displaying the CustomDialogWindow.
                    songYear = int.Parse(customDialogWindow.returnInfo()); // Storing the Song's Year into its variable thanks to the user's input in the CustomDialogWindow.

                    queryInsertSong = new MySqlCommand($"INSERT INTO song VALUES('{songTitle}', '{songGenre}', '{songLyrics}', '{songDuration}', '{songArtist}', '{songAlbum}', {songYear}, {Convert.ToByte(songFavourite)});", 
                        connection); // Preparing Insert MySQL statement.
                    queryInsertSong.ExecuteNonQuery(); // Inserting data into the database.

                    connection.Close(); // Closing the MySQL Connection.
                }
                else // There isn't at least one song in the playlist before attempting to save information.
                {
                    MessageBox.Show("You can't save information about a song if there's none in the playlist!", "Error!"); // Shows the exception if some problem happens.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Event handler which activates the displaying of information about a song from the database.
        private void btnShowInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listSongsReproduction.HasItems) // Checks if there's at least one song in the playlist before attempting to retrieve information.
                {
                    connection = new MySqlConnection(DataBaseUtil.MySQLDatabaseConnection); // Initializing the connection's settings.
                    connection.Open(); // Opening said MySQL Connection.

                    MySqlCommand queryCreateDB = new MySqlCommand(DataBaseUtil.MySQLCreateDatabase, connection); // Preparing the command which creates the MySQL database.
                    MySqlCommand queryUseDB = new MySqlCommand(DataBaseUtil.MySQLUseDatabase, connection); // Preparing the command which selects the just created database.
                    MySqlCommand queryCreateTable = new MySqlCommand(DataBaseUtil.MySQLCreateTable, connection); // Preparing the command which creates the MySQL Table which saves the songs' information.
                    MySqlCommand querySongInfo = new MySqlCommand($"SELECT * FROM song WHERE title = '{currentTrack.ToString().Replace("System.Windows.Controls.ListBoxItem: ", "")}';", connection); // Preparing the command which creates the MySQL query.
                    MySqlDataReader querySongInfoReader;  // Class prepared for reading retrieved data.

                    queryCreateDB.ExecuteNonQuery(); // Creating the MySQL Database.
                    queryUseDB.ExecuteNonQuery(); // Selecting the just created database.
                    queryCreateTable.ExecuteNonQuery(); // Preparing the command which creates the MySQL Table which saves the songs' information.
                    querySongInfoReader = querySongInfo.ExecuteReader(); // Assigning the data reader to the query result.

                    if (querySongInfoReader.Read()) // Checks if the reader can read the searched song from the database.
                    {
                        song = new Song(); // Preparing an empty Song object.

                        string bodyInfo =
                            $"Success on retrieving information about the current song!\n\n" +
                            $"- TITLE -\t\t {querySongInfoReader.GetString("title")}\n" +
                            $"- ARTIST -\t {querySongInfoReader.GetString("artist")}\n" +
                            $"- ALBUM -\t {querySongInfoReader.GetString("album")}\n" +
                            $"- YEAR -\t\t {querySongInfoReader.GetString("year").ToString()}\n" +
                            $"- DURATION -\t {querySongInfoReader.GetString("duration")}\n" +
                            $"- GENRE -\t {querySongInfoReader.GetString("genre")}\n" +
                            $"- FAVOURITE? -\t {(querySongInfoReader.GetString("isFavourite").ToString().Equals("1") ? "Yes!" : "No")}\n" +
                            $"- LYRICS -\n\n {querySongInfoReader.GetString("lyrics")}\n\n"; // Preparing the string for the MessageBox with the Song's information.

                        song.Title = querySongInfoReader.GetString("title"); // Assigning the Song's Title.
                        song.Artist = querySongInfoReader.GetString("artist"); // Assigning the Song's Artist.
                        song.Album = querySongInfoReader.GetString("album"); // Assigning the Song's Album.
                        song.Year = int.Parse(querySongInfoReader.GetString("year")); // Assigning the Song's Year.
                        song.Duration = querySongInfoReader.GetString("duration"); // Assigning the Song's Duration.
                        song.Favourites = (querySongInfoReader.GetString("isFavourite").ToString().Equals("1") ? true : false); // Assigning the Song's Favourite Status.
                        song.Lyrics = querySongInfoReader.GetString("lyrics"); // Assigning the Song's Lyrics.

                        switch (querySongInfoReader.GetString("genre").ToLower()) // Depending on what genre string is retrieved, the 'song' variable will be assigned one enum value.
                        {
                            case "lofi":
                                song.Genre = Genre.Lofi;
                                break;

                            case "jazz":
                                song.Genre = Genre.Jazz;
                                break;

                            case "techno":
                                song.Genre = Genre.Techno;
                                break;

                            case "pop":
                                song.Genre = Genre.Pop;
                                break;

                            case "new wave":
                                song.Genre = Genre.NewWave;
                                break;

                            case "other":
                                song.Genre = Genre.Other;
                                break;
                        }
                        MessageBox.Show(bodyInfo, $"{song.Title} - Information"); // Displaying the song's information from the database.
                    }
                    connection.Close(); // Closing the MySQL Connection.
                }
                else // There isn't at least one song in the playlist before attempting to retrieve information.
                {
                    MessageBox.Show("You can't retrieve information about a song if there's not a current one in the playlist!", "Error!"); // Shows the exception if some problem happens.
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        #endregion

        #region Sliders

        #region Song Slider

        // Event handler which activates when the dragging action has started (Song Slider).
        private void sliderTimeLine_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDragging = true;
        }

        // Event handler which activates when the dragging action has ended (Song Slider).
        private void sliderTimeLine_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDragging = false;
            musicPlayer.Position = TimeSpan.FromSeconds(sliderTimeSong.Value); // Sets the current duration value from the song to the slider.
        }

        // Event handler which activates when the slider is being left clicked on a position (Song Slider).
        private void sliderTimeLine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            musicPlayer.Position = TimeSpan.FromSeconds(sliderTimeSong.Value); // Sets the current duration value from the song to the slider.
        }

        #endregion

        #region Volume Slider

        // Event handler which activates when the dragging action has started (Volume Slider).
        private void sliderVolume_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDragging = true;
        }

        // Event handler which activates when the dragging action has ended (Volume Slider).
        private void sliderVolume_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDragging = false;
            musicPlayer.Volume = sliderVolume.Value; // Sets the MediaPlayer Volume to the Slider's value.
        }

        // Event handler which activates when the slider is being left clicked on a position (Volume Slider).
        private void sliderVolume_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            musicPlayer.Volume = sliderVolume.Value; // Sets the MediaPlayer Volume to the Slider's value.
        }

        #endregion

        #endregion // Ends the Slider Region

        #endregion // Ends the Event Handlers Region

        #region Functions and Methods

        // Function which allows the user to select one or many music files in order to be added to the playlist.
        private void openFiles()
        {
            OpenFileDialog fd = new OpenFileDialog(); // OpenFileDialog is the C# Class which allows to select files in a Windows OS.
            fd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); // Sets an initial predefined directory, in this case 'My music'.
            fd.Multiselect = true; // Allows to select multiple files. 
            fd.Title = "Select .mp3, .wav, .wma or .aac audio files"; // Title of the OpenFileDialog.
            fd.Filter = "Media files mp3, wav, wma, aac, wmv (*.mp3),(*.wmv),(*.wav),(*.aac),(*.wma)|*.mp3;*.wav;*.aac;*.wma|Audio - MP3 (*.mp3)|*.mp3|Audio - WAV (*.wav)|*.wav|Audio - AAC (*.aac)|*.aac|Audio - WMA (*.wma)|*.wma"; // Filter of the OpenFileDialog.
            
            try
            {
                Nullable<bool> result = fd.ShowDialog(); // Boolean set with a predefined null value which acquires a True value if the OpenFileDialog is successfully shown to the user.
                if (result == true) // If the OpenFileDialog shows to the user.
                {
                    stop(); // Function which stops the current song.
                    musicPlayer.Source = null; // The MediaPlayer source of songs is set to null.
                    listSongsReproduction.Items.Clear(); // The playlist ListBox Items has now no values in order to insert the new music files.

                    string[] selectedFiles = fd.FileNames; // String Array of the selected music file paths.
                    foreach (string file in selectedFiles) // Looping through each music file path within the array.
                    {
                        ListBoxItem lstItem = new ListBoxItem(); // ListBoxItem is mandatory in order to put the new song in the ListBox playlist.
                        lstItem.Content = System.IO.Path.GetFileNameWithoutExtension(file); // The ListBoxItem sets as its content the music file path without its extension, ready to be added to the playlist.
                        lstItem.Tag = file; // The visual tag displayed to the user at the ListBox playlist is just the name of the file, the path is not shown.
                        listSongsReproduction.Items.Add(lstItem); // Adds the item (song) to the ListBox playlist.
                    }
                    listSongsReproduction.SelectedIndex = 0; // Sets where to start playing in the ListBox playlist, in this case from the beginning.
                    playOrPauseTrack(); // Function which starts playing the first new song of the playlist.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Function which allows the user to play or pause the current song.
        private void playOrPauseTrack()
        {
            try
            {
                if (listSongsReproduction.HasItems) // Checks if there is at least one song in the playlist.
                {
                    if (buttonPlayPause.Content.Equals("▶")) // Checks if the content of the 'Play / Pause' Button is 'Play'.
                    {
                        play(); // Plays the current song.
                    }
                    else if (buttonPlayPause.Content.Equals("⏸")) // Checks if the content of the 'Play / Pause' Button is 'Pause'.
                    {
                        pause(); // Pauses the current song.
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Function which allows the user to play the current song.
        private void play()
        {
            try
            {
                if (!MediaElementIsPaused) // Checks if the MediaPlayer IS NOT paused.
                {
                    if (currentTrack != null) // Checks if the current song IS NOT null.
                    {
                        previousTrack = currentTrack; // Sets as previous song the current song. 
                        previousTrack.Foreground = trackColor; // Changes the previous (now current) song tag color.
                    }
                    currentTrack = (ListBoxItem)listSongsReproduction.SelectedItem; // Sets as current song the selected ListBoxItem.
                    currentTrack.Foreground = currentTrackIndicator; // Changes the current song tag color.

                    musicPlayer.Source = new Uri(currentTrack.Tag.ToString()); // The MediaPlayer gets its source of music files from the current song URI.
                    sliderTimeSong.Value = 0; // Sets the SongSlider to its minimum value.
                }
                musicPlayer.Play(); // Starts playing music.
                MediaElementIsPaused = false; // States that the MediaPlayer IS NOT paused.
                buttonPlayPause.Content = "⏸"; // Sets the 'Play / Pause' Button text to 'Pause' as the app is actually playing music.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Function which allows the user to pause the current song.
        private void pause()
        {
            try
            {
                buttonPlayPause.Content = "▶"; // Sets the 'Play / Pause' Button text to 'Play' as the app is actually playing music.
                musicPlayer.Pause(); // Pauses music.
                MediaElementIsPaused = true; // States that the MediaPlayer IS paused.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Function which allows the user to stop the current song.
        private void stop()
        {
            try
            {
                if (listSongsReproduction.HasItems) // Checks if there is at least one song in the playlist.
                {
                    buttonPlayPause.Content = "▶"; // Sets the 'Play / Pause' Button text to 'Play' as the app is actually stopped playing music.
                    musicPlayer.Stop(); // Stops playing music.
                    sliderTimeSong.Value = 0; // Sets the SongSlider to its minimum value.
                    MediaElementIsPaused = false; // States that the MediaPlayer IS NOT paused.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Function which allows the user to select the next song in the playlist.
        private void moveToNextTrack() 
        {
            try
            {
                if (listSongsReproduction.Items.IndexOf(currentTrack) < listSongsReproduction.Items.Count - 1) // Checks if the current song is not the playlist's final song.
                {
                    listSongsReproduction.SelectedIndex = listSongsReproduction.Items.IndexOf(currentTrack) + 1; // States the selected index from the ListBox playlist as the next song.
                    MediaElementIsPaused = false; // States that the MediaPlayer IS NOT paused.
                    play(); // Plays the next song (the current one now).
                }
                else if (listSongsReproduction.Items.IndexOf(currentTrack) == listSongsReproduction.Items.Count - 1) // Checks if the current song is the playlist's final song.
                {
                    listSongsReproduction.SelectedIndex = 0; // Entering this 'else if' means that the final song in the playlist cannot have a new next song, so we set the 'next' one as the first song in the playlist.
                    MediaElementIsPaused = false; // States that the MediaPlayer IS NOT paused.
                    play(); // Plays the next song (the current one now).
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        // Function which allows the user to select the previous song in the playlist.
        private void moveToPreviousTrack()
        {
            try
            {
                if (listSongsReproduction.Items.IndexOf(currentTrack) > 0) // Checks if the current song IS NOT the first one of the playlist.
                {
                    listSongsReproduction.SelectedIndex = listSongsReproduction.Items.IndexOf(currentTrack) - 1; // States the selected index from the ListBox playlist as the previous song.
                    MediaElementIsPaused = false; // States that the MediaPlayer IS NOT paused.
                    play(); // Plays the next song (the current one now).
                }
                else if (listSongsReproduction.Items.IndexOf(currentTrack) == 0) // Checks if the current song IS the first one of the playlist.
                {
                    listSongsReproduction.SelectedIndex = listSongsReproduction.Items.Count - 1; // Entering this 'else if' means that the first song in the playlist cannot have a new previous song, so we set the 'previous' one as the last song in the playlist.
                    MediaElementIsPaused = false; // States that the MediaPlayer IS NOT paused.
                    play(); // Plays the next song (the current one now).
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Shows the exception if some problem happens.
            }
        }

        #endregion

    }
}