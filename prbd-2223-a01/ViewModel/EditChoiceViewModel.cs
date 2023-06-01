using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class EditChoiceViewModel : ViewModelCommon {
    private readonly int _nb;
    private readonly Choice _choice;

    public ICommand StartEditCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand UpdateChoiceCommand { get; }

    public int Nb => _nb;
    public Choice Choice => _choice;

    public EditChoiceViewModel(Choice choice, int nb) {
        _choice = choice;
        _nb = nb;
        Content = choice.Label + " (" + nb + ")";
        EditMode = false;

        StartEditCommand = new RelayCommand(() => {
            EditMode = true;
            RaisePropertyChanged(nameof(BorderBrush));
            Content = Choice.Label;
        });

        UpdateChoiceCommand = new RelayCommand(() => {
            Choice.Label = Content;
            EditMode = false;
            RaisePropertyChanged(nameof(BorderBrush));
            Content = Choice.Label + " (" + nb + ")";
        }, () => !HasErrors);

        CancelEditCommand = new RelayCommand(() => {
            ClearErrors();
            EditMode = false;
            RaisePropertyChanged(nameof(BorderBrush));
            Content = Choice.Label + " (" + nb + ")";
        });
    }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    private string _content;
    public string Content {
        get => _content;
        set => SetProperty(ref _content, value, () => {
            ClearErrors();
            if (Content.IsNullOrEmpty())
                AddError(nameof(Content), "Choice cannot be empty");
        });
    }

    public string BorderBrush => EditMode ? "LightBlue" : "Transparent";
}
