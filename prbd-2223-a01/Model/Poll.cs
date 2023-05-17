using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRBD_Framework;

namespace MyPoll.Model;

public enum PollType {
    Multiple,
    Simple
}

public enum PollStatus {
    Open,
    Closed
}

public class Poll : EntityBase<MyPollContext> {

    public static readonly string ClosedColor = "#FFE6DC";
    public static readonly string UnansweredColor = "#D3D3D3";
    public static readonly string AnsweredColor = "#C4E0C4";
    public static readonly string NEW_POLL_LABEL = "New Poll";

    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required, ForeignKey(nameof(Creator))]
    public int CreatorId { get; set; }
    [Required]
    public virtual User Creator { get; set; }

    public PollType Type { get; set; }
    public PollStatus Status { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public virtual ICollection<User> Participants { get; set; } = new HashSet<User>();
    public virtual ICollection<Choice> Choices { get; set; } = new HashSet<Choice>();

    [NotMapped]
    public IQueryable<Vote> Votes =>
        from v in Context.Votes
        where v.Choice.PollId == Id
        select v;

    public string ParticipantsLabel {
        get => Participants.Count + " participant" + (Participants.Count > 1 ? "s" : "");
    }

    public int VoteCount {
        get => Votes.Count();
    }

    public string VoteCountLabel {
        get => VoteCount + " vote" + (VoteCount > 1 ? "s" : "");
    }

    public bool IsClosed => Status == PollStatus.Closed;
    public bool IsSimple => Type == PollType.Simple;

    [NotMapped]
    public ICollection<Choice> BestChoices {
        get => VoteCount == 0 ? null : Choices.Where(c => c.Score >= Choices.Max(ch => ch.Score)).ToList();
    }

    public string BestChoiceTitle {
        get => "Best Choice" + (BestChoices?.Count > 1 ? "s" : "") + " :";
    }

    private bool HasVoted() {
        return Votes.Any(v => v.User == App.CurrentUser);
    }

    public string BackgroundColor {
        get => Status == PollStatus.Closed ? ClosedColor : !HasVoted() ? UnansweredColor : AnsweredColor;
    }

    public Poll() {
    }
}
