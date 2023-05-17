using System.Windows.Input;
using System.Windows.Media;
using FontAwesome6;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainCellViewModel : ViewModelCommon {

    public ICommand ChangeVoteCommand { get; set; }

    public MainCellViewModel(User participant, Choice choice, MainRowViewModel mainRowViewModel) {
        IsVoted = Context.Votes.Any(v => v.User == participant && v.Choice == choice);
        Vote = Context.Votes.FirstOrDefault(v => v.User == participant && v.Choice == choice) ?? new Vote {
            Choice = choice,
            User = participant,
            Value = VoteValue.None
        };

        ChangeVoteCommand = new RelayCommand<string>(s => {
            VoteValue val = s switch {
                "Yes" => VoteValue.Yes,
                "Maybe" => VoteValue.Maybe,
                "No" => VoteValue.No,
                _ => VoteValue.None
            };
            Vote.Value = Vote.Value == val ? VoteValue.None : val;
            if (choice.Poll.IsSimple && Vote.Value != VoteValue.None) {
                mainRowViewModel.ClearVotes(this);
            }
            Console.WriteLine(Vote.Value);
            IsVoted = Vote.Value != VoteValue.None;
            RaiseProperties();
        });
    }

    private void RaiseProperties() {
        RaisePropertyChanged(nameof(VotedYes));
        RaisePropertyChanged(nameof(VotedNo));
        RaisePropertyChanged(nameof(VotedMaybe));
        RaisePropertyChanged(nameof(VotedIcon));
        RaisePropertyChanged(nameof(VotedColor));
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

    public void ClearVote() {
        Vote.Value = VoteValue.None;
        RaiseProperties();
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

    public bool VotedYes => Vote.Value == VoteValue.Yes;
    public bool VotedMaybe => Vote.Value == VoteValue.Maybe;
    public bool VotedNo => Vote.Value == VoteValue.No;
}
