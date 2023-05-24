using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRBD_Framework;

namespace MyPoll.Model;

public class User : EntityBase<MyPollContext> {
    public int Id { get; set; }
    [Required]
    public string FullName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public virtual ICollection<Poll> Polls { get; set; } = new HashSet<Poll>();

    [NotMapped]
    public ICollection<Vote> Votes {
        get => Context.Votes.Where(v => v.User == this).ToList();
    }

    public User() { }

    public void Save() {
        Context.Users.Add(this);
        Context.SaveChanges();
    }
}
