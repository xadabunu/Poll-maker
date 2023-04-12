using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll.View;

public partial class PollVotesView : UserControlBase {
    public PollVotesView(Poll poll) {
        InitializeComponent();
        DataContext = new PollVotesViewModel(poll);
    }
}

