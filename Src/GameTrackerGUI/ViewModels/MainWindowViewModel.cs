using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;

namespace GameTrackerGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand AddGameCommand { get; set; }
        public Interaction<AddWindowViewModel, Unit> ShowAddDialog { get; }

        public MainWindowViewModel()
        {
            ShowAddDialog = new Interaction<AddWindowViewModel, Unit>();
            AddGameCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var addWindowView = new AddWindowViewModel();
                await ShowAddDialog.Handle(addWindowView);
            });
        }
    }
}