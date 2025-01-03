using RubyBingoApp.Core;
using RubyBingoApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubyBingoApp.MVVM.ViewModel
{
    public class HomeViewModel : Core.ViewModel
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

        public RelayCommand NavigateToNewBoardViewCommand { get; set; }

        public HomeViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateToNewBoardViewCommand = new RelayCommand(execute => { Navigation.NavigateTo<NewBoardViewModel>(); }, canExecute => true);
        }
    }
}
