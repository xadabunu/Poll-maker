using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;
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
    public IQueryable<Vote> GetVotes() {

        var q = from v in Context.Votes where v.Choice.PollId == Id select v;
        return q;
    }

    [NotMapped]
    public IQueryable<Vote> Votes =>
        from v in Context.Votes
        where v.Choice.PollId == Id
        select v;

    public int VoteCount {
        get => Votes.Count();
    }

    // public virtual ICollection<Choice> BestChoices {
    //     get => Choices.Where(c => c.Score >= Choices.Max(ch => ch.Score)).ToList();
    // }
    //
    // public string BestChoiceTitle {
    //     get => "Best Choice" + (BestChoices.Count > 1 ? "s" : "");
    // }

    public Poll(int id, string title, User creator, PollStatus? status, PollType? type) {
        Id = id;
        Title = title;
        Type = type ?? PollType.Multiple;
        Status = status ?? PollStatus.Open;
        Creator = creator;
    }

    public Poll() {
    }
}
