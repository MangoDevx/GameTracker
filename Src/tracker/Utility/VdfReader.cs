using System.Text.RegularExpressions;

namespace tracker.VdfReader;

public static class VdfReader
{
    public static async Task<IEnumerable<string>> ReadVdfLibraryPathsAsync(string filePath)
    {
        var rawVdf = await File.ReadAllTextAsync(filePath);
        var matches = MatchLibraryPaths.Matches(rawVdf);
        var libraryPaths = new List<string>();
        foreach (Match match in matches)
        {
            var split = match.Value.Split('"');
            libraryPaths.Add(split[3].Replace(@"\\", "/"));
        }

        return libraryPaths;
    }

    private static readonly Regex MatchLibraryPaths = new Regex(@"\""path\""[\s\r\n]*\""([^\""\""]*)\""", RegexOptions.Compiled | RegexOptions.Singleline);
}