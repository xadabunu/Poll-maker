using System.Collections.ObjectModel;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainViewModel : ViewModelCommon {

    public ICommand LogoutCommand { get; }

    public string Title { get; set; } = "My Poll App (" + App.CurrentUser.FullName + ")";

    public MainViewModel() : base() {
        LogoutCommand = new RelayCommand(() => Logout());
    }

    private void Logout() {
        Console.WriteLine("logout");
    }
}
