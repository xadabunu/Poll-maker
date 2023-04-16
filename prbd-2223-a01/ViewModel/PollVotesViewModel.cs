using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollVotesViewModel : ViewModelCommon {

    public PollVotesViewModel(Poll poll) {
        Poll = poll;

        var participants = poll.Participants.OrderBy(p => p.FullName);

        _participantsVM = participants.Select(p => new MainRowViewModel(this, p, poll.Choices.ToList())).ToList();
    }

    public Poll Poll { get; set; }

    private List<MainRowViewModel> _participantsVM;
    public List<MainRowViewModel> ParticipantsVM => _participantsVM;
}
