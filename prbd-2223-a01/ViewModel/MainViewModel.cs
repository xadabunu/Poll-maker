using System.Collections.ObjectModel;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainViewModel : ViewModelBase<User, MyPollContext> {

    public ObservableCollection<User> Users { get; set; }

    public MainViewModel() : base() {

        Users = new ObservableCollection<User>(Context.Users.OrderBy(p => p.FullName));

    }

    protected override void OnRefreshData() {
    }

    public string Title { get; } = "List of all users";
}

