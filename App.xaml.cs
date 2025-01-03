using Microsoft.Extensions.DependencyInjection;
using RubyBingoApp.Core;
using RubyBingoApp.MVVM.ViewModel;
using RubyBingoApp.Service;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RubyBingoApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {


            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<NewBoardViewModel>();
            services.AddSingleton<SavedBoardViewModel>();
            services.AddSingleton<StatsViewModel>();
            services.AddSingleton<MainStylingViewModel>();
            services.AddSingleton<BasicStylingViewModel>();
            services.AddSingleton<AdvancedStylingViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

            _serviceProvider = services.BuildServiceProvider();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Icon = new BitmapImage(new Uri("pack://application:,,,/RubyBingoApp;component/Assets/Icon.png"));
            mainWindow.Show();
            base.OnStartup(e);
        }
    }

}
