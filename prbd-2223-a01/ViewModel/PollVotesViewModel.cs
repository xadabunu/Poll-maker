using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollVotesViewModel : ViewModelCommon {

    static void p(string s) => Console.WriteLine(s);

    public ICommand EditPollCommand { get; }
    public ICommand AddCommentCommand { get; }
    public ICommand PostCommand { get; }
    public ICommand CancelCommentCommand { get; }
    public ICommand DeleteCommentCommand { get; }
    public ICommand OpenPollCommand { get; }
    public ICommand DeletePollCommand { get; }
    public ObservableCollection<Comment> Comments { get; set; }

    public PollVotesViewModel(Poll poll) {
        Poll = poll;
        _isNew = Poll.Title == Poll.NEW_POLL_LABEL;
        EditPollMode = _isNew;
        IsClosed = Poll.IsClosed;
        CanComment = !IsClosed;
        CanEdit = IsCreator && !EditPollMode;

        GetParticipants();

        ParticipantsVM = Participants.Select(p => new MainRowViewModel(this, Poll, p.User)).ToList();
        EditPollCommand = new RelayCommand(() => {
            EditPollMode = true;
            ShowGrid = false;
        });
        AddCommentCommand = new RelayCommand(() => {
            CanComment = false;
            WritingMode = true;
        });
        PostCommand = new RelayCommand(() => PostAction(),
            () => !Comment.IsNullOrEmpty());
        CancelCommentCommand = new RelayCommand(() => {
            Comment = "";
            WritingMode = false;
            CanComment = true;
        });

        Choices = Poll.Choices.OrderBy(c => c.Label).ToList();

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

        /* ------------------ partie Add/Edit du constructeur ------------------ */

        EditTitle = _isNew ? "" : Poll.Title;
        EditType = Poll.Type == PollType.Multiple ? 0 : 1;
        NoChoice = Poll.Choices.Count == 0;
        NoParticipant = Poll.Participants.Count == 0;
        IsChecked = Poll.Status == PollStatus.Closed;
        ShowGrid = !EditPollMode && !NoChoice && !NoParticipant;

        CancelCommand = new RelayCommand(() => {
            ClearErrors();
            if (_isNew) {
                Poll.Title = Poll.NEW_POLL_LABEL;
                NotifyColleagues(App.Messages.MSG_POLL_DELETED, Poll);
            }

            App.ClearContext();
            EditPollMode = false;
            CanEdit = true;
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        });

        GetAddables();
        GetEditChoices();

        DeleteParticipantCommand = new RelayCommand<dynamic>(obj => {
            if (obj.Nb > 0) {
                MessageBoxResult result = MessageBox
                    .Show("This participant has already " + obj.Nb + " vote(s) for this poll.\n" +
                          "If you proceed and save the poll, their vote(s) will be deleted.\nDo you confirm?",
                        "Confirmation", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes) {
                    return;
                }
            }

            User u = obj.User;
            Participants.Remove(obj);
            Poll.Participants.Remove(u);
            Poll.Votes.Where(v => v.User == u).ToList()
                .ForEach(v => Context.Votes.Remove(v));
            NoParticipant = Participants.Count == 0;
            GetAddables();
        });

        AddChoiceCommand = new RelayCommand(() => {
            NoChoice = false;
            Poll.Choices.Add(new Choice { PollId = Poll.Id, Label = NewChoice.Trim() });
            GetEditChoices();
            NewChoice = "";
        }, () => !NewChoice.IsNullOrEmpty());

        DeleteChoiceCommand = new RelayCommand<EditChoiceViewModel>(vm => {
            if (vm.Nb > 0) {
                MessageBoxResult result = MessageBox
                    .Show("This choice has already " + vm.Nb + " vote(s).\n" +
                          "If you proceed ans save the poll, the corresponding vote(s) will be deleted.\nDo you confirm?",
                        "Confirmation", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes) {
                    return;
                }
            }
            EditChoices.Remove(vm);
            Poll.Choices.Remove(vm.Choice);
            NoChoice = EditChoices.Count == 0;
        });

        AddParticipantCommand = new RelayCommand<User>(AddParticipantAction, user => user != null);

        AddMySelfCommand = new RelayCommand(() => {
            AddParticipantAction(CurrentUser);
        }, () => !Participants.Contains(CurrentUser));

        AddEverybodyCommand = new RelayCommand(() =>
            Addables.ToList().ForEach(u => AddParticipantAction(u)));

        SaveCommand = new RelayCommand(() => {
            if (_isNew) {
                Context.Polls.Add(Poll);
                Context.SaveChanges();
                NotifyColleagues(App.Messages.MSG_RETAG_TAB, Poll);
                _isNew = false;
            } else
                Context.SaveChanges();
            EditPollMode = false;
            CanEdit = true;
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        }, () => ValidateTitle() && NoEditingChoice && (_isNew || HasChanges));
    }

    private List<Choice> _choices;
    public List<Choice> Choices {
        get => _choices;
        private set => SetProperty(ref _choices, value);
    }

    public Poll Poll { get; private set; }

    private List<MainRowViewModel> _participantsVM;
    public List<MainRowViewModel> ParticipantsVM {
        get => _participantsVM;
        private set => SetProperty(ref _participantsVM, value);
    }

    public void AskEditMode() {
        ParticipantsVM.ForEach(vm => vm.Changes());
    }

    public bool IsCreator => IsAdmin || CurrentUser == Poll.Creator;

    private bool _isClosed;
    public bool IsClosed {
        get => _isClosed;
        set => SetProperty(ref _isClosed, value);
    }

    private bool _canEdit;
    public bool CanEdit {
        get => _canEdit;
        set => SetProperty(ref _canEdit, value);
    }

    private bool _writingMode;
    public bool WritingMode {
        get => _writingMode;
        set => SetProperty(ref _writingMode, value);
    }

    private bool _canComment;
    public bool CanComment {
        get => _canComment;
        set => SetProperty(ref _canComment, value);
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
        CanComment = !IsClosed;
        Comment = "";
    }

    private bool _editPollMode;

    private bool _showGrid;
    public bool ShowGrid {
        get => _showGrid;
        set => SetProperty(ref _showGrid, value);
    }

    protected sealed override void OnRefreshData() {
        if (_isNew) return;

        Poll = Context.Polls.Find(Poll.Id);
        IsClosed = Poll.IsClosed;
        CanComment = !IsClosed;
        Comments = new ObservableCollection<Comment>(Poll.Comments.OrderByDescending(c => c.CreationDate));
        Choices = Poll.Choices.OrderBy(c => c.Label).ToList();
        ShowGrid = !EditPollMode && !NoParticipant && !NoChoice;
        GetParticipants();
        ParticipantsVM = Participants
            .Select(p => new MainRowViewModel(this, Poll, p.User)).ToList();

        /* -------------------------- partie Add/Edit du refresh -------------------------- */

        GetAddables();
        GetEditChoices();

        EditTitle = Poll.Title;
        NotifyColleagues(App.Messages.MSG_RENAME_TAB, Poll);
        EditType = Poll.Type == PollType.Multiple ? 0 : 1;
        NewChoice = "";

        NoChoice = Poll.Choices.Count == 0;
        NoParticipant = Poll.Participants.Count == 0;
        IsChecked = Poll.Status == PollStatus.Closed;
        RaisePropertyChanged(nameof(CanBeSingle));
        RaisePropertyChanged();

        AskEditMode();
    }

    /* -------------------------- partie Add/Edit des attributs -------------------------- */

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteParticipantCommand { get; }
    public ICommand DeleteChoiceCommand { get; }
    public ICommand AddChoiceCommand { get; }
    public ICommand AddParticipantCommand { get; }
    public ICommand AddMySelfCommand { get; }
    public ICommand AddEverybodyCommand { get; }

    private ObservableCollection<dynamic> _participants;
    public ObservableCollection<dynamic> Participants {
        get => _participants;
        private set => SetProperty(ref _participants, value);
    }

    private ObservableCollection<User> _addables;
    public ObservableCollection<User> Addables {
        get => _addables;
        private set => SetProperty(ref _addables, value);
    }

    private ObservableCollection<EditChoiceViewModel> _editChoices;
    public ObservableCollection<EditChoiceViewModel> EditChoices {
        get => _editChoices;
        private set => SetProperty(ref _editChoices, value);
    }

    private bool _isChecked;
    public bool IsChecked {
        get => _isChecked;
        set {
            SetProperty(ref _isChecked, value);
            Poll.Status = IsChecked ? PollStatus.Closed : PollStatus.Open;
        }
    }

    private User _added;
    private bool _isNew;
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
        set => SetProperty(ref _newChoice, value);
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

    private bool ValidateTitle() {
        ClearErrors();

        if (EditTitle.IsNullOrEmpty())
            AddError(nameof(EditTitle), "Title required");
        else if (EditTitle.Length < 10)
            AddError(nameof(EditTitle), "Title length must be at least 10 char");
        else if (EditTitle == Poll.NEW_POLL_LABEL)
            AddError(nameof(EditTitle), "Sorry, this name is reserved :/");
        else {
            Poll poll = Context.Polls.FirstOrDefault(p => p.Title == EditTitle);
            if (poll != null && poll.Creator != Poll.Creator)
                AddError(nameof(EditTitle), "This poll already exists, it was created by " + poll.Creator.FullName);
            else
                Poll.Title = EditTitle;
        }

        return !HasErrors;
    }

    private void AddParticipantAction(User user) {
        NoParticipant = false;
        Poll.Participants.Add(user);
        GetParticipants();
        Addables.Remove(user);
    }

    private void GetParticipants() {
        Participants = new ObservableCollection<dynamic>(Poll.Participants.OrderBy(p => p.FullName)
            .Select(user => new {User = user, Nb = GetUserVotesNb(user)}));
    }

    private void GetEditChoices() {
        EditChoices = new ObservableCollection<EditChoiceViewModel>(Poll.Choices.OrderBy(ch => ch.Label)
            .Select(ch => new EditChoiceViewModel(ch, GetChoiceVotesNb(ch))));
    }

    private void GetAddables() {
        Addables = new ObservableCollection<User>(Context.Users.Where(u => !Poll.Participants.Contains(u)).OrderBy(u => u.FullName));
    }

    private int GetUserVotesNb(User user) => Poll.Votes.Count(v => v.User == user);

    private int GetChoiceVotesNb(Choice choice) => Poll.Votes.Count(v => v.Choice == choice);

    public bool CanBeSingle => !Poll.Votes.GroupBy(v => v.User).Any(elem => elem.Count() > 1);

    private bool NoEditingChoice => !EditChoices.Any(vm => vm.EditMode);
}
