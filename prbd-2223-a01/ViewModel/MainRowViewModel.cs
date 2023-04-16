using System.Windows.Input;
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
        get => _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }

    public bool Editable => !EditMode && Participant == App.CurrentUser;

    private void EditModeChanged() {

        if (Participant == App.CurrentUser)
            foreach (var Mcvm in _cellsVm)
                Mcvm.EditMode = EditMode;
        _pollVotesViewModel.AskEditMode(EditMode);
    }

    private List<MainCellViewModel> _cellsVm = new();
    public List<MainCellViewModel> CellsVM {
        get => _cellsVm;
        private set => SetProperty(ref _cellsVm, value);
    }

    private void RefreshChoices() {
        CellsVM = Choices
            .Select(c => new MainCellViewModel(Participant, c))
            .ToList();
    }

    private void Save() {

    }

    private void Cancel() {}

    private void Delete() {}
}
