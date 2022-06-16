using System.Text.Json;
using System.Text.RegularExpressions;
using tracker.Models;

namespace tracker.VdfReader;

public static class VdfReader
{
    public static async Task<IEnumerable<VdfData?>> ReadVdfDataAsync(string filePath)
    {
        var rawVdf = await File.ReadAllTextAsync(filePath);

        HashSet<string> appSections = new();
        var appMatches = MatchAppSection.Matches(rawVdf);
        foreach (Match match in appMatches)
            appSections.Add(match.Value);

        HashSet<string> jsonAppSections = new();
        foreach (var value in appSections)
        {
            var newValue = value.Replace("apps\"", "apps\":");
            var matches = MatchAppSpaces.Matches(newValue);
            foreach (Match match in matches)
                newValue = newValue.Replace(match.Value, "\": \"");

            var quoteSections = MatchAppQuotations.Matches(newValue);
            for (var i = 0; i < quoteSections.Count - 1; i++)
                newValue = newValue.Replace(quoteSections[i].Value, quoteSections[i].Value + ",");

            jsonAppSections.Add(newValue);
        }

        var returnArray = new VdfData?[jsonAppSections.Count];
        for (var i = 0; i < jsonAppSections.Count; i++)
            returnArray[i] = JsonSerializer.Deserialize<VdfData>("{" + jsonAppSections.ElementAt(i) + "}");

        return returnArray;
    }

    private static readonly Regex MatchAppSection = new Regex(@"\""apps\""[\s\r\n]*{([^{}]*)}", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex MatchAppSpaces = new Regex(@"\""	+\""", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex MatchAppQuotations = new Regex(@""".*""$", RegexOptions.Compiled | RegexOptions.Multiline);
}