using MyPoll.Model;

namespace MyPoll.ViewModel;

public class EditChoiceViewModel : ViewModelCommon {
    private readonly int _nb;
    private readonly Choice _choice;

    public int Nb => _nb;
    public Choice Choice => _choice;

    public EditChoiceViewModel(Choice choice, int nb) {
        _choice = choice;
        _nb = nb;
    }
}
