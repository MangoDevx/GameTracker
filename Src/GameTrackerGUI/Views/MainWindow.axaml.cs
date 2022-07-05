using System.Reactive;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using GameTrackerGUI.ViewModels;
using ReactiveUI;

namespace GameTrackerGUI.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(x => x(ViewModel!.ShowAddDialog.RegisterHandler(DoShowAddDialogAsync)));
        }

        private async Task DoShowAddDialogAsync(InteractionContext<AddWindowViewModel, Unit> interaction)
        {
            var dialog = new AddWindow
            {
                Content = interaction.Input
            };
            
            var result = await dialog.ShowDialog<Unit>(this);
            interaction.SetOutput(result);
        }
    }
}