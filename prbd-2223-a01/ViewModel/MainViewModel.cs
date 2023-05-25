using System.Windows.Input;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainViewModel : ViewModelCommon {

    public ICommand LogoutCommand { get; }
    public ICommand ReloadCommand { get; }

    public string Title { get; } = "My Poll App (" + App.CurrentUser.FullName + ")";

    public MainViewModel() : base() {
        LogoutCommand = new RelayCommand(() => NotifyColleagues(App.Messages.MSG_LOGOUT));
        ReloadCommand = new RelayCommand(() => {
            if (Context.ChangeTracker.HasChanges()) return;
            App.ClearContext();
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        });
    }
}
