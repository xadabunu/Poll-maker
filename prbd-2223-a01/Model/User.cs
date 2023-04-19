using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Castle.Components.DictionaryAdapter;
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
        set => value.Except(Context.Votes).ToList().ForEach(vote => Context.Add(vote));
    }

    public User(int id, string name, string mail, string password) {
        Id = id;
        FullName = name;
        Email = mail;
        Password = password;
    }

    public User() { }

    public void Save() {
        Context.Users.Add(this);
        Context.SaveChanges();
    }
}
