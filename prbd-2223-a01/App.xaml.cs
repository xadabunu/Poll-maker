using System.Windows;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll;

public partial class App : ApplicationBase<User, MyPollContext> {

    public enum Messages {
        MSG_SIGNUP,
        MSG_LOGIN,
        MSG_POLL_SELECTED,
        MSG_CANCEL_SIGNUP,
        MSG_LOGOUT,
        MSG_NEW_POLL,
        MSG_POLL_DELETED
    }

    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();

        Register<User>(this, Messages.MSG_LOGIN, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });

        Register(this, Messages.MSG_SIGNUP, () =>
            NavigateTo<SignupViewModel, User, MyPollContext>());

        Register(this, Messages.MSG_CANCEL_SIGNUP, () =>
            NavigateTo<LoginViewModel, User, MyPollContext>());

        Register(this, Messages.MSG_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, MyPollContext>();
        });
    }
    private static void PrepareDatabase() {
        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        // Cold start
        Console.Write("Cold starting database... ");
        Context.Users.Find(0);
        Console.WriteLine("done");
    }

    protected override void OnRefreshData() {
        CurrentUser = Context.Users.Find(CurrentUser.Id);
    }
}
