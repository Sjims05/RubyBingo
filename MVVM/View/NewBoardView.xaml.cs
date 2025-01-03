using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Threading;
using RubyBingoApp.MVVM.ViewModel;
using System.Windows.Media;
using RubyBingoApp.MVVM.View.DialogBoxes;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data.Common;
using System.Windows.Markup;
using System.Linq;

namespace RubyBingoApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for NewBoardView.xaml
    /// </summary>
    public partial class NewBoardView : UserControl
    {
        private MainWindow mainWindow = (MainWindow) Application.Current.MainWindow;
        private double maxSize;

        private DispatcherTimer _timer;
        private DateTime _startTime;

        int PreviousTime = 0;
        int PreviousId = 0;

        public NewBoardView()
        {
            InitializeComponent();
            UpdateMaxSize();

            StartRealTimeClock();
            StartTimer();
            SetAutoSaveTimer();

            TileGridInit();
            CheckBoardState();
            RegenBoardHitPieces();

            mainWindow.UpdateLayout();
        }

        private void TileGridInit()
        {
            int thing = 0;
            for (int row = 1; row <= 5; row++)
            {
                for (int col = 1; col <= 5; col++)
                {
                    Border tile = (Border)BoardGrid.Children[(col + 5 * (row - 1)) - 1];

                    string tagData = tile.Tag.ToString();
                    var parts = tagData.Split(';');
                    var position = parts[0].Split(',');

                    int tileCol = int.Parse(position[0]);
                    int tileRow = int.Parse(position[1]);
                    string tileState = parts[1];

                    tile.Tag =
                        new TileData
                        {
                            Column = tileCol,
                            Row = tileRow,
                            State = tileState
                        };
                    thing++;
                }
            }
        }

        private class TileData
        {
            public int Column { get; set; }
            public int Row { get; set; }
            public string State { get; set; }
        }

        private void Click_HitTile(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border border;
            if (sender is TextBox clickedTextBox) 
                border = (Border)clickedTextBox.Parent;
            else border = (Border)sender;

            var tileData = (TileData)border.Tag;
            int column = tileData.Column;
            int row = tileData.Row;
            string state = tileData.State;

            string newState;
            if (state == "Normal") 
                newState = "Toggled";
            else 
                newState = "Normal";

            border.Tag = new TileData 
            {
                Column = column,
                Row = row,
                State = newState
            };

            UpdateTileColors(column, row);
        }

        private void UpdateTileColors(int mainCol, int mainRow)
        {
            if (mainCol == mainRow) // Cross
            {
                for (int i = 1; i <= 5; i++)
                    if (mainCol != i && mainRow != i) UpdateTile(i, i);
            }
            if (mainCol == 6 - mainRow) // Reverse Cross
            {
                for (int i = 1; i <= 5; i++) 
                    if (mainCol != i && mainRow != 6 - i) UpdateTile(i, 6 - i);
            }

            for (int i = 1; i <= 5; i++) 
            {
                if (i != mainRow) UpdateTile(mainCol, i); // Horizontal
                if (i != mainCol) UpdateTile(i, mainRow); // Vertical
            }
            
            UpdateTile(mainCol, mainRow); // mainTile update
        }

        private void UpdateTile(int col, int row)
        {
            SolidColorBrush HitBlue = (SolidColorBrush)FindResource("Hit-Blue");
            SolidColorBrush HitYellow = (SolidColorBrush)FindResource("Hit-Yellow");
            SolidColorBrush HitWhite = (SolidColorBrush)FindResource("Hit-White");
            SolidColorBrush HitGrey = (SolidColorBrush)FindResource("Hit-Grey");


            var tileGridList = BoardGrid.Children;

            int amountToggledCross = 0;
            if (col == row) // Cross
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (col == i && row == i) continue;

                    Border subTile = (Border)tileGridList[(i + 5 * (i - 1)) - 1];
                    var subTileData = (TileData)subTile.Tag;

                    if (subTileData.State == "Toggled") amountToggledCross++;
                }
            }

            int amountToggledReverseCross = 0;
            if (col == 6 - row) // Reverse Cross
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (col == i && row == 6 - i) continue;

                    Border subTile = (Border)tileGridList[(i + 5 * ((6 - i) - 1)) - 1];
                    var subTileData = (TileData)subTile.Tag;

                    if (subTileData.State == "Toggled") amountToggledReverseCross++;
                }
            }

            int amountToggledHorizontal = 0;
            for (int i = 1; i <= 5; i++) // Horizontal
            {
                if (row == i) continue;

                Border subTile = (Border)tileGridList[(col + 5 * (i - 1)) - 1];
                var subTileData = (TileData)subTile.Tag;

                if (subTileData.State == "Toggled") amountToggledHorizontal++;
            }

            int amountToggledVertical = 0;
            for (int i = 1; i <= 5; i++) // Vertical
            {
                if (col == i) continue;

                Border subTile = (Border)tileGridList[(i + 5 * (row - 1)) - 1];
                var subTileData = (TileData)subTile.Tag;

                if (subTileData.State == "Toggled") amountToggledVertical++;
            }

            int[] amounts = { amountToggledCross, amountToggledReverseCross, amountToggledHorizontal, amountToggledVertical };

            Border tile = (Border)tileGridList[(col + 5 * (row - 1)) - 1];
            var tileData = (TileData)tile.Tag;

            if (tileData.State == "Toggled")
            {
                if (amounts.Contains(4) || amounts.Contains(3))
                    tile.Background = HitYellow;
                else
                    tile.Background = HitBlue;
            }
            else
            {
                if (amounts.Contains(4) || amounts.Contains(3))
                    tile.Background = HitWhite;
                else
                    tile.Background = HitGrey;
            }

        }

        private void HomeButton(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mainWindow.Height = 650;
            mainWindow.Width = 1200;
            Grid mainGrid = mainWindow.MainGrid;
            mainGrid.ColumnDefinitions[0].Width = new GridLength(250);
            mainWindow.Topmost = false;

            if (this.DataContext is NewBoardViewModel newBoardViewModel)
            {
                newBoardViewModel.NavigateToHomeViewCommand.Execute(null);
            }
        }

        private (List<int> savedBoard, int savedId, string savedDate, string savedTime, string savedDateTime) PrepareSaveData()
        {
            List<int> savedBoard = new List<int>();
            int savedId = mainWindow.GetBingoId();
            string savedDate = _startTime.ToString("dd-MM-yyyy");

            TimeSpan elapsed = DateTime.Now - _startTime;
            int hours = (int)elapsed.Hours;
            int minutes = (int)elapsed.Minutes;
            int seconds = elapsed.Seconds;

            string savedTime = $"{hours:00}:{minutes:00}:{seconds:00}";
            string savedDateTime = DateTime.Now.ToString("HH-mm");

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (UIElement child in BoardGrid.Children)
                {
                    if (child is Border border && border.Background is SolidColorBrush solidColorBrush)
                    {
                        Color color = solidColorBrush.Color;

                        int colorNumber = color == ((SolidColorBrush)FindResource("Hit-Yellow")).Color ? 0 :  // Hit-Yellow
                                          color == ((SolidColorBrush)FindResource("Hit-Blue")).Color ? 1 :  // Hit-Blue
                                          color == ((SolidColorBrush)FindResource("Hit-White")).Color ? 2 :  // White
                                          3; // Default Grey

                        savedBoard.Add(colorNumber);
                    }
                }
            });

            return (savedBoard, savedId, savedDate, savedTime, savedDateTime);
        }


        private void SaveToFile(string filePath, List<int> savedBoard, int savedId, string savedDate, string savedTime, string savedDateTime)
        {
            try
            {
                string folderPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                if (File.Exists(filePath) && !filePath.Contains("_auto_"))
                {
                    ConfirmDialog confirmDialog = new ConfirmDialog("File already exists. Do you want to overwrite it?", "File Exists");
                    confirmDialog.ShowDialog();

                    if (!confirmDialog.IsConfirmed)
                        return;
                }

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine($"{savedId}");
                    writer.WriteLine($"{savedDate}");
                    writer.WriteLine($"{savedTime}");
                    writer.WriteLine($"{savedDateTime}");
                    writer.WriteLine($"{string.Join(",", savedBoard)}");
                    writer.WriteLine("0");
                }

                if (!filePath.Contains("_auto_"))
                {
                    MessageDialog messageDialog = new MessageDialog("Board saved successfully", "Save Successful");
                    messageDialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog($"An error occurred while saving the board: {ex.Message}", "Save Error");
                messageDialog.ShowDialog();
            }
        }

        private void SaveBoard(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var (savedBoard, savedId, savedDate, savedTime, savedDateTime) = PrepareSaveData();
            string[] savingNameDate = savedDate.Split('-');
            int sum = int.Parse(savingNameDate[0]) + 31 * int.Parse(savingNameDate[1]) + 400 * int.Parse(savingNameDate[2]);
            string newSavingNameDate = sum.ToString();

            //TODO Remove "..", "..", Or save it in appdata or documents
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SavedBoards");
            string filePath = Path.Combine(folderPath, $"{newSavingNameDate}-{savedId}.txt");

            SaveToFile(filePath, savedBoard, savedId, savedDate, savedTime, savedDateTime);
        }

        private void AutoSave()
        {
            var (savedBoard, savedId, savedDate, savedTime, savedDateTime) = PrepareSaveData();

            //TODO Remove "..", "..", Or save it in appdata or documents
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Temp");
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filePath = Path.Combine(folderPath, "_auto_.txt");

            SaveToFile(filePath, savedBoard, savedId, savedDate, savedTime, savedDateTime);
        }

        private void SetAutoSaveTimer()
        {
            System.Timers.Timer autoSaveTimer = new System.Timers.Timer(1*60000);
            autoSaveTimer.Elapsed += AutoSaveTimerElapsed;
            autoSaveTimer.AutoReset = true;
            autoSaveTimer.Start();
        }

        private void AutoSaveTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            AutoSave();
        }

        public void CheckBoardState()
        {
            //TODO Remove "..", "..", Or save it in appdata or documents
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Temp");
            string filePath = Path.Combine(folderPath, "_auto_.txt");

            if (Directory.Exists(folderPath))
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    if (lines.Length > 5 && lines[5] == "1")
                    {
                        string[] savedStringBoard = lines[4].Split(',');
                        List<int> savedBoard = savedStringBoard.Select(int.Parse).ToList();
                        LoadAutoSave(savedBoard, int.Parse(lines[0]), lines[1], lines[2]);
                    }
                    else if (lines.Length > 5 && lines[5] == "0")
                    {
                        return;
                    }
                    else
                    {
                        PreviousTime = mainWindow.GetRemainingTime();
                        PreviousId = mainWindow.GetBingoId();

                        mainWindow.SetBingoId(int.Parse(lines[5]));
                    }


                    lines[5] = "0";
                    File.WriteAllLines(filePath, lines);
                }
            }
        }

        private void LoadAutoSave(List<int> savedBoard, int savedId, string savedDate, string savedTime)
        {
            PreviousTime = mainWindow.GetRemainingTime();
            PreviousId = mainWindow.GetBingoId();

            mainWindow.SetBingoId(savedId);

            TimeSpan savedTimeSpan = TimeSpan.Parse(savedTime);
            _startTime = DateTime.Now - savedTimeSpan;

            Application.Current.Dispatcher.Invoke(() =>
            {

                int index = 0;
                foreach (UIElement child in BoardGrid.Children)
                {
                    if (child is Border border && index < savedBoard.Count)
                    {
                        int colorNumber = savedBoard[index];
                        Color color = colorNumber == 0 ? ((SolidColorBrush)FindResource("Hit-Yellow")).Color : // Hit-Yellow
                                      colorNumber == 1 ? ((SolidColorBrush)FindResource("Hit-Blue")).Color :   // Hit-Blue
                                      colorNumber == 2 ? ((SolidColorBrush)FindResource("Hit-White")).Color :  // White
                                      ((SolidColorBrush)FindResource("Hit-Grey")).Color;                       // Default Grey

                        border.Background = new SolidColorBrush(color);
                        if (border.Tag is TileData tileData)
                        {
                            tileData.State = colorNumber >= 2 ? "Normal" : "Toggled";
                            border.Tag = new TileData
                            {
                                Column = tileData.Column,
                                Row = tileData.Row,
                                State = tileData.State
                            };
                        }
                        index++;
                    }
                }
            });

            if (_timer == null)
            {
                StartTimer();
            }
            else
            {
                _timer.Start();
            }
        }

        private async void BingoButton_Click(object sender, RoutedEventArgs e)
        {
            string screenshotPath = null;
            InputDialog inputDialog = new InputDialog("Enter Your Name", "Discord Bingo Name", false);
            inputDialog.ShowDialog();
            if (inputDialog.InputString != "")
            {
                try
                {
                    screenshotPath = CaptureScreenshot();
                    await SendScreenshotToDiscord(screenshotPath, inputDialog.InputString); // Wait for the Discord upload to complete
                }
                catch (Exception ex)
                {
                    MessageDialog messageDialog = new MessageDialog($"An error occurred: {ex.Message}", "Error");
                    messageDialog.ShowDialog();
                }
                finally
                {
                    if (screenshotPath != null && File.Exists(screenshotPath))
                    {
                        try { File.Delete(screenshotPath); }
                        catch (Exception deleteEx)
                        {
                            MessageDialog messageDialog = new MessageDialog($"Failed to delete the screenshot file: {deleteEx.Message}", "Error");
                            messageDialog.ShowDialog();
                        }
                    }
                }
            }
        }

        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string screenshotPath = CaptureScreenshot();
                MessageDialog messageDialog = new MessageDialog($"Screenshot saved to:\n{screenshotPath}", "Screenshot Taken");
                messageDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog($"An error occurred while taking the screenshot: {ex.Message}", "Error");
                messageDialog.ShowDialog();
            }
        }

        private string CaptureScreenshot()
        {
            var originalWindowState = mainWindow.WindowState;
            var originalWidth = mainWindow.Width;
            var originalHeight = mainWindow.Height;
            var originalTop = mainWindow.Top;
            var originalLeft = mainWindow.Left;

            try
            {
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Width = 675;
                mainWindow.Height = 675;
                mainWindow.Top = (SystemParameters.PrimaryScreenHeight - mainWindow.Height) / 2;
                mainWindow.Left = (SystemParameters.PrimaryScreenWidth - mainWindow.Width) / 2;

                mainWindow.UpdateLayout();

                var renderTarget = new RenderTargetBitmap(
                    (int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                renderTarget.Render(this);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                string screenshotsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "BingoScreenshots");
                Directory.CreateDirectory(screenshotsFolder);
                string filePath = Path.Combine(screenshotsFolder, $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                Clipboard.SetImage(renderTarget);

                mainWindow.WindowState = originalWindowState;
                mainWindow.Width = originalWidth;
                mainWindow.Height = originalHeight;
                mainWindow.Top = originalTop;
                mainWindow.Left = originalLeft;
                mainWindow.UpdateLayout();

                return filePath;
            }
            catch
            {
                mainWindow.WindowState = originalWindowState;
                mainWindow.Width = originalWidth;
                mainWindow.Height = originalHeight;
                mainWindow.Top = originalTop;
                mainWindow.Left = originalLeft;
                mainWindow.UpdateLayout();

                throw;
            }
        }

        private async Task SendScreenshotToDiscord(string filePath, string name)
        {
            string webhookUrl = "https://discord.com/api/webhooks/1298556596762312714/kSg2r5612lLcvzAikbkcOUQWVJ6_AjdXg4UNVpyCm1tLeZgU-8tAsrV1X4_oV21L2fBu";

            using (HttpClient client = new HttpClient())
            {
                MultipartFormDataContent formData = new MultipartFormDataContent();
                formData.Add(new StringContent($"Bingo!! :tada:   **{name}** har fået 5 på stribe :trophy:"), "content");

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var streamContent = new StreamContent(fileStream);
                    formData.Add(streamContent, "file", Path.GetFileName(filePath));
                    HttpResponseMessage response = await client.PostAsync(webhookUrl, formData);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageDialog messageDialog = new MessageDialog("Screenshot successfully sent to Discord!", "Success");
                        messageDialog.ShowDialog();
                    }
                    else
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Failed to send screenshot to Discord. Status: {response.StatusCode}, Response: {responseBody}");
                    }
                }
            }
        }



        public void RegenBoardHitPieces()
        {
            List<string> stringList = new List<string>();

            //TODO Remove "..", "..", Or save it in appdata or documents
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "RubyQuotes.txt"); // For testing
            filePath = Path.GetFullPath(filePath);

            //string appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);  // For release
            //string filePath = Path.Combine("Data", "RubyQuotes.txt");

            try
            {
                stringList.AddRange(File.ReadAllLines(filePath, Encoding.UTF7));
                //Console.WriteLine("Ruby quotes are as follows:\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading the file: " + ex.Message);
            }

            foreach (string line in stringList)
            {
                //Console.WriteLine(line);
            }

            if (mainWindow.GetRemainingTime() <= 1)
            {
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

                mainWindow.SetBingoId(randomNumber);
                InputHitPieces(randomNumber);
            } else
            {
                InputHitPieces(mainWindow.GetBingoId());
            }

            void InputHitPieces(int Id)
            {
                Random random = new Random(Id);

                foreach (UIElement child in BoardGrid.Children)
                {
                    if (child is Border border)
                    {
                        if (border.Child is TextBox textBox)
                        {
                            if (stringList.Count > 0)
                            {
                                int randomIndex = random.Next(stringList.Count);
                                string randomElement = stringList[randomIndex];
                                textBox.Text = randomElement; 
                                stringList.RemoveAt(randomIndex); 
                            }
                            else
                            {
                                textBox.Text = "Empty"; // Default text if no more data is available
                            }
                        }
                    }
                }
            }

            if (PreviousId > 999)
            {
                mainWindow.SetBingoId(PreviousId);
                mainWindow.SetRemainingTime(PreviousTime);
            }
        }

        private void UpdateMaxSize()
        {
            double screenWidth = this.ActualWidth;
            maxSize = screenWidth/ 2 - 70;
        }

        private void GridSplitter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double leftColumnWidth = ColLeft.ActualWidth;
            double rightColumnWidth = ColRight.ActualWidth;

            double delta = e.HorizontalChange;
            double newLeftColumnWidth = leftColumnWidth - delta;
            double newRightColumnWidth = rightColumnWidth - delta;

            if (Math.Abs(delta) < 0.1) return;

            UpdateMaxSize();

            if (IsWidthValid(newLeftColumnWidth) && IsWidthValid(newRightColumnWidth))
            {
                ColLeft.Width = new GridLength(newLeftColumnWidth);
                ColRight.Width = new GridLength(newRightColumnWidth);
            }

            if (newLeftColumnWidth > maxSize)
            {
                ColLeft.Width = new GridLength(maxSize);
                ColRight.Width = new GridLength(maxSize);
            }
            else if (newRightColumnWidth > maxSize)
            {
                ColLeft.Width = new GridLength(maxSize);
                ColRight.Width = new GridLength(maxSize);
            }
        }

        private bool IsWidthValid(double newWidth)
        {
            return newWidth > 0 && newWidth < maxSize;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMaxSize();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.Topmost = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.Topmost = false;
        }

        private void StartRealTimeClock()
        {
            var clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Update every second
            };
            clockTimer.Tick += (sender, e) =>
            {
                // Update the label with the current time
                RealTimeLabel.Content = "    " + DateTime.Now.ToString("HH:mm");
            };
            clockTimer.Start();
        }

        private void StartTimer()
        {
            _startTime = DateTime.Now;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Update every second
            };
            _timer.Tick += UpdateElapsedTime;
            _timer.Start();
        }

        private void UpdateElapsedTime(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - _startTime;

            // Calculate total minutes
            int hours = (int)elapsed.Hours;
            int minutes = (int)elapsed.Minutes;
            int seconds = elapsed.Seconds;

            // Update the label content
            ElapsedTimeLabel.Content = $"Time {hours:00}:{minutes:00}:{seconds:00}";
        }

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= UpdateElapsedTime;
            }
        }
    }
}
