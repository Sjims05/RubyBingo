using System;
using System.IO;
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

namespace RubyBingoApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for StatsView.xaml
    /// </summary>
    public partial class StatsView : UserControl
    {
        public StatsView()
        {
            InitializeComponent();
            VisualizeData();
        }

        public void VisualizeData()
        {
            var TotalGamesPlayed =  this.TGP;
            var TotalGameTime =     this.TGT;
            var AverageGameTime =   this.AGT;
            var AverageHEG =        this.AHEG;
            var MostHEG =           this.MHEG;
            var LeastHEG =          this.LHEG;

            (int TGP, int TGT, int AHEG, int MHEG, int LHEG) = GetStoredData();
            (int SavedTGP, int SavedTGT, List<int> SavedHEG) = GetSavedData();

            Console.WriteLine(TGP);
            Console.WriteLine(TGT);
            Console.WriteLine(AHEG);
            Console.WriteLine(MHEG);
            Console.WriteLine(LHEG);
            Console.WriteLine("");
            Console.WriteLine(SavedTGP);
            Console.WriteLine(SavedTGT);
            Console.WriteLine(string.Join(", ", SavedHEG));

            TotalGamesPlayed.Content = (TGP + SavedTGP).ToString();
            TotalGameTime.Content = (Math.Round((SavedTGT / 60f), 2)).ToString() + " min";
            AverageGameTime.Content = (Math.Round(((TGP + SavedTGP) / 60f), 2)).ToString() + " min";

            if (SavedHEG.Any())
            {
                AverageHEG.Content = Math.Round(SavedHEG.Average(), 2);
                MostHEG.Content = SavedHEG.Max();
                LeastHEG.Content = SavedHEG.Min();
            }
            else
            {
                AverageHEG.Content = 0; // Or some other default value
                MostHEG.Content = 0;
                LeastHEG.Content = 0;
            }
        }

        public (int TGP, int TGT, List<int> HEG) GetSavedData()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SavedBoards");
            if (!Directory.Exists(folderPath))
                return (0, 0, new List<int>());

            int totalGamesPlayed = 0;
            int totalGameTimeInSeconds = 0;
            List<int> hitsEveryGame = new List<int>();

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

                    // Count numbers <= 1 in line 5 for HEG
                    string[] numbers = lines[4].Split(',');
                    int hitsCount = numbers.Count(n => int.TryParse(n, out int value) && value <= 1);
                    hitsEveryGame.Add(hitsCount);
                }
            }

            return (totalGamesPlayed, totalGameTimeInSeconds, hitsEveryGame);
        }

        public (int TGP, int TGT, int AHEG, int MHEG, int LHEG) GetStoredData()
        {
            // Define the file path
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Stats.txt");

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // Return default values if the file doesn't exist
                return (0, 0, 0, 0, 0);
            }

            int totalGamesPlayed = 0;
            int totalGameTimeInSeconds = 0;
            int averageHEG = 0;
            int mostHEG = 0;
            int leastHEG = 0;

            // Read all lines from the file
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                // Split the line into key and value parts
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // Parse the values based on the key
                    switch (key)
                    {
                        case "Total Games Played":
                            int.TryParse(value, out totalGamesPlayed);
                            break;

                        case "Total Game Time":
                            int.TryParse(value, out totalGameTimeInSeconds);
                            break;

                        case "Average HEG":
                            int.TryParse(value, out averageHEG);
                            break;

                        case "Most HEG":
                            int.TryParse(value, out mostHEG);
                            break;

                        case "Least HEG":
                            int.TryParse(value, out leastHEG);
                            break;
                    }
                }
            }

            return (totalGamesPlayed, totalGameTimeInSeconds, averageHEG, mostHEG, leastHEG);
        }

    }
}
