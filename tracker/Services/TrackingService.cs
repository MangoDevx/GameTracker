namespace tracker.Services;

public class TrackingService
{
    private readonly CancellationToken _ct;

    public TrackingService(CancellationToken ct)
    {
        _ct = ct;
    }

    public async void RunTrackingService()
    {
        await TrackProcesses();
    }

    public async Task TrackProcesses()
    {
        while (true)
        {
            Console.WriteLine("xyz");
            await Task.Delay(1500);
        }
    }
}
