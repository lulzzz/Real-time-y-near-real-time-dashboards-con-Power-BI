using Microsoft.Azure.EventHubs;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace ConsoleEventHubSender
{
    class Program
    {
        public static int eventsToSend;
        public static int milisecondsBetweenEvents;
        public static string eventHubConnString = ConfigurationManager.AppSettings["eventHubNameSpaceConnectionString"];
        public static string eventHubEntityPath = ConfigurationManager.AppSettings["eventHubName"];
        public static EventHubClient client;
        static void Main(string[] args)
        {
            milisecondsBetweenEvents = 1000;
            eventsToSend = 20;
            SendEvents().GetAwaiter().GetResult();
        }

        private static async Task SendEvents()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(eventHubConnString)
            {
                EntityPath = eventHubEntityPath
            };
            client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            Random ran = new Random();
            for (int i = 0; i<eventsToSend; i++)
            {
                string randomData = PrepareRandomData(ran);
                try
                {
                    await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(randomData)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} - Exception: {ex.ToString()}");
                }
                await Task.Delay(milisecondsBetweenEvents);
            }
            await client.CloseAsync();
        }

        private static string PrepareRandomData(Random ran)
        {
            #region Latitude and Longitude generator
            double lat = ran.NextDouble() - 10.5;
            double lon = ran.NextDouble() - 76;
            string latString = lat.ToString();
            latString = latString.Replace(',', '.');
            latString = latString.Substring(0, 7);
            string lonString = lon.ToString();
            lonString = lonString.Replace(',', '.');
            lonString = lonString.Substring(0, 7);
            #endregion

            var message = $"{{" +
                $"\"IdentidadBarco\": {ran.Next(1, 5)}," +
                $"\"TemperaturaBodega1\": {ran.Next(15, 20)}," +
                $"\"TemperaturaBodega2\": {ran.Next(15, 20)}," +
                $"\"TemperaturaBodega3\": {ran.Next(15, 20)}," +
                $"\"TemperaturaBodega4\": {ran.Next(15, 20)}," +
                $"\"VelocidadBarco\": {ran.Next(80, 120)}," +
                $"\"Latitud\": {latString}," +
                $"\"Longitud\": {lonString}," +
                $"\"Hora\": {DateTime.UtcNow.Hour}," +
                $"\"Minuto\": {DateTime.UtcNow.Minute}," +
                $"\"Segundo\": {DateTime.UtcNow.Second}," +
                $"\"Tiempo\": \"{DateTime.UtcNow.TimeOfDay}\"" +
                $"}}";
            return message;
        }
    }
}
