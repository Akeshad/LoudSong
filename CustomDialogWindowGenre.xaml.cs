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
using System.Windows.Shapes;

namespace LoudSong
{
    // CustomDialogWindow represents the user's input window for selecting a Song's Gender. Extends from WPF Window.
    public partial class CustomDialogWindowGenre : Window
    {

        #region Fields

        public static string genreString; // Enum transformed to String for storing later in the database.

        #endregion

        #region Constructor

        // Constructor for the CustomDialogWindow. It is called at MainWindow.xaml.cs.
        public CustomDialogWindowGenre()
        {
            InitializeComponent(); // Initializes the mandatory components to make the Window work.
            cmbBoxGenre.Items.Add(Genre.Jazz.ToString()); // Adds to the ComboBox the Genre Enum 'Jazz'.
            cmbBoxGenre.Items.Add(Genre.Lofi.ToString()); // Adds to the ComboBox the Genre Enum 'Lofi'.
            cmbBoxGenre.Items.Add(Genre.NewWave.ToString()); // Adds to the ComboBox the Genre Enum 'NewWave'.
            cmbBoxGenre.Items.Add(Genre.Pop.ToString()); // Adds to the ComboBox the Genre Enum 'Pop'.
            cmbBoxGenre.Items.Add(Genre.Techno.ToString()); // Adds to the ComboBox the Genre Enum 'Techno'.
            cmbBoxGenre.Items.Add(Genre.Other.ToString()); // Adds to the ComboBox the Genre Enum 'Other'.
        }

        #endregion

        #region Event Handlers

        // Event handler which activates when the user presses on the 'OK' Button. Converts the Enum to String for storing later in the database.
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBoxGenre.SelectedIndex >= 0 && cmbBoxGenre.SelectedIndex <= cmbBoxGenre.Items.Count) // Checks if the selected option in the ComboBox is within its available options.
            {
                genreString = cmbBoxGenre.SelectedItem.ToString(); // Stores the selected Item from the ComboBox as a String.
                this.Close(); // Closes the CustomDialogWindow.

            }
            else
            {
                MessageBox.Show("Chosen option is invalid! Please, try again.", "Error!"); // Shown if something wrong happens.
            }
        }

        // Closes the CustomDialogWindow.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the CustomDialogWindow.
        }

        // Event handler which allows the user to move the window while left clicking and dragging on a invisible TextBlock situated at the top border of the window.
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // Allows to move the window.
        }

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

    }
}