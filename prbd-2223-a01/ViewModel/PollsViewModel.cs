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
        set => SetProperty(ref _polls, value);
    }
    public ICommand ClearFilter { get; }
    public ICommand OpenView { get; }

    private string _filter;
    public string Filter {
        get => _filter;
        set => SetProperty(ref _filter, value, OnRefreshData);
    }

    public PollViewModel() {
        OnRefreshData();
        ClearFilter = new RelayCommand(() => Filter = "");
        OpenView = new RelayCommand<Poll>(poll =>
            NotifyColleagues(App.Messages.MSG_POLL_SELECTED, poll));
    }

    protected sealed override void OnRefreshData() {
        Polls = new ObservableCollection<Poll>(Context.Polls
            .Where(p => p.Participants.Any(part => part.Id == CurrentUser.Id))
            .Union(Context.Polls.Where(p => p.Creator == CurrentUser))
            .OrderBy(p => p.Title));

        if (Filter.IsNullOrEmpty()) return;
        var query =
            from p in Polls
            where p.Title.Contains(Filter) ||
                  p.Participants.Any(u => u.FullName.Contains(Filter)) ||
                  p.Creator.FullName.Contains(Filter) ||
                  p.Choices.Any(c => c.Label.Contains(Filter))
            orderby p.Title
            select p;
        Polls = new ObservableCollection<Poll>(query);
    }
}
