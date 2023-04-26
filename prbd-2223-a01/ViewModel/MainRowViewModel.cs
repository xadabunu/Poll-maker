using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using MyPoll.Model;
using MyPoll.View;
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

    public MainRowViewModel(PollVotesViewModel pollVotesViewModel, User participant, List<Choice> choices) {

        _choices = choices;
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
        get => App.CurrentUser == Participant && _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }

    public bool Editable => Participant == App.CurrentUser && !_editMode;

    private void EditModeChanged() {
        if (Participant == App.CurrentUser)
            _cellsVm.ForEach(vm => vm.EditMode = EditMode);
        _pollVotesViewModel.AskEditMode(EditMode);
    }

    private List<MainCellViewModel> _cellsVm = new();
    public List<MainCellViewModel> CellsVM {
        get => _cellsVm;
        private set => SetProperty(ref _cellsVm, value);
    }

    public void Changes() {
        RaisePropertyChanged(nameof(Editable));
    }

    private void RefreshChoices() {
        CellsVM = Choices
            .Select(c => new MainCellViewModel(Participant, c))
            .ToList();
    }

    private void Save() {
        Participant.Votes = CellsVM.Where(vm => vm.IsVoted).Select(vm => vm.Vote).ToList();
        Context.SaveChanges();
        EditMode = false;
        RefreshChoices();
    }

    private void Cancel() {
        Context.Votes.Where(v => v.Value == VoteValue.None).ExecuteDelete();
        RefreshChoices();
        EditMode = false;
    }

    private void Delete() {
        CellsVM.ToList().ForEach(vm => vm.Vote.Value = VoteValue.None);
        Context.Votes.Where(v => v.Value == VoteValue.None).ExecuteDelete();
        Context.SaveChanges();
        RefreshChoices();
    }
}
