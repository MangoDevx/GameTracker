using Spectre.Console;
using tracker.Database;
using tracker.Database.DbModels;

namespace tracker.Services;

public class ConsoleService
{
    private readonly string[] _options =
    {
        "Add Process%Adds a new process to track",
        "Edit Process%Edits a process to track, change the name, filepath, etc",
        "Delete Process%Removes a process to track, may be readded automatically",
        "List Processes%Lists the processes that will be tracked when they are running",
        "Blacklist Process%Blacklists a process from being automatically added to tracking",
        "Hide App%Hides the application in the background. To close it end tracker.exe in Task Manager"
    };

    private readonly DataContext _context;

    public ConsoleService(DataContext context)
    {
        _context = context;
    }

    public async Task RunConsole()
    {
        Console.WriteLine("Press any key to continue to the next screen...");
        Console.ReadKey();
        await MainScreen();
    }

    private async Task MainScreen()
    {
        while (true)
        {
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
                var title = i + 1 + ". " + split[0];
                var desc = split[1];
                table.AddRow(title, desc);
            }

            table.Caption("[white]To select one of the [/][deepSkyBlue3]options[/][white], enter it below[/]");
            AnsiConsole.Write(table);
            Console.WriteLine();

            var optionHint = "(";
            for (var i = 0; i < _options.Length - 1; i++)
                optionHint += i + 1 + ", ";
            optionHint += $"{_options.Length})";

            int selectedOption;
            while (true)
            {
                AnsiConsole.Cursor.Show();
                selectedOption = AnsiConsole.Ask<int>($"Which [deepSkyBlue3]option[/] do you want to select {optionHint}:");
                if (selectedOption < 1 || selectedOption > _options.Length)
                    AnsiConsole.Markup("[red]Invalid input[/]\n");
                else
                    break;
            }

            switch (selectedOption)
            {
                case 1:
                    await AddProcess();
                    break;
                case 2:
                    EditProcess();
                    break;
                case 3:
                    DeleteProcess();
                    break;
                case 4:
                    ListProcesses();
                    break;
                case 5:
                    BlacklistProcess();
                    break;
                case 6:
                    HideApp();
                    break;
            }
        }
    }

    private async Task AddProcess()
    {
        Console.Clear();
        Console.WriteLine();
        AnsiConsole.Write(new Rule("[deepSkyBlue3]Add Process[/]"));
        Console.WriteLine();
        while (true)
        {
            var inputPath = AnsiConsole.Ask<string>("Please input the [deepSkyBlue3]path[/] to the game/app or [red]back[/] to go back: ");
            if (inputPath.ToLowerInvariant() == "back")
                break;

            if (!File.Exists(inputPath))
            {
                AnsiConsole.Markup("[red]Invalid input.[/] Please make sure you input the full path, including the application.\n\n");
                continue;
            }

            var gameName = inputPath.Split('\\').Last();
            if (string.IsNullOrEmpty(gameName))
                gameName = inputPath.Split('/').Last();
            if (string.IsNullOrEmpty(gameName))
                gameName = inputPath;

            if (_context.Processes.Any(x => x.Name == gameName))
            {
                AnsiConsole.Markup("[red]This application is already in the list[/]\n");
            }
            else
            {
                _context.Processes.Add(new TrackedProcess { Name = gameName, Path = inputPath, HoursRan = 0, LastAccessed = DateTime.UtcNow.ToString("o"), Tracking = true });
                await _context.SaveChangesAsync();
                AnsiConsole.Write(new Markup($"Successfully added [springgreen3]{gameName}[/] to the list.\n"));
            }

            var inputAgain = AnsiConsole.Ask<string>("Do you want to input another path? ([springgreen3]y[/]/[red]n[/]): ");
            if (inputAgain.ToLowerInvariant() == "n")
                break;
        }
    }

    private void EditProcess()
    {

    }

    private void DeleteProcess()
    {

    }

    private void ListProcesses()
    {

    }

    private void BlacklistProcess()
    {

    }

    private void HideApp()
    {

    }
}
