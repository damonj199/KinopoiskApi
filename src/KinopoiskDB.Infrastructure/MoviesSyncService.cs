//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//namespace KinopoiskDB.Infrastructure;

//public class MoviesSyncService : BackgroundService
//{
//    private readonly IServiceProvider _serviceProvider;

//    public MoviesSyncService(IServiceProvider serviceProvider)
//    {
//        _serviceProvider = serviceProvider;
//    }
//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                //var movieService = scope.ServiceProvider.GetRequiredService<>();
//            }
//        }
//    }
//}
