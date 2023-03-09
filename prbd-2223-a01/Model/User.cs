using System.ComponentModel.DataAnnotations;
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

    public User(int id, string name, string mail, string password) {
        Id = id;
        FullName = name;
        Email = mail;
        Password = password;
    }

    public User() { }
}
