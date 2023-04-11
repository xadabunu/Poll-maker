using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollViewModel : ViewModelCommon {

    private ObservableCollection<Poll> _polls;
    public ObservableCollection<Poll> Polls {
        get => _polls;
        set => SetProperty(ref _polls, value, () => { });
    }
    public ICommand ClearFilter { get; set; }
    public ICommand OpenView { get; }

    private string _filter;
    public string Filter {
        get => _filter;
        set => SetProperty(ref _filter, value, ApplyFilterAction);
    }

    public PollViewModel() {
        Polls = new ObservableCollection<Poll>(CurrentUser.Polls.Union(Context.Polls.Where(p => p.Creator == CurrentUser)));

        ClearFilter = new RelayCommand(() => Filter = "");
        OpenView = new RelayCommand(() => Console.WriteLine("open view"));
    }

    private void ApplyFilterAction() {
        Polls = new ObservableCollection<Poll>(CurrentUser.Polls.Union(Context.Polls.Where(p => p.Creator == CurrentUser)));

        if (!Filter.IsNullOrEmpty()) {
            var query =
                from p in Polls
                where p.Title.Contains(Filter) ||
                      p.Participants.Any(u => u.FullName.Contains(Filter)) ||
                      p.Creator.FullName.Contains(Filter) ||
                      p.Choices.Any(c => c.Label.Contains(Filter))
                select p;
            Polls = new ObservableCollection<Poll>(query);
        }
    }
}
