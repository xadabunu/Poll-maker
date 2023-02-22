using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainViewModel : ViewModelBase<User, MyPollContext> {
    public string Title { get; } = "prbd-2223-a01";
}

