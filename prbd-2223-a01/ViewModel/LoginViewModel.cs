using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using MyPoll.Model;
using PRBD_Framework;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using MyPoll.View;

namespace MyPoll.ViewModel;

public class LoginViewModel : ViewModelCommon {

    public ObservableCollection<User> Users { get; set; }
    public ICommand LoginCommand { get; }
    // public ICommand LogAs { get; }
    public ICommand LogAsJ { get; }
    public ICommand LogAsH { get; }
    public ICommand LogAsA { get; }
    public ICommand SignUp { get; }


    public  string[] Harry = { "harry@test.com", "harry" };
    public string[] John = { "john@test.com", "john" };
    public string[] Admin = { "admin@test.com", "admin" };


    private string _email;
    private string _password = "";

    public string Email {
        get => _email;
        set => SetProperty(ref _email, value, () => Validate());
    }

    public string Password {
        get => _password;
        set => SetProperty(ref _password, value, () => Validate()) ;
    }

    public LoginViewModel() : base() {

        Users = new ObservableCollection<User>(Context.Users);

        LoginCommand = new RelayCommand(() => Login(), () => ValidateFields());
        // LogAs = new RelayCommand<string[]>((cred) => {
        //     Email = cred[0];
        //     Password = cred[1];
        //     Login();
        // });
        LogAsJ = new RelayCommand(() => LogAs(John));
        LogAsH = new RelayCommand(() => LogAs(Harry));
        LogAsA = new RelayCommand(() => LogAs(Admin));
        SignUp = new RelayCommand<WindowBase>((win) => GoToSignUp(win));
    }

    private void LogAs(string[] creds) {
        Email = creds[0];
        Password = creds[1];
        Login();
    }

    protected override void OnRefreshData() {
    }

    private void Login() {
        var user = GetUserByEmail();
        NotifyColleagues(App.Messages.MSG_LOGIN, user);
    }

    private bool ValidateFields() {
        User u = GetUserByEmail();
        // if (u != null) {
        //     return CheckPassword(u, Password);
        // }
        // return false;
        return u != null && CheckPassword(u);
    }

    private User GetUserByEmail() {
        User user = (from u in Users
                    where u.Email == Email
                    select u).SingleOrDefault();
        return user;
    }

    private bool EmailExists() => Users.Any(u => u.Email == Email);

    private bool CheckPassword(User user) => user != null && SecretHasher.Verify(Password, user.Password);

    public override bool Validate() {
        ClearErrors();

        if (Email.IsNullOrEmpty())
            AddError(nameof(Email), "required");
        else if (!Regex.IsMatch(Email, EmailRegex))
            AddError(nameof(Email), "invalid email address");
        else if (!EmailExists())
            AddError(nameof(Email), "unknown email address");
        else {
            if (Password.IsNullOrEmpty())
                AddError(nameof(Password), "required");
            else if (!CheckPassword(GetUserByEmail()))
                AddError(nameof(Password), "wrong password");
        }

        return !HasErrors;
    }

    public void GoToSignUp(WindowBase win) {
        var v = new SignupView();
        v.Show();
        win.Close();
    }
}

