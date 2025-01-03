using RubyBingoApp.MVVM.View.DialogBoxes;
using RubyBingoApp.MVVM.ViewModel;
using RubyBingoApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RubyBingoApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {

        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public HomeView()
        {
            InitializeComponent();
            GetMainStats();
        }

        private void GetMainStats()
        {
            var TotalGamesPlayed = this.Home_TGP;
            var TotalGameTime = this.Home_TGT;

            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SavedBoards");
            if (!Directory.Exists(folderPath))
                return;

            int totalGamesPlayed = 0;
            int totalGameTimeInSeconds = 0;

            string[] files = Directory.GetFiles(folderPath, "*.txt");

            foreach (string file in files)
            {
                totalGamesPlayed++; // Increment the number of games (TGP)

                string[] lines = File.ReadAllLines(file);

                // Ensure file has sufficient lines to avoid index issues
                if (lines.Length >= 5)
                {
                    // Parse the total game time (TGT) in seconds
                    string gameTime = lines[2]; // "00:00:04"
                    string[] timeParts = gameTime.Split(':');
                    if (timeParts.Length == 3)
                    {
                        int hours = int.Parse(timeParts[0]);
                        int minutes = int.Parse(timeParts[1]);
                        int seconds = int.Parse(timeParts[2]);
                        totalGameTimeInSeconds += hours * 3600 + minutes * 60 + seconds;
                    }
                }
            }

            TotalGamesPlayed.Content = totalGamesPlayed.ToString();
            TotalGameTime.Content = (Math.Round((totalGameTimeInSeconds / 60f), 2)).ToString() + " min";
        }

        private void NewBoard_Click(object sender, MouseButtonEventArgs e)
        {
            MakeWindowSmall();
            if (this.DataContext is HomeViewModel homeViewModel)
            {
                // Execute the command to navigate to the new board view
                homeViewModel.NavigateToNewBoardViewCommand.Execute(null);
            }
        }
        private void LastBoard_Click(object sender, MouseButtonEventArgs e)
        {
            MakeWindowSmall();
            //TODO Remove "..", "..", Or save it in appdata or documents
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Temp");
            string filePath = Path.Combine(folderPath, "_auto_.txt");

            if (Directory.Exists(folderPath))
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    lines[5] = "1";
                    File.WriteAllLines(filePath, lines);
                }
            }

            if (this.DataContext is HomeViewModel homeViewModel)
            {
                // Execute the command to navigate to the new board view
                homeViewModel.NavigateToNewBoardViewCommand.Execute(null);
            }
        }

        private void CustomBoard_Click(object sender, MouseButtonEventArgs e)
        {
            InputDialog inputDialog = new InputDialog("Enter Custom id", "Custom id", true);
            inputDialog.ShowDialog();
            if (inputDialog.InputString == "")
                return;

            MakeWindowSmall();
            //TODO Remove "..", "..", Or save it in appdata or documents
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Temp");
            string filePath = Path.Combine(folderPath, "_auto_.txt");

            if (Directory.Exists(folderPath))
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    lines[5] = $"{inputDialog.InputString}";
                    File.WriteAllLines(filePath, lines);
                }
            }

            if (this.DataContext is HomeViewModel homeViewModel)
            {
                // Execute the command to navigate to the new board view
                homeViewModel.NavigateToNewBoardViewCommand.Execute(null);
            }
        }

        private void MakeWindowSmall()
        {
            mainWindow.Height = 675;
            mainWindow.Width = 675;
            Grid mainGrid = mainWindow.MainGrid;
            mainGrid.ColumnDefinitions[0].Width = new GridLength(0);
        }
    }
}
