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

namespace RubyBingoApp.MVVM.View.DialogBoxes
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {

        public string InputString { get; private set; }
        private bool OnlyNumbers { get; set; }

        public InputDialog(string watermark, string title, bool onlyNumbers)
        {
            InitializeComponent();

            InputTextBox.GotFocus += RemoveWatermark;
            InputTextBox.LostFocus += ShowWatermark;

            this.WatermarkTextBlock.Text = watermark;
            this.TitleLabel.Content = title;

            OnlyNumbers = onlyNumbers;
        }

        private void Click_Enter(object sender, MouseButtonEventArgs e)
        {
            string InputName = this.InputTextBox.Text;
            if (!OnlyNumbers)
            {
                if (InputName == "")
                {
                    this.MessageTextBlock.Text = "Oy bruv, you gotta put at least one letter init, can't leave it blank, fam.";
                }
                else
                {
                    this.InputString = InputName;
                    this.Close();
                }   
            }
            else
            {
                bool isValid = true;
                foreach (char c in InputName)
                {
                    if (!char.IsDigit(c))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (!isValid)
                {
                    this.MessageTextBlock.Text = "Yo, just drop the numbers, fam. No cap, keep it digits only, aight? We’re not tryna see any weird stuff in here 🤓";
                }
                else
                {
                    this.InputString = InputName;
                    this.Close();
                }
            }
        }

        private void Click_Cancel(object sender, MouseButtonEventArgs e)
        {
            this.InputString = "";
            this.Close();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new System.Windows.Thickness(8);
            }
            else
            {
                this.BorderThickness = new System.Windows.Thickness(0);
            }
        }

        private void RemoveWatermark(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MessageTextBlock.Text))
            {
                WatermarkTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        // Event when the TextBox loses focus
        private void ShowWatermark(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MessageTextBlock.Text))
            {
                WatermarkTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.InputString = "";
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    // Get the mouse position relative to the entire screen
                    var mousePosition = PointToScreen(e.GetPosition(this));

                    // Get the current screen's DPI scaling factor
                    var source = PresentationSource.FromVisual(this);
                    double dpiFactorX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
                    double dpiFactorY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

                    // Adjust mouse position for DPI scaling
                    double scaledMouseX = mousePosition.X / dpiFactorX;
                    double scaledMouseY = mousePosition.Y / dpiFactorY;

                    // Calculate horizontal ratio
                    double horizontalRatio = e.GetPosition(this).X / this.ActualWidth;

                    // Restore window to normal size
                    this.WindowState = WindowState.Normal;

                    // Reposition the window so the cursor stays aligned correctly
                    this.Left = scaledMouseX - (this.Width * horizontalRatio);
                    this.Top = scaledMouseY - 10; // Small adjustment to align with the top of the screen
                }

                // Allow the window to be dragged
                this.DragMove();
            }
        }
    }
}
