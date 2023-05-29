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
                Title = Poll.NEW_POLL_LABEL,
                Creator = App.CurrentUser
            }));

        Register<Poll>(App.Messages.MSG_POLL_DELETED, poll => {
            tabControl.CloseByTag(poll.Id.ToString());
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        });

        Register<Poll>(App.Messages.MSG_RENAME_TAB, poll => {
            MyTabControl.RenameHeader(tabControl.FindByTag(poll.Id.ToString()), poll.Title);
        });

        // sert uniquement dans le cas d'un nouveau poll
        Register<Poll>(App.Messages.MSG_RETAG_TAB, poll => {
            MyTabControl.RenameTag(tabControl.FindByTag("0"), poll.Id.ToString());
        });
    }

    private void OpenPollVotesTab(Poll poll) {
        OpenTab(poll.Title, poll.Id.ToString(), () => new PollVotesView(poll));
    }

    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null)
            tabControl.Add(createView(), header, tag);
        else
            tabControl.SetFocus(tab);
    }
}

