namespace tracker.Services;

public class TrackingService
{

    public async void RunTrackingService()
    {
        await TrackProcesses();
    }

    private async Task TrackProcesses()
    {
        while (true)
        {
            Console.WriteLine("xyz");
            await Task.Delay(1500);
        }
    }
}
