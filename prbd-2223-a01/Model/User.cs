using PRBD_Framework;

namespace MyPoll.Model;

public class User : EntityBase<MyPollContext> {
    public int UserId { get; set; }
    public string Name { get; set; }
}
