using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PRBD_Framework;

namespace MyPoll.Model;

public enum VoteValue {
    No,
    Maybe,
    Yes
}

public class Vote : EntityBase<MyPollContext> {

    //[ForeignKey(nameof(User))]
    public int UserId { get; set; }
    [Required]
    public virtual User User { get; set; }

    //[ForeignKey(nameof(Choice))]
    public int ChoiceId { get; set; }
    [Required]
    public virtual Choice Choice { get; set; }

    public Poll GetPoll() {
        return Choice.Poll;
    }

    [Required]
    public VoteValue Value { get; set; }

    public Vote() { }
    public Vote(int userId, int choiceId, VoteValue value) {
        UserId = userId;
        ChoiceId = choiceId;
        Value = value;
    }
}
