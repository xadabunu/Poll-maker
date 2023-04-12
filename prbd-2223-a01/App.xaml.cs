using System.Windows;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll;

public partial class App : ApplicationBase<User, MyPollContext> {

    public enum Messages {
        MSG_SIGNUP,
        MSG_LOGIN,
        MSG_POLL_SELECTED
    }

    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();
        Register<User>(this, Messages.MSG_LOGIN, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });
        Register(this, Messages.MSG_SIGNUP, () =>
            NavigateTo<SignupViewModel, User, MyPollContext>());
    }
    private static void PrepareDatabase() {
        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        // Cold start
        Console.Write("Cold starting database... ");
        Context.Users.Find(0);
        Console.WriteLine("done");

        /*
        //var q = from u in Context.Users
        //        select u;

        //var q = Context.Users.Where(u => u.Comments.Any(c => c.Poll.Creator.FullName == "Harry Covère"));

        //var q = Context.Polls.ToList().Select(p => new { p.Id, Count = p.Votes.Count() });

        //var q1 = from p in Context.Polls
        //         select new { p.Id, Count = (from v in Context.Votes where v.Choice.PollId == p.Id select v).Count() };


        //foreach (var v in q) {
        //    Console.WriteLine(v);
        //}
        */
    }
}
