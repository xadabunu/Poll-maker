using System.Windows;
using PRBD_Framework;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
}
