using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();

        Register<Poll>(App.Messages.MSG_POLL_SELECTED,
            poll => OpenPollVotesTab(poll));

        Register(App.Messages.MSG_NEW_POLL,
            () => OpenPollVotesTab(new Poll {
                Title = App.NEW_POLL_LABEL,
                Creator = App.CurrentUser
            }));

        Register(App.Messages.MSG_NEW_POLL_CANCEL,
            () => {
                tabControl.CloseByTag(App.NEW_POLL_LABEL);
            });
    }

    private void OpenPollVotesTab(Poll poll) {
        OpenTab(poll.Title, poll.Title, () => new PollVotesView(poll));
    }

    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null)
            tabControl.Add(createView(), header, tag);
        else
            tabControl.SetFocus(tab);
    }
}

