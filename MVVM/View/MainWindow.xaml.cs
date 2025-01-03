using RubyBingoApp.Core;
using RubyBingoApp.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Timers;
using System.Windows.Media.Animation;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace RubyBingoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private System.Timers.Timer countdownTimer;
        private int remainingTimeInSeconds;
        private int currentBingoId { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            countdownTimer = new System.Timers.Timer(1000);
            countdownTimer.Elapsed += CountdownTimer_Elapsed;

            GetTempId();
            StatsInit();
        }

        public void StatsInit()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Stats.txt");
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Total Games Played =0");
                    writer.WriteLine("Win Rate =0");
                    writer.WriteLine("Total Game Time =0");
                    writer.WriteLine("Average Game Time =0");

                    writer.WriteLine("Average HBB =0");
                    writer.WriteLine("Most HBB =0");
                    writer.WriteLine("Least HBB =0");

                    writer.WriteLine("Average HEG =0");
                    writer.WriteLine("Most HEG =0");
                    writer.WriteLine("Least HEG =0");
                }
            }
        }

        public void SetBingoId(int value)
        {
            currentBingoId = value;
            SetTempId();

            this.Title = "Ruby Bingo Pro - " + value.ToString();
            remainingTimeInSeconds = 20 * 60;
            countdownTimer.Start();
        }

        public int GetBingoId()
        {
            return currentBingoId;
        }

        public void SetRemainingTime(int sec)
        {
            remainingTimeInSeconds = sec;
        }
        public int GetRemainingTime()
        {
            return remainingTimeInSeconds;
        }

        private void CountdownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (remainingTimeInSeconds > 0)
            {
                remainingTimeInSeconds--;
                Dispatcher.Invoke(() =>
                {
                    this.Title = "Ruby Bingo Pro - " + GetBingoId().ToString() + " " + remainingTimeInSeconds.ToString();
                });
            }
            else
            {
                countdownTimer.Stop();
                Dispatcher.Invoke(() =>
                {
                    this.Title = "Ruby Bingo Pro";
                });
            }
        }

        private void SetTempId()
        {
            string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Data", "Temp"); // For testing
            string filePath = Path.Combine(TempFolder, "BingoId.txt");
            filePath = Path.GetFullPath(filePath);

            string timeSaved = DateTime.Now.ToString("HH-mm");
            string daySaved = DateTime.Now.DayOfYear.ToString();
            string yearSaved = DateTime.Now.Year.ToString();

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(TempFolder)))
                    Directory.CreateDirectory(Path.GetDirectoryName(TempFolder));

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine($"{currentBingoId}");
                    writer.WriteLine($"{timeSaved}");
                    writer.WriteLine($"{daySaved}");
                    writer.WriteLine($"{yearSaved}");
                }
            } catch { return; }
        }

        private void GetTempId()
        {
            string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Data", "Temp"); // For testing
            string filePath = Path.Combine(TempFolder, "BingoId.txt");
            filePath = Path.GetFullPath(filePath);

            string timeNow = DateTime.Now.ToString("HH-mm");
            int dayNow = DateTime.Now.DayOfYear;
            int yearNow = DateTime.Now.Year;

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(TempFolder)))
                    return;

                if (!File.Exists(filePath))
                    return;

                List<string> contentsList = new List<string>();
                contentsList.AddRange(File.ReadAllLines(filePath, Encoding.UTF7));

                if (dayNow != int.Parse(contentsList[2]))
                    return;

                if (yearNow != int.Parse(contentsList[3]))
                    return;

                Console.WriteLine(dayNow == int.Parse(contentsList[2]));
                Console.WriteLine(yearNow == int.Parse(contentsList[3]));

                var splitTimeNow = timeNow.Split('-');
                var splitTimeThen = contentsList[1].Split('-');

                int timeNowValue = int.Parse(splitTimeNow[0]) * 60 + int.Parse(splitTimeNow[1]);
                int timeThenValue = int.Parse(splitTimeThen[0]) * 60 + int.Parse(splitTimeThen[1]);

                if (timeNowValue - 20 < timeThenValue)
                {
                    remainingTimeInSeconds = 20 * 60 - 60 * (timeNowValue - timeThenValue);
                    currentBingoId = int.Parse(contentsList[0]);
                    countdownTimer.Start();
                }
                else return;

            }
            catch { return; }
        }

        private void Hotbar_Home(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is MainViewModel mainViewModel)
            {
                // Execute the command to navigate to the new board view
                mainViewModel.NavigateToHomeViewCommand.Execute(null);
            }
        }

        private void Hotbar_SavedBoards(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is MainViewModel mainViewModel)
            {
                // Execute the command to navigate to the new board view
                mainViewModel.NavigateToSavedBoardViewCommand.Execute(null);
            }
        }

        private void Hotbar_Stats(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is MainViewModel mainViewModel)
            {
                // Execute the command to navigate to the new board view
                mainViewModel.NavigateToStatsViewCommand.Execute(null);
            }
        }

        private void Hotbar_Styling(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is MainViewModel mainViewModel)
            {
                // Execute the command to navigate to the new board view
                mainViewModel.NavigateToStylingViewCommand.Execute(null);
            }
        }

        private void Hotbar_Settings(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is MainViewModel mainViewModel)
            {
                // Execute the command to navigate to the new board view
                mainViewModel.NavigateToSettingsViewCommand.Execute(null);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mainWindow = Application.Current.MainWindow;

                // Handle dragging when maximized
                if (mainWindow.WindowState == WindowState.Maximized)
                {
                    // Get the mouse position relative to the entire screen
                    var mousePosition = PointToScreen(e.GetPosition(this));

                    // Get the current screen's DPI scaling factor
                    var source = PresentationSource.FromVisual(mainWindow);
                    double dpiFactorX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
                    double dpiFactorY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

                    // Adjust mouse position for DPI scaling
                    double scaledMouseX = mousePosition.X / dpiFactorX;
                    double scaledMouseY = mousePosition.Y / dpiFactorY;

                    // Calculate horizontal ratio
                    double horizontalRatio = e.GetPosition(mainWindow).X / mainWindow.ActualWidth;

                    // Restore window to normal size
                    mainWindow.WindowState = WindowState.Normal;

                    // Reposition the window so the cursor stays aligned correctly
                    mainWindow.Left = scaledMouseX - (mainWindow.Width * horizontalRatio);
                    mainWindow.Top = scaledMouseY - 10; // Small adjustment to align with the top of the screen
                }

                // Allow the window to be dragged
                mainWindow.DragMove();
            }
        }


        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized) {
                WindowState = WindowState.Maximized;
            } else {
                WindowState = WindowState.Normal;
            }
            
        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
    }
}
