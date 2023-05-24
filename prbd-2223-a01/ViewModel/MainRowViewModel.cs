using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainRowViewModel : ViewModelCommon {

    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }


    public User Participant { get; set; }
    private PollVotesViewModel _pollVotesViewModel;

    private List<Choice> _choices;
    public List<Choice> Choices => _choices.OrderBy(c => c.Label).ToList();
    private Poll _poll;
    private bool _isClosed;

    public MainRowViewModel(PollVotesViewModel pollVotesViewModel, Poll poll, User participant) {

        _poll = poll;

        _choices = poll.Choices.ToList();
        _isClosed = poll.IsClosed;
        Participant = participant;
        _pollVotesViewModel = pollVotesViewModel;
        RefreshChoices();

        EditCommand = new RelayCommand(() => EditMode = true);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete);
    }

    private bool _editMode;
    public bool EditMode {
        get => (IsAdmin || App.CurrentUser == Participant) && _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }

    public bool Editable => (IsAdmin || Participant == App.CurrentUser) && !_editMode && !_isClosed;

    private void EditModeChanged() {
        if (IsAdmin || Participant == App.CurrentUser)
            _cellsVm.ForEach(vm => vm.EditMode = EditMode);
        _pollVotesViewModel.AskEditMode();
    }

    private List<MainCellViewModel> _cellsVm = new();
    public List<MainCellViewModel> CellsVM {
        get => _cellsVm;
        private set => SetProperty(ref _cellsVm, value);
    }

    public void Changes() {
        RaisePropertyChanged(nameof(Editable));
    }

    public void ClearVotes(MainCellViewModel cellvm) {
        CellsVM.Where(other => other != cellvm).ToList()
            .ForEach(vm => vm.ClearVote());
    }

    private void RefreshChoices() {
        CellsVM = Choices
            .Select(c => new MainCellViewModel(Participant, c, this))
            .ToList();
    }

    private void Save() {
        CellsVM.Where(vm => vm.Vote.Value == VoteValue.ToRemove)
            .ToList().ForEach(vm => Context.Votes.Remove(vm.Vote));
        CellsVM.Where(vm => vm.IsVoted && !Context.Votes.Contains(vm.Vote))
            .ToList().ForEach(vm => Context.Votes.Add(vm.Vote));

        Context.SaveChanges();
        EditMode = false;
        RefreshChoices();
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }

    private void Cancel() {
        Context.ChangeTracker.Clear();
        RefreshChoices();
        EditMode = false;
    }

    private void Delete() {
        CellsVM.Where(vm => vm.Vote.Value != VoteValue.None)
            .ToList().ForEach(vm => Context.Votes.Remove(vm.Vote));

        Context.SaveChanges();
        RefreshChoices();
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }

    protected override void OnRefreshData() {
        var poll = Context.Polls.Find(_poll.Id);
        if (poll != null)
            _isClosed = poll.IsClosed;
    }
}
