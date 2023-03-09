using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRBD_Framework;

namespace MyPoll.Model;

public class Comment : EntityBase<MyPollContext> {
    [Key]
    public int Id { get; set; }
    //[Required, ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    [Required]
    public virtual User Author { get; set; }
    //[Required, ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    [Required]
    public virtual Poll Poll { get; set; }
    [Required]
    public string Text { get; set; }
    public DateTime CreationDate { get; set; }

    public Comment(User author, Poll poll, string text, DateTime? creationDate) {
        Author = author;
        Poll = poll;
        Text = text;
        CreationDate = creationDate ?? DateTime.Now;
    }

    public Comment() { }
}
