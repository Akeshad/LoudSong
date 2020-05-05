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

namespace LoudSong
{
    // This class represents the Toggle Button for the Favourite setting at the main window. Extends from UserControl.
    public partial class TobleButton : UserControl
    {

        #region Private Fields

        private SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(160, 160, 160)); // Color when the toggle option is 'Off'.  
        private SolidColorBrush On = new SolidColorBrush(Color.FromRgb(130, 190, 125)); // Color when the toggle option is 'On'. 

        private Thickness LeftSide = new Thickness(-39, 0, 0, 0); // Thickness of the toggle option when it's at the left side.
        private Thickness RightSide = new Thickness(0, 0, -39, 0); // Thickness of the toggle option when it's at the right side.

        private bool Toggled = false; // Status of the toggle.

        #endregion

        #region Constructor

        // Constructor for the Toggle Button.
        public TobleButton()
        {
            InitializeComponent();
            Back.Fill = Off; // Sets the 'Off' color.
            Toggled = false; // Not toggled.
            Dot.Margin = LeftSide; // Margin at left.
        }

        #endregion

        #region Properties

        public bool Toggled1 { get => Toggled; set => Toggled = value; } // Property to be called for the status from the MainWindow.

        #endregion

        #region Event Handlers

        // Event handler which activates when the toggle is being left clicked in order to toggle it 'On' or 'Off'.
        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Toggled) // Not toggled.
            {
                Back.Fill = On; // Sets the 'On' color.
                Toggled = true; // Toggled.
                Dot.Margin = RightSide; // Margin at right.
            }
            else // Toggled.
            {
                Back.Fill = Off; // Sets the 'Off' color.
                Toggled = false; // Not toggled.
                Dot.Margin = LeftSide; // Margin at left.
            }
        }

        #endregion

    }
}
