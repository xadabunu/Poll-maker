using System.Windows;
using PRBD_Framework;

namespace MyPoll.View;

public partial class LoginView : WindowBase {
    public LoginView() {
        InitializeComponent();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
}
