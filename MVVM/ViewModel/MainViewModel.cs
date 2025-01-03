using RubyBingoApp.Core;
using RubyBingoApp.Service;

namespace RubyBingoApp.MVVM.ViewModel
{
    public class MainViewModel : Core.ViewModel
    {
        private INavigationService _navigation;

        public INavigationService Navigation
        {
            get => _navigation;
            set 
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateToHomeViewCommand { get; set; }
        public RelayCommand NavigateToSavedBoardViewCommand { get; set; }
        public RelayCommand NavigateToStatsViewCommand { get; set; }
        public RelayCommand NavigateToStylingViewCommand { get; set; }
        public RelayCommand NavigateToSettingsViewCommand { get; set; }

        public MainViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateToHomeViewCommand = new RelayCommand(execute => { Navigation.NavigateTo<HomeViewModel>(); }, canExecute => true);
            NavigateToSavedBoardViewCommand = new RelayCommand(execute => { Navigation.NavigateTo<SavedBoardViewModel>(); }, canExecute => true);
            NavigateToStatsViewCommand = new RelayCommand(execute => { Navigation.NavigateTo<StatsViewModel>(); }, canExecute => true);
            NavigateToStylingViewCommand = new RelayCommand(execute => { Navigation.NavigateTo<MainStylingViewModel>(); }, canExecute => true);
            NavigateToSettingsViewCommand = new RelayCommand(execute => { Navigation.NavigateTo<SettingsViewModel>(); }, canExecute => true);

            Navigation.NavigateTo<HomeViewModel>();
        }
    }
}
