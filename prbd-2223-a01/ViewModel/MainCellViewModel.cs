using System.Windows.Input;
using System.Windows.Media;
using FontAwesome6;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainCellViewModel : ViewModelCommon {

    public ICommand ChangeVote { get; set; }

    public MainCellViewModel(User participant, Choice choice) {
        IsVoted = Context.Votes.Any(v => v.User == participant && v.Choice == choice);
        ChangeVote = new RelayCommand(() => IsVoted = !IsVoted);
    }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    private bool _isVoted;
    public bool IsVoted {
        get => _isVoted;
        set => SetProperty(ref _isVoted, value);
    }

    public EFontAwesomeIcon VotedIcon => IsVoted ? EFontAwesomeIcon.Solid_Check : EFontAwesomeIcon.None;
    public Brush VotedColor => IsVoted ? Brushes.Green : Brushes.White;
    public string VotedToolTip => IsVoted ? "Yes" : "No";
}
