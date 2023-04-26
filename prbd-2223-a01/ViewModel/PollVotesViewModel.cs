using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollVotesViewModel : ViewModelCommon {

    public ICommand EditPollCommand { get; }
    public ICommand AddCommentCommand { get; }
    public ICommand PostCommand { get; }

    public PollVotesViewModel(Poll poll) {
        Poll = poll;

        var participants = poll.Participants.OrderBy(p => p.FullName);

        _participantsVM = participants.Select(p => new MainRowViewModel(this, p, poll.Choices.ToList())).ToList();
        EditPollCommand = new RelayCommand(EditPollAction);
        AddCommentCommand = new RelayCommand(() => WritingMode = true);
        PostCommand = new RelayCommand(() => PostAction(), () => !Comment.IsNullOrEmpty());
    }

    public List<Choice> Choices => Poll.Choices.OrderBy(c => c.Label).ToList();

    public Poll Poll { get; set; }

    private List<MainRowViewModel> _participantsVM;
    public List<MainRowViewModel> ParticipantsVM => _participantsVM;

    public void AskEditMode(bool editMode) {
        ParticipantsVM.ForEach(vm => vm.Changes());
    }

    public bool IsCreator => CurrentUser == Poll.Creator;

    private bool _writingMode = false;
    public bool WritingMode {
        get => _writingMode;
        set => SetProperty(ref _writingMode, value);
    }

    private string _comment;
    public string Comment {
        get => _comment;
        set => SetProperty(ref _comment, value);
    }

    private void RefreshComments() {
        Poll.Comments = Context.Comments
            .Where(c => c.Poll == Poll)
            .OrderByDescending(c => c.CreationDate)
            .ToList();
    }

    private void EditPollAction() {

    }
    private void PostAction() {
        WritingMode = false;
        Poll.Comments.Add(new Comment
        {
            AuthorId = CurrentUser.Id,
            PollId = Poll.Id,
            Text = Comment,
            CreationDate = DateTime.Now
        });
        Context.SaveChanges();
        RefreshComments();
        Comment = "";
    }
}
