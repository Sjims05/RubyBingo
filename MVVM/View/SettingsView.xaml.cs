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

namespace RubyBingoApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {

        int yellowHighlight = 0;
        int greyHighlight = 0;

        public SettingsView()
        {
            InitializeComponent();
        }

        private void HighlightChange()
        {
            var activeResource = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("ActiveResource.xaml"));

            if (activeResource != null)
            {
                // Remove the current theme (DarkTheme.xaml)
                var darkTheme = activeResource.MergedDictionaries
                    .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("DarkTheme.xaml"));

                if (darkTheme != null)
                {
                    activeResource.MergedDictionaries.Remove(darkTheme);
                }

                // Add the new theme (LightTheme.xaml)
                var lightTheme = new ResourceDictionary
                {
                    Source = new Uri(@"\Resources\Themes\LightTheme.xaml", UriKind.Relative)
                };
                activeResource.MergedDictionaries.Add(lightTheme);
            }
        }

        private void SwitchTheme()
        {
            var activeThemeDict = this.Resources.MergedDictionaries
                        .FirstOrDefault(dict => dict.Source.OriginalString.Contains("ActiveTheme.xaml"));

            foreach (var dict in activeThemeDict.MergedDictionaries)
            {
                if (dict.Source.OriginalString.Contains("Dark") || dict.Source.OriginalString.Contains("Light"))
                {
                    Console.WriteLine($"{dict.Source.OriginalString}");
                    this.Resources.Clear();
                }
            }
        }


        private void ThemeToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            SwitchTheme();
        }

        private void ThemeToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            SwitchTheme();
        }

        private void YellowHighlightingToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void YellowHighlightingToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void GreyHighlightingToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void GreyHighlightingToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
