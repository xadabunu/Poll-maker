using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using MyPoll.Model;
using PRBD_Framework;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;

namespace MyPoll.ViewModel;

public class MainViewModel : ViewModelBase<User, MyPollContext> {

    public ObservableCollection<User> Users { get; set; }
    public ICommand LoginCommand { get; set; }
    // public ICommand LogAs { get; }
    public ICommand LogAsJ { get; }
    public ICommand LogAsH { get; }
    public ICommand LogAsA { get; }


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

    public MainViewModel() : base() {

        Users = new ObservableCollection<User>(Context.Users.OrderBy(p => p.FullName));

        LoginCommand = new RelayCommand(() => Login(), () => ValidateFields());
        // LogAs = new RelayCommand<string[]>((cred) => {
        //     Email = cred[0];
        //     Password = cred[1];
        //     Login();
        // });
        LogAsJ = new RelayCommand(() => LogAs(John));
        LogAsH = new RelayCommand(() => LogAs(Harry));
        LogAsA = new RelayCommand(() => LogAs(Admin));
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
        Console.WriteLine(user.FullName + " connected");
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
        //return Users.Any(u => u.Email == email);
    }

    private bool EmailExists() => Users.Any(u => u.Email == Email);

    private bool CheckPassword(User user) => user != null && SecretHasher.Verify(Password, user.Password);

    public override bool Validate() {
        ClearErrors();

        string regex = @"[a-zA-Z0-9]{1,20}[@]{1}[a-zA-A0-9]{1,15}[.]{1}[a-z]{1,7}";

        if (Email.IsNullOrEmpty())
            AddError(nameof(Email), "required");
        else if (!Regex.IsMatch(Email, regex))
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
}

