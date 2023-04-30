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


    private readonly string[] _harry = { "harry@test.com", "harry" };
    private readonly string[] _john = { "john@test.com", "john" };
    private readonly string[] _admin = { "admin@test.com", "admin" };


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
        LogAsJ = new RelayCommand(() => LogAs(_john));
        LogAsH = new RelayCommand(() => LogAs(_harry));
        LogAsA = new RelayCommand(() => LogAs(_admin));
        SignUp = new RelayCommand(() => NotifyColleagues(App.Messages.MSG_SIGNUP));
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
        return u != null && CheckPassword(u);
    }

    private User GetUserByEmail() {
        User user = (from u in Users
                    where u.Email == Email
                    select u).SingleOrDefault();
        return user;
    }

    private bool CheckPassword(User user) => user != null && SecretHasher.Verify(Password, user.Password);

    public override bool Validate() {
        ClearErrors();
        User u = GetUserByEmail();

        if (Email.IsNullOrEmpty())
            AddError(nameof(Email), "required");
        else if (!Regex.IsMatch(Email, EmailRegex))
            AddError(nameof(Email), "invalid email address");
        else if (u == null)
            AddError(nameof(Email), "unknown email address");
        else {
            if (Password.IsNullOrEmpty())
                AddError(nameof(Password), "required");
            else if (!CheckPassword(GetUserByEmail()))
                AddError(nameof(Password), "wrong password");
        }

        return !HasErrors;
    }
}

