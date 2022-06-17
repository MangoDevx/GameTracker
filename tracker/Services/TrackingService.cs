using Microsoft.Extensions.Hosting;

namespace tracker.Services;

public class TrackingService
{

    public async Task TrackProcesses()
    {
        while (true)
        {
            Console.WriteLine("HEY!");
            await Task.Delay(1000);
        }
    }
}
