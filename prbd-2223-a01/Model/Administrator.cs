namespace MyPoll.Model;

public class Administrator : User {
    public Administrator(int id, string name, string mail, string password) : base(id, name, mail, password) {
    }

    public Administrator() { }
}
