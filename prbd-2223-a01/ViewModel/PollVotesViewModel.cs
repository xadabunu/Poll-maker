using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollVotesViewModel : ViewModelCommon {

    public PollVotesViewModel(Poll poll) {
        Poll = poll;

        // Save = new RelayCommand(SaveAction, CanSaveAction);
    }

    public Poll Poll { get; set; }
    public string CreatorTitle => " x)";
}
