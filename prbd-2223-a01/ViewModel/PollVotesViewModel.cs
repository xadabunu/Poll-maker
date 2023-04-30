using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollVotesViewModel : ViewModelCommon {

    public ICommand EditPollCommand { get; }
    public ICommand AddCommentCommand { get; }
    public ICommand PostCommand { get; }
    public ICommand DeleteCommentCommand { get; }
    public ObservableCollection<Comment> Comments { get; set; }

    public PollVotesViewModel(Poll poll) {
        Poll = poll;

        var participants = poll.Participants.OrderBy(p => p.FullName);

        _participantsVM = participants.Select(p => new MainRowViewModel(this, p, poll.Choices.ToList())).ToList();
        EditPollCommand = new RelayCommand(() => {
            EditPollMode = true;
            ShowGrid = false;
        });
        AddCommentCommand = new RelayCommand(() => WritingMode = true);
        PostCommand = new RelayCommand(() => PostAction(), () => !Comment.IsNullOrEmpty());

        Comments = new ObservableCollection<Comment>(Context.Comments
            .Where(c => c.Poll == poll)
            .OrderByDescending(c => c.CreationDate));

        DeleteCommentCommand = new RelayCommand<Comment>(comment => {
            Context.Comments.Remove(comment);
            Context.SaveChanges();
            Comments.Remove(comment);
        });

        Console.WriteLine("Edit =============>>>> " + EditPollMode);
        Console.WriteLine("Choice =============>>>> " + NoChoice);
        Console.WriteLine("Part =============>>>> " + NoParticipant);

        /* ------------------ Add/Edit part ------------------ */

        EditTitle = Poll.Title;
        EditType = Poll.Type == PollType.Multiple ? 0 : 1;
        NoChoice = Poll.Choices.Count == 0;
        NoParticipant = Poll.Participants.Count == 0;
        ShowGrid = !EditPollMode && !NoChoice && !NoParticipant;
        CancelCommand = new RelayCommand(() => {
            EditPollMode = false;
            ShowGrid = !NoParticipant && !NoChoice;
        });
        Participants = new ObservableCollection<User>(Poll.Participants);
        Addables = new ObservableCollection<User>(
            Context.Users.Where(u => !Participants.Contains(u)).OrderBy(u => u.FullName));
        DeleteParticipantCommand = new RelayCommand<User>(u => {
            Participants.Remove(u);
            NoParticipant = Participants.Count == 0;
            Addables.Add(u);
        });
        EditChoices = new ObservableCollection<Choice>(Poll.Choices);
        EditChoiceCommand = new RelayCommand<Choice>(choice => {
            NewChoice = choice.Label;
            EditChoices.Remove(choice);
        });
        DeleteChoiceCommand = new RelayCommand<Choice>(choice => {
            EditChoices.Remove(choice);
            NoChoice = EditChoices.Count == 0;
        });
        AddParticipantCommand = new RelayCommand<User>(user => {
            NoParticipant = false;
            Addables.Remove(user);
            Participants.Add(user);
        });
        AddChoiceCommand = new RelayCommand(() => {
            NoChoice = false;
            Choice c = new Choice { PollId = Poll.Id, Label = NewChoice };
            EditChoices.Add(c);
            NewChoice = "";
        }, ValidateChoice);
    }

    public List<Choice> Choices => Poll.Choices.OrderBy(c => c.Label).ToList();

    public Poll Poll { get; set; }

    private List<MainRowViewModel> _participantsVM;
    public List<MainRowViewModel> ParticipantsVM => _participantsVM;

    public void AskEditMode(bool editMode) {
        ParticipantsVM.ForEach(vm => vm.Changes());
    }

    public bool IsCreator => CurrentUser == Poll.Creator;

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
        Comment c = new Comment {
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

    /* -------------------------- Add/Edit -------------------------- */

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteParticipantCommand { get; }
    public ICommand EditChoiceCommand { get; }
    public ICommand DeleteChoiceCommand { get; }
    public ICommand AddChoiceCommand { get; }
    public ICommand AddParticipantCommand { get; }
    public ObservableCollection<User> Participants { get; set; }
    public ObservableCollection<User> Addables { get; set; }
    public ObservableCollection<Choice> EditChoices { get; }
    private User _added;

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
        set => SetProperty(ref _editType, value);
    }

    public User Added {
        get => _added;
        set => SetProperty(ref _added, value);
    }

    private string _newChoice;
    public string NewChoice {
        get => _newChoice;
        set => SetProperty(ref _newChoice, value, () => ValidateChoice());
    }

    public bool EditPollMode {
        get => _editPollMode;
        set => SetProperty(ref _editPollMode, value);
    }

    private string _editTitle;
    public string EditTitle {
        get => _editTitle;
        set => SetProperty(ref _editTitle, value, () => Validate());
    }

    public override bool Validate() {
        if (EditTitle.IsNullOrEmpty())
            AddError(nameof(EditTitle), "Title required");
        else if (EditTitle.Length < 7)
            AddError(nameof(EditTitle), "Title length must be at least 7 char");
        return !HasErrors;
    }

    private bool ValidateChoice() {
        bool b = !NewChoice.IsNullOrEmpty() && NewChoice.Length > 2;
        if (!NewChoice.IsNullOrEmpty() && NewChoice.Length < 3) {
            AddError(nameof(NewChoice), "Label of choice must be >= 3 characters long");
        }

        return b;
    }
}
