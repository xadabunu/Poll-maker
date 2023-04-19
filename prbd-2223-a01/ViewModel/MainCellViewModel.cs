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
        _vote = Context.Votes.FirstOrDefault(v => v.User == participant && v.Choice == choice);

        ChangeVote = new RelayCommand(() => IsVoted = !IsVoted);
    }

    private Vote _vote;
    public Vote Vote {
        get => _vote;
        set => SetProperty(ref _vote, value);
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

    public EFontAwesomeIcon VotedIcon {
        get {
            if (Vote != null) {
                return Vote.Value switch {
                    VoteValue.Yes => EFontAwesomeIcon.Solid_Check,
                    VoteValue.Maybe => EFontAwesomeIcon.Solid_CircleQuestion,
                    VoteValue.No => EFontAwesomeIcon.Solid_X,
                    _ => EFontAwesomeIcon.None
                };
            }
            return EFontAwesomeIcon.None;
        }
    }

    public Brush VotedColor {
        get {
            if (Vote != null) {
                return Vote.Value switch {
                    VoteValue.Yes => Brushes.Green,
                    VoteValue.Maybe => Brushes.Orange,
                    VoteValue.No => Brushes.Red,
                    _ => Brushes.White
                };
            }
            return Brushes.White;
        }
    }

    public bool VotedYes {
        get => _vote.Value == VoteValue.Yes;
    }

    public bool VotedMaybe {
        get => _vote.Value == VoteValue.Maybe;
    }

    public bool VotedNo => Vote.Value == VoteValue.No;

    public string VotedToolTip => IsVoted ? "Yes" : "No";
}
