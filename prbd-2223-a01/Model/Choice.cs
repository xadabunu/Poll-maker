using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPoll.Model;

public class Choice : EntityBase<MyPollContext> {
    public int Id { get; set; }
    [Required, ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    public virtual Poll Poll { get; set; }
    [Required]
    public string Label { get; set; }

    public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();

    public Choice() {}
    public Choice (int id, int pollId, string label) {
        Id = id;
        PollId = pollId;
        Label = label;
    }

    public double Score {
        get {
            double score = 0;
            foreach (var v in Votes) {
                score += (double)v.Value / 2;
            }
            return score;
        }
    }

    public string LabelAndScore {
        get => Label + " (" + Score + ")";
    }
}
