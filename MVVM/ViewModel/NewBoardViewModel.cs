using RubyBingoApp.Core;
using RubyBingoApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubyBingoApp.MVVM.ViewModel
{
    public class NewBoardViewModel : Core.ViewModel
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

        public NewBoardViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateToHomeViewCommand = new RelayCommand(execute => { Navigation.NavigateTo<HomeViewModel>(); }, canExecute => true);
        }
    }
}
