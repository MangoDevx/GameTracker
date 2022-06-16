using Spectre.Console;

namespace tracker.Services;

public class ConsoleService
{
    private readonly string[] _options =
    {
        "Add Process%Adds a new process to track",
        "Edit Process%Edits a process to track, change the name, filepath, etc",
        "Remove Process%Removes a process to track, may be readded automatically",
        "Blacklist Process%Blacklists a process from being automatically added to tracking"
    };

    public async Task RunConsole()
    {
        Console.WriteLine("Press any key to continue to the next screen...");
        Console.ReadKey();
        Console.Clear();
        Console.WriteLine();
        var table = new Table { Border = TableBorder.Rounded };
        table.Title("[white]Welcome to[/] [springgreen3]GameTracker[/][white]![/]");
        table.BorderColor(Color.DeepSkyBlue3);
        table.AddColumn("Options");
        table.AddColumn("Descriptions");
        for (var i = 0; i < _options.Length; i++)
        {
            var option = _options[i];
            var split = option.Split('%');
            var title = i+1 + ". " + split[0];
            var desc = split[1];
            table.AddRow(title, desc);
        }

        table.Caption("[white]To select one of the [/][deepSkyBlue3]options[/][white], enter it below[/]");
        AnsiConsole.Write(table);
        Console.WriteLine();
        AnsiConsole.Cursor.Show();
        var selectedOption = AnsiConsole.Ask<int>("Which [deepSkyBlue3]option[/] do you want to select (1, 2, 3, 4):");
        AnsiConsole.Cursor.Hide();
        Console.WriteLine($"You picked option {selectedOption}");

    }
}
