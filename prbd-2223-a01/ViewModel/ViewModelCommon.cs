using PRBD_Framework;
using MyPoll.Model;

namespace MyPoll.ViewModel;

public class ViewModelCommon : ViewModelBase<User, MyPollContext> {
    public static bool IsAdmin => App.IsLoggedIn && App.CurrentUser is Administrator;

    public static bool IsNotAdmin => !IsAdmin;

    public readonly string EmailRegex = @"[a-zA-Z0-9]{1,20}[@]{1}[a-zA-A0-9]{1,15}[.]{1}[a-z]{1,7}";
    //bool EmailExists(string email) => Users.Any(u => u.Email == email);
}
