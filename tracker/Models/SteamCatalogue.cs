namespace tracker.Models;

public class SteamCatalogue
{
    public Applist? applist { get; set; }
}

public class Applist
{
    public App?[] apps { get; set; }
}

public class App
{
    public long appid { get; set; }
    public string name { get; set; }
}

