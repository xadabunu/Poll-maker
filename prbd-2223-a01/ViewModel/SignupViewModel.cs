using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class SignupViewModel : ViewModelCommon {

    public ObservableCollection<User> Users { get; }
    public ICommand SignUp { get; }
    public ICommand CancelCommand { get; }

    private string _email;
    public string Email {
        get => _email;
        set => SetProperty(ref _email, value, () => Validate());
    }

    private string _password;
    public string Password {
        get => _password;
        set => SetProperty(ref _password, value, () => Validate());
    }

    private string _confPassword;
    public string ConfPassword {
        get => _confPassword;
        set => SetProperty(ref _confPassword, value, () => Validate());
    }

    private string _fullName;
    public string FullName {
        get => _fullName;
        set => SetProperty(ref _fullName, value, () => Validate());
    }

    public SignupViewModel() {

        Users = new ObservableCollection<User>(Context.Users);

        SignUp = new RelayCommand(() => Login(), () => Validate());
        CancelCommand = new RelayCommand<WindowBase>((win) => Cancel(win));
    }

    private void Login() {
        Console.WriteLine("OK!");
    }

    private void Cancel(WindowBase win) {
        var w = new LoginView();
        win.Close();
        w.Show();
    }

    public override bool Validate() {
        ClearErrors();

        CheckEmail();
        CheckPassword();
        CheckConfPassword();
        CheckFullName();

        return !HasErrors;
    }

    private bool EmailExists() => Users.Any(u => u.Email == Email);

    private void CheckEmail() {
        if (Email.IsNullOrEmpty())
            AddError(nameof(Email), "required");
        else if (!Regex.IsMatch(Email, EmailRegex))
            AddError(nameof(Email), "invalid email address");
        else if (EmailExists())
            AddError(nameof(Email), "unavailable email address");
    }

    private void CheckPassword() {
        if (Password.IsNullOrEmpty())
            AddError(nameof(Password), "required");
        else if (Password.Length < 3)
            AddError(nameof(Password), "length must be >= 3");
    }

    private void CheckConfPassword() {
        if (Password.IsNullOrEmpty())
            AddError(nameof(ConfPassword), "required");
        else if (Password != ConfPassword)
            AddError(nameof(ConfPassword), "must match password");
    }

    private void CheckFullName() {
        if (FullName.IsNullOrEmpty())
            AddError(nameof(FullName), "required");
        else if (FullName.Length < 3)
            AddError(nameof(FullName), "length must be >= 3");
    }
}
