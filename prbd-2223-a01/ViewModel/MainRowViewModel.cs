using MyPoll.Model;
using MyPoll.View;

namespace MyPoll.ViewModel;

public class MainRowViewModel : ViewModelCommon {

    public User Participant { get; set; }
    private PollVotesViewModel _pollVotesViewModel;

    private List<Choice> _choices;
    public List<Choice> Choices => _choices;

    public MainRowViewModel(PollVotesViewModel pollVotesViewModel, User participant, List<Choice> choices) {

        _choices = choices;
        Participant = participant;
        _pollVotesViewModel = pollVotesViewModel;
        RefreshChoices();
    }

    private List<MainCellViewModel> _cellsVm = new();

    public List<MainCellViewModel> CellsVM {
        get => _cellsVm;
        private set => SetProperty(ref _cellsVm, value);
    }

    private void RefreshChoices() {
        CellsVM = _choices
            .Select(c => new MainCellViewModel(Participant, c))
            .ToList();
    }
}
