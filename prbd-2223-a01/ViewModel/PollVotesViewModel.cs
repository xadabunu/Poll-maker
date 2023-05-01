using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollVotesViewModel : ViewModelCommon {

    public ICommand EditPollCommand { get; }
    public ICommand AddCommentCommand { get; }
    public ICommand PostCommand { get; }
    public ICommand DeleteCommentCommand { get; }
    public ICommand OpenPollCommand { get; }
    public ICommand DeletePollCommand { get; }
    public ObservableCollection<Comment> Comments { get; set; }

    public PollVotesViewModel(Poll poll) {
        Poll = poll;
        _isNew = Poll.Title == App.NEW_POLL_LABEL;
        EditPollMode = _isNew;
        IsClosed = Poll.IsClosed;

        var participants = poll.Participants.OrderBy(p => p.FullName);

        _participantsVM = participants.Select(p => new MainRowViewModel(this, p, poll.Choices.ToList())).ToList();
        EditPollCommand = new RelayCommand(() => {
            EditPollMode = true;
            ShowGrid = false;
        });
        AddCommentCommand = new RelayCommand(() => WritingMode = true);
        PostCommand = new RelayCommand(() => PostAction(),
            () => !Comment.IsNullOrEmpty());

        Comments = new ObservableCollection<Comment>(Context.Comments
            .Where(c => c.Poll == poll)
            .OrderByDescending(c => c.CreationDate));

        DeleteCommentCommand = new RelayCommand<Comment>(comment => {
            Context.Comments.Remove(comment);
            Context.SaveChanges();
            Comments.Remove(comment);
        });

        OpenPollCommand = new RelayCommand(() => {
            Poll.Status = PollStatus.Open;
            Context.SaveChanges();
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        });

        DeletePollCommand = new RelayCommand(() => {
            var result = MessageBox
                .Show("This poll and all its comments and votes will be deleted.\nDo you confirm?",
                    "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                Context.Polls.Remove(Poll);
                Context.SaveChanges();
                NotifyColleagues(App.Messages.MSG_POLL_DELETED, Poll);
            }
        });

        /* ------------------ Add/Edit part ------------------ */

        EditTitle = Poll.Title;
        EditType = Poll.Type == PollType.Multiple ? 0 : 1;
        NoChoice = Poll.Choices.Count == 0;
        NoParticipant = Poll.Participants.Count == 0;
        IsChecked = Poll.Status == PollStatus.Closed;
        ShowGrid = !EditPollMode && !NoChoice && !NoParticipant;

        CancelCommand = new RelayCommand(() => {
            if (_isNew) {
                ClearErrors();
                NotifyColleagues(App.Messages.MSG_POLL_DELETED, Poll);
            }
            EditPollMode = false;
            Participants = new ObservableCollection<User>(Poll.Participants);
            EditChoices = new ObservableCollection<Choice>(Poll.Choices);
            Addables = new ObservableCollection<User>(
                Context.Users.Where(u => !Participants.Contains(u)).OrderBy(u => u.FullName));

            IsChecked = Poll.Status == PollStatus.Closed;
            EditType = Poll.Type == PollType.Multiple ? 0 : 1;
            _editedChoice = null;
            NewChoice = "";
            ShowGrid = !NoParticipant && !NoChoice;
            //clearChanges ?
        });

        Participants = new ObservableCollection<User>(Poll.Participants);

        Addables = new ObservableCollection<User>(
            Context.Users.Where(u => !Participants.Contains(u)).OrderBy(u => u.FullName));

        DeleteParticipantCommand = new RelayCommand<User>(u => {
            Participants.Remove(u);
            Poll.Participants.Remove(u);
            NoParticipant = Participants.Count == 0;
            Addables.Add(u);
        });

        EditChoices = new ObservableCollection<Choice>(Poll.Choices);

        EditChoiceCommand = new RelayCommand<Choice>(choice => {
            _editedChoice = choice;
            NewChoice = choice.Label;
        });

        AddChoiceCommand = new RelayCommand(() => {
            EditChoices.Remove(_editedChoice);
            NoChoice = false;
            var c = new Choice { PollId = Poll.Id, Label = NewChoice };
            EditChoices.Add(c);
            Poll.Choices.Add(c);
            NewChoice = "";
        }, () => !NewChoice.IsNullOrEmpty());

        DeleteChoiceCommand = new RelayCommand<Choice>(choice => {
            EditChoices.Remove(choice);
            Poll.Choices.Remove(choice);
            NoChoice = EditChoices.Count == 0;
        });

        AddParticipantCommand = new RelayCommand<User>(AddParticipantAction);

        AddMySelfCommand = new RelayCommand(() => {
            AddParticipantAction(CurrentUser);
        }, () => !Participants.Contains(CurrentUser));

        AddEverybodyCommand = new RelayCommand(() =>
            Addables.ToList().ForEach(u => AddParticipantAction(u)));

        SaveCommand = new RelayCommand(() => {
            if (_isNew)
                Context.Polls.Add(Poll);
            Context.SaveChanges();
            EditPollMode = false;
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        }, () => ValidateTitle() && HasChanges);
    }

    public List<Choice> Choices => Poll.Choices.OrderBy(c => c.Label).ToList();

    public Poll Poll { get; set; }

    private List<MainRowViewModel> _participantsVM;
    public List<MainRowViewModel> ParticipantsVM => _participantsVM;

    public void AskEditMode(bool editMode) {
        ParticipantsVM.ForEach(vm => vm.Changes());
    }

    public bool IsCreator => CurrentUser == Poll.Creator;

    private bool _isClosed;
    public bool IsClosed {
        get => _isClosed;
        set => SetProperty(ref _isClosed, value);
    }

    public bool CanEdit => IsCreator && !EditPollMode;

    private bool _writingMode;
    public bool WritingMode {
        get => _writingMode;
        set => SetProperty(ref _writingMode, value);
    }

    private string _comment;
    public string Comment {
        get => _comment;
        set => SetProperty(ref _comment, value);
    }

    private void PostAction() {
        var c = new Comment {
            AuthorId = CurrentUser.Id,
            PollId = Poll.Id,
            Text = Comment,
            CreationDate = DateTime.Now
        };
        Comments.Insert(0, c);
        Context.Comments.Add(c);
        Context.SaveChanges();
        WritingMode = false;
        Comment = "";
    }

    private bool _editPollMode;

    private bool _showGrid;
    public bool ShowGrid {
        get => _showGrid;
        set => SetProperty(ref _showGrid, value);
    }

    protected sealed override void OnRefreshData() {
        Poll = Context.Polls.First(p => p.Id == Poll.Id);
        IsClosed = Poll.IsClosed;
        Comments = new ObservableCollection<Comment>(Poll.Comments.OrderByDescending(c => c.CreationDate));

        /* -------------------------- Add/Edit -------------------------- */

        Participants = new ObservableCollection<User>(Poll.Participants);
        Addables = new ObservableCollection<User>(
            Context.Users.Where(u => !Participants.Contains(u)).OrderBy(u => u.FullName));
        EditChoices = new ObservableCollection<Choice>(Poll.Choices);
        EditTitle = Poll.Title;
        EditType = Poll.Type == PollType.Multiple ? 0 : 1;
        NoChoice = Poll.Choices.Count == 0;
        NoParticipant = Poll.Participants.Count == 0;
        IsChecked = Poll.Status == PollStatus.Closed;
    }

    /* -------------------------- Add/Edit -------------------------- */

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteParticipantCommand { get; }
    public ICommand EditChoiceCommand { get; }
    public ICommand DeleteChoiceCommand { get; }
    public ICommand AddChoiceCommand { get; }
    public ICommand AddParticipantCommand { get; }
    public ICommand AddMySelfCommand { get; }
    public ICommand AddEverybodyCommand { get; }
    public ObservableCollection<User> Participants { get; private set; }
    public ObservableCollection<User> Addables { get; set; }
    public ObservableCollection<Choice> EditChoices { get; private set; }
    private User _added;
    private Choice _editedChoice;

    private bool _isChecked;
    private bool _isNew;

    public bool IsChecked {
        get => _isChecked;
        set {
            SetProperty(ref _isChecked, value);
            Poll.Status = IsChecked ? PollStatus.Closed : PollStatus.Open;
        }
    }

    private bool _noChoice;
    public bool NoChoice {
        get => _noChoice;
        set => SetProperty(ref _noChoice, value);
    }

    private bool _noParticipant;
    public bool NoParticipant {
        get => _noParticipant;
        set => SetProperty(ref _noParticipant, value);
    }

    private int _editType;
    public int EditType {
        get => _editType;
        set {
            SetProperty(ref _editType, value);
            Poll.Type = EditType == 0 ? PollType.Multiple : PollType.Simple;
        }
    }

    public User Added {
        get => _added;
        set => SetProperty(ref _added, value);
    }

    private string _newChoice;
    public string NewChoice {
        get => _newChoice;
        set => SetProperty(ref _newChoice, value.Trim());
    }

    public bool EditPollMode {
        get => _editPollMode;
        set => SetProperty(ref _editPollMode, value);
    }

    private string _editTitle;
    public string EditTitle {
        get => _editTitle;
        set => SetProperty(ref _editTitle, value, () => ValidateTitle());
    }

    private  bool ValidateTitle() {
        ClearErrors();

        if (EditTitle.IsNullOrEmpty())
            AddError(nameof(EditTitle), "Title required");
        else if (EditTitle.Length < 7)
            AddError(nameof(EditTitle), "Title length must be at least 7 char");
        else if (EditTitle == App.NEW_POLL_LABEL)
            AddError(nameof(EditTitle), "Sorry, this name is reserved :/");
        else {
            Poll.Title = EditTitle;
            return HasChanges;
        }

        return !HasErrors;
    }

    // private bool ValidateChoice() {
    //     ClearErrors();
    //
    //     bool b = !NewChoice.IsNullOrEmpty() && NewChoice.Length > 2;
    //     if (!NewChoice.IsNullOrEmpty() && NewChoice.Length < 3) {
    //         AddError(nameof(NewChoice), "Choice must be >= 3 characters long");
    //     }
    //
    //     return b;
    // }

    private void AddParticipantAction(User user) {
        NoParticipant = false;
        Participants.Add(user);
        Poll.Participants.Add(user);
        Addables.Remove(user);
    }

    public bool CanBeSingle {
        get {
            bool b = true;

            Poll.Votes.GroupBy(v => v.User).ToList()
                .ForEach(l => {
                    if (l.Count() > 1)
                        b = false;
                });
            return b;
        }
    }
}
