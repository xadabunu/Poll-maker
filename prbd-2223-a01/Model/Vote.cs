using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PRBD_Framework;

namespace MyPoll.Model;

public enum VoteValue {
    No = -2,
    Maybe = 1,
    Yes = 2,
    None = 0
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
}
