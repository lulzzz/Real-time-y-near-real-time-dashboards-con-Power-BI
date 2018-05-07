using Microsoft.Azure.EventHubs;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace ConsoleEventHubSender
{
    class Program
    {
        public static string eventHubConnString = ConfigurationManager.AppSettings["eventHubNameSpaceConnectionString"];
        public static string eventHubEntityPath = ConfigurationManager.AppSettings["eventHubName"];
        public static EventHubClient client;
        static void Main(string[] args)
        {
            PrepareEventHub(args).GetAwaiter().GetResult();
        }

        private static async Task PrepareEventHub(string[] args)
        {
            await SendMessages(6000);
        }

        private static async Task SendMessages(int number)
        {
            #region Builder
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(eventHubConnString)
            {
                EntityPath = eventHubEntityPath
            };
            Random ran = new Random();
            client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            #endregion

            for (int i = 0; i<number; i++)
            {
                #region Latitud, longitud
                double lat = ran.NextDouble() - 11;
                double lon = ran.NextDouble() - 79;
                string latString = lat.ToString();
                latString = latString.Replace(',', '.');
                latString = latString.Substring(0, 7);
                string lonString = lon.ToString();
                lonString = lonString.Replace(',', '.');
                lonString = lonString.Substring(0, 7);
                #endregion
                try
                {
                    var message = $"{{" +
                        $"\"IdentidadBarco\": {ran.Next(1, 19)}," +
                        $"\"TemperaturaBodega1\": {ran.Next(15, 20)}," +
                        $"\"TemperaturaBodega2\": {ran.Next(16, 20)}," +
                        $"\"TemperaturaBodega3\": {ran.Next(14, 20)}," +
                        $"\"TemperaturaBodega4\": {ran.Next(13, 20)}," +
                        $"\"VelocidadBarco\": {ran.Next(80,120)}," +
                        $"\"Latitud\": {latString}," +
                        $"\"Longitud\": {lonString}," +
                        $"\"Hora\": {DateTime.UtcNow.Hour}," +
                        $"\"Minuto\": {DateTime.UtcNow.Minute}," +
                        $"\"Segundo\": {DateTime.UtcNow.Second}," +
                        $"\"Tiempo\": \"{DateTime.UtcNow.TimeOfDay}\"" +
                        $"}}";
                    await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} - Exception: {ex.ToString()}");
                }
                await Task.Delay(1000);
            }
            await client.CloseAsync();
        }
    }
}
