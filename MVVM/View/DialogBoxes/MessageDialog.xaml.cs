﻿using System;
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
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        public MessageDialog(string message, string title)
        {
            InitializeComponent();
            this.MessageTextBlock.Text = message;
            this.TitleLabel.Content = title;
        }

        private void Click_Ok(object sender, MouseButtonEventArgs e)
        {
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

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("downgirtngie");
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Handle dragging when maximized
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