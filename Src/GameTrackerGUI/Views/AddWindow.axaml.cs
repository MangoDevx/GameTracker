using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameTrackerGUI.Views;

public partial class AddWindow : Window
{
    public AddWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}