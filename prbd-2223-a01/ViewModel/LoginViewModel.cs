using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using MyPoll.Model;
using PRBD_Framework;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;

namespace MyPoll.ViewModel;

public class MainViewModel : ViewModelBase<User, MyPollContext> {

    public ObservableCollection<User> Users { get; set; }
    public bool EnableButton { get; set; }
    public ICommand CheckEnableButton { get; set; }
    public ICommand LoginCommand { get; set; }

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

        EnableButton = true;
        Users = new ObservableCollection<User>(Context.Users.OrderBy(p => p.FullName));

        CheckEnableButton = new RelayCommand(() => EnableButton = CheckPassword(GetUserByEmail())); ;
        LoginCommand = new RelayCommand(() => Login(), () => ValidateFields());
    }

    protected override void OnRefreshData() {
    }

    private void Login() {
        Console.WriteLine("user connected");
    }

    private bool ValidateFields() {
        User u = GetUserByEmail();
        // if (u != null) {
        //     return CheckPassword(u, Password);
        // }
        // return false;
        return u != null && CheckPassword(u);
    }

    public string Title { get; } = "List of all users";
    private User GetUserByEmail() {
        User user = (from u in Users
                    where u.Email == Email
                    select u).SingleOrDefault();
        return user;
        //return Users.Any(u => u.Email == email);
    }

    private bool EmailExists() => Users.Any(u => u.Email == Email);

    private bool CheckPassword(User user) {
        if (user != null) {
            return SecretHasher.Verify(Password, user.Password);
        }
        return false;
    }

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

