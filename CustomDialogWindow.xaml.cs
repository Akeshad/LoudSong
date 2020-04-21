using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    // CustomDialogWindow represents the user's inputs window. This class contains private fields and event handlers which manages all the user's inputs. Extends from WPF Window.
    public partial class CustomDialogWindow : Window
    {

        #region Private Fields

        private Regex regexDuration; // RegularExpression for comparing the user's input for the Song's Duration.
        private Regex regexYear; // RegularExpression for comparing the user's input for the Song's Year.

        private string labelText; // Represents the message to be displayed in the custom Dialog.
        private string info; // Represents the valid information got by the user's input.
        private string errorText; // Represents the message to be displayed if an error happened.
        private int caracters; // Represents the input character's limit.
        private bool isDuration; // States if this is TRUE if the Dialog is for the Song's Duration or FALSE if it IS NOT.
        private bool isYear; // States if this is TRUE if the Dialog is for the Song's Year or FALSE if it IS NOT.

        #endregion

        #region Constructor

        // Constructor for the CustomDialogWindow. It is called at MainWindow.xaml.cs.
        public CustomDialogWindow(string labelText, string errorText, int caracters, bool isDuration, bool isYear)
        {
            InitializeComponent(); // Initializes the mandatory components to make the Window work.
            this.labelText = labelText;
            this.errorText = errorText;
            this.caracters = caracters;
            this.isDuration = isDuration;
            this.isYear = isYear;
            lblCustomDialog.Content = labelText; // The CustomWindow's Label shows the text which represents instructions for the user.
        }

        #endregion

        #region Event Handlers

        // Event handler which activates when the user presses on the 'OK' Button. Checks all the input data, stores it if it's valid and closes the CustomWindow itself.
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (isDuration) // Checks if the user's input IS related to the Song's Duration.
            {
                regexDuration = new Regex(@"\d{2}:\d{2}"); // Regular Expression set!
                if (txtBoxCustomDialog.Text == null || !regexDuration.IsMatch(txtBoxCustomDialog.Text)) // Checks if the user's input for duration is empty, above the character limit and if the input follows a '03:29' pattern.
                {
                    MessageBox.Show(errorText, "Error!"); // The input is not correct, so this MessageBox informs about the error.
                }
                else
                {
                    info = txtBoxCustomDialog.Text; // Stores the valid input as the user's information about the song.
                    this.Close(); // Closes the CustomDialogWindow.
                }
            }
            else if (isYear) // Checks if the user's input IS related to the Song's Year.
            {
                regexYear = new Regex(@"\d{4}"); // Regular Expression set!
                if (txtBoxCustomDialog.Text == null || !regexYear.IsMatch(txtBoxCustomDialog.Text)) // Checks if the user's input for duration is empty, above the character limit and if the input follows a '03:29' pattern.
                {
                    MessageBox.Show(errorText, "Error!"); // The input is not correct, so this MessageBox informs about the error.
                }
                else
                {
                    info = txtBoxCustomDialog.Text; // Stores the valid input as the user's information about the song.
                    this.Close(); // Closes the CustomDialogWindow.
                }
            }
            else // Checks if the user's input IS NOT related to the Song's Year or Duration.
            {
                if (txtBoxCustomDialog.Text == null || txtBoxCustomDialog.Text.Length == 0 || txtBoxCustomDialog.Text.Length >= caracters) // Checks if the user's input is empty and within the character limit.
                {
                    MessageBox.Show(errorText, "Error!"); // The input is not correct, so this MessageBox informs about the error.
                }
                else
                {
                    info = txtBoxCustomDialog.Text; // Stores the valid input as the user's information about the song.
                    this.Close(); // Closes the CustomDialogWindow.
                }
            }
        }

        // Closes the CustomDialogWindow.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the CustomDialogWindow.
        }

        #endregion

        #region Functions and Methods

        // Gets the user's input back as a String.
        public string returnInfo()
        {
            return this.info;
        }

        #endregion

    }
}
