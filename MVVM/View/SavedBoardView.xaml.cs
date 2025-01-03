using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using RubyBingoApp.MVVM.ViewModel;
using System.Windows.Media;
using RubyBingoApp.MVVM.View.DialogBoxes;

namespace RubyBingoApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for SavedBoardView.xaml
    /// </summary>
    public partial class SavedBoardView : UserControl
    {
        int itemCount = 0;
        int itemWidth = 250;

        public SavedBoardView()
        {
            InitializeComponent();
            LoadBoards();
        }

        private void LoadBoards()
        {
            //TODO Remove "..", "..", Or save it in appdata or documents
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SavedBoards");
            filePath = Path.GetFullPath(filePath);

            try
            {
                if (!Directory.Exists(filePath)) return;

                var files = Directory.GetFiles(filePath)
                                     .OrderByDescending(fileName => Path.GetFileName(fileName))
                                     .ToList();

                itemCount = files.Count;
                UpdateUniformGrid(itemCount, this.DynamicUniformGrid.ActualWidth, itemWidth);

                foreach (var file in files)
                {
                    var fileContent = File.ReadAllLines(file);

                    string id = fileContent[0];
                    string date = fileContent[1];
                    string dateTime = fileContent[3];
                    string time = fileContent[2];
                    List<int> board = new List<int>();

                    var parts = fileContent[4].Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        board.Add(int.Parse(parts[i]));
                    }

                    int count = 0;
                    foreach (int num in board)
                    {
                        if (num <= 1)
                        {
                            count++;
                        }
                    }
                    string progress = count.ToString() + "/" + board.Count.ToString();

                    Console.WriteLine(id);
                    Console.WriteLine(date);
                    Console.WriteLine(dateTime);
                    Console.WriteLine(time);
                    Console.WriteLine(string.Join(",", board));

                    var savedBoardWidget = new SavedBoardWidget
                    {
                        Id = id,
                        Date = date,
                        Time = time,
                        DateTime = dateTime,
                        BoardHits = board,
                        Progress = progress,
                        FilePath = file
                    };
                    ContentControl boardControl = new ContentControl
                    {
                        DataContext = savedBoardWidget,
                        Template = (ControlTemplate)this.Resources["CustomWidgetTemplate"]
                    };
                    boardControl.ApplyTemplate();

                    for (int i = 0; i < 25; i++)
                    {
                        var border = (Border)boardControl.Template.FindName($"Border{i}", boardControl);
                        if (border != null)
                        {
                            switch (savedBoardWidget.BoardHits[i])
                            {
                                case 0:
                                    border.Style = (Style)this.Resources["Hit-Yellow-Border"];
                                    break;
                                case 1:
                                    border.Style = (Style)this.Resources["Hit-Blue-Border"];
                                    break;
                                case 2:
                                    border.Style = (Style)this.Resources["Hit-White-Border"];
                                    break;
                                case 3:
                                    border.Style = (Style)this.Resources["Hit-Grey-Border"];
                                    break;
                                default:
                                    border.Style = (Style)this.Resources["Hit-Grey-Border"];
                                    break;
                            }
                        }
                    }

                    this.DynamicUniformGrid.Children.Add(boardControl);
                }
            }
            catch (Exception ex) { Console.WriteLine($"An error occurred: {ex.Message}"); }
        }

        private void UpdateUniformGrid(int itemCount, double containerWidth, double itemWidth)
        {
            int columns = Math.Max(1, (int)(containerWidth / itemWidth));
            int rows = (int)Math.Ceiling((double)itemCount / columns);

            DynamicUniformGrid.Columns = columns;
            DynamicUniformGrid.Rows = rows;
        }

        private void DynamicUniformGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateUniformGrid(itemCount, this.DynamicUniformGrid.ActualWidth, itemWidth);
        }

        private void Widget_Close(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Console.WriteLine("dan");
            if (sender is FrameworkElement element && element.DataContext is SavedBoardWidget widget)
            {
                string filePath = widget.FilePath;
                if (!string.IsNullOrEmpty(filePath))
                {
                    // Perform operations with the file path, e.g., delete the file
                    if (File.Exists(filePath))
                    {

                        Console.WriteLine($"Deleted file: {filePath}");
                    }
                }
            }
        }

    }
}
