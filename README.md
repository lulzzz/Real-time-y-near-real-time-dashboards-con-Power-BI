# Dashboards en real-time y near real-time con Power BI

Power BI admite como origen de datos un dataset que se alimente en modo streaming. Esta secuencia de datos puede ser visualizada en dashboards en forma desatendida, es decir, sin necesidad de hacer refresco de los tiles.

En estas líneas intentaremos explicar los conceptos y detallar los pasos para realizar dashboards con Power BI en tiempo real y casi tiempo real.

## Resumen
Además de las series temporales, hay numerosos escenarios donde la posibilidad de contar con información en tiempo real o con unos pocos segundos de retardo es fundamental para la toma de decisiones.

## Contexto
Las [arquitecturas lambda](https://en.wikipedia.org/wiki/Lambda_architecture) fueron concebidas para diferenciar información analítica y/o histórica (cold data, cold path, low speed, batch-processing, batch layer, etc.) de la información que requiere un tratamiento inmediato (hot data, hot path, high speed, stream-processing, speed layer, etc.)

A continuación se analiza un escenario genérico y se describen los pasos para implementar el hot path a través de las herramientas de [Azure Event Hub](https://azure.microsoft.com/en-us/services/event-hubs/), [Azure Stream Analytics](https://azure.microsoft.com/en-us/services/stream-analytics/) y [Power BI](https://powerbi.microsoft.com/en-us/what-is-power-bi/).

## Software y herramientas

* Subscripción de Azure
* [Visual Studio](https://www.visualstudio.com)
* [Azure Event Hub](https://azure.microsoft.com/en-us/services/event-hubs/)
* [Azure Stream Analytics](https://azure.microsoft.com/en-us/services/stream-analytics/)
* [Power BI](https://powerbi.microsoft.com/en-us/what-is-power-bi/)
* [Azure SQL Database](https://azure.microsoft.com/en-us/services/sql-database/) (opcional)

## Fases del proyecto
Como parte del ciclo de vida de desarrollo del proyecto propio que se esté ejecutando es importante destacar dos fases que son convenientes tener en especial consideración.
* La gestión de requerimientos, para una adecuada evaluación de requisitos y resultados esperados (no toda información es posible ni conveniente desplegarla en tiempo real).
* El diseño de la arquitectura, que contemple los requisitos funcionales y los atributos de calidad o requerimientos no funcionales.

A continuación se desarrollan brevemente algunas recomendaciones y buenas prácticas sobre estas dos actividades.

## Gestión de requerimientos
Explicar aquí la conveniencia o no de tener datos en real time y near real time. Los escenarios en los que conviene tener esta información, etc.

## Arquitectura de alto nivel
Mostrar aquí el diagrama y los principales componentes

![Diagrama de arquitectura](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/diagram.JPG?raw=true)

## Producción de eventos
<<<<<<< HEAD
Típicamente los eventos son generados por dispositivos o aplicaciones en forma independiente a través del tiempo. Para imitar este flujo de datos incluimos el código fuente de un emulador de eventos con el que se podrá completar el ejercicio de visualización en real-time [(Ver carpeta EventHubSender)](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/tree/master/EventHubSender) para el codigo fuente completo.

### Datos aleatorios generados ###
Generamos datos aleatorios de un barco con los siguientes atributos:
- Temperatura de 4 bodegas (valores numericos que van de 15 - 20)
- Velocidad de barco (valores numericos que van de 80-120)
- Latitud y longitud del barco (valores numericos que representan una posicion en el mar caribe cerca a Cartagena, Colombia)
- Tiempo general, hora, minuto y segundo en el que se envian los datos
- ID de barco (valores numericos que van de 1-5) 

### Codigo fuente ###
Contamos con un proyecto de consola hecho en C# que cumple con las siguientes premisa de que envie N eventos generados cada M milisegundos. Para poder usar el emulador solo es necesario obtener una cadena de conexion a un EventHub Namespace y el nombre de un EventHub que exista dentro de ese Namespace. Ingresa al archivo <b>App.Config</b> en el codigo fuente y pega dichos valores:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="eventHubNameSpaceConnectionString" value="yourEventHubNamespaceConnectionString" />
    <add key="eventHubName" value="yourEventHubName" />
  </appSettings>
</configuration>
```

El codigo fuente del emulador de eventos cuenta con solo un archivo llamado <b>Program.cs</b> que contiene toda la logica. Se cuentan con 4 atributos principales:
- Numero de eventos a enviar
- Milisegundos que deben pasar entre cada evento
- Cadena de conexion de un EventHub Namespace
- Nombre de un Event Hub

```csharp
class Program
{
    public static int eventsToSend;
    public static int milisecondsBetweenEvents;
    public static string eventHubConnString = ConfigurationManager.AppSettings["eventHubNameSpaceConnectionString"];
    public static string eventHubEntityPath = ConfigurationManager.AppSettings["eventHubName"];
}
```

El metodo principal consta solo de 3 lineas en las que se define el numero de eventos, milisegundos entre eventos y llamada al metodo <b>SendEvents()</b> que se encarga de mandar los datos al servicio de EventHub:
```csharp
static void Main(string[] args)
{
    milisecondsBetweenEvents = 1000;
    eventsToSend = 20;
    SendEvents().GetAwaiter().GetResult();
}
```

El metodo <b>SendEvents</b> esta compuesto de 3 partes diferenciadas:
- Un constructor de cliente de EventHub
- Una llamada a un metodo que genera datos aleatorios
- El envio de dicha data generada a EventHub
```csharp
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
```

Finalmente, el metodo <b>PrepareRandomData</b> genera los datos de temperatura, latitud, longitud, tiempo y ID para un barco ficticio y devuelve una cadena con formato JSON para que dicha cadena sea enviada a EventHub:
```csharp
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
```
=======
Cualquier artefacto que envíe datos al Event Hub es considerado un productor de eventos. Estos artefactos son típicamente dispositivos o aplicaciones que envían información a través del tiempo. Los eventos son transmitidos mediante HTTPS o AMQP. Para imitar este flujo de datos incluimos el código fuente de un emulador de eventos con el que se podrá completar el ejercicio de visualización en real-time.

[Event Hubs API Overview](https://docs.microsoft.com/es-es/azure/event-hubs/event-hubs-dotnet-framework-api-overview)

INCLUIR AQUÍ DESCRIPCIÓN DEL CÓDIGO FUENTE Y MODO DE USO)
>>>>>>> 0d5fb579e2f3902deca8fd7076d398a495ff9504

## Ingestión y encolamiento
Azure Event Hubs es un servicio para la ingestión masiva de eventos, en este tutorial se empleará como vía de entrada para los datos. Event Hubs posee varias características interesantes para soportar múltiples escenarios de control de flujo, contextos de publicación-subscripción, etc. Más información [aquí](https://docs.microsoft.com/es-es/azure/event-hubs/event-hubs-what-is-event-hubs).

### Crear un Event Hub en Azure
1. Inicie sesión en [Azure Portal](https://portal.azure.com/)
2. Inidque **Crear un recurso**


Desarrollar paso a paso cómo crear un event hubs, la aplicación cliente e intercalar conceptos y tips

## Análisis de streaming de datos
Desarrollar paso a paso una query en Stream Analytics

## Visualización

### Real-time

### Near real-time
Una vez que Stream Analytics ha creado el dataset, éste puede ser utilizado en reportes convencionales empleando todos las visualizaciones disponibles. Notar sin embargo que los reportes no se refrescan automáticamente. Los reportes están concebidos para ser interactivos, es decir, para que el usuario realice operaciones de filtrado, drill down y búsquedas en forma manual. Para obtener un tile actualizado en near real-time éste debe ser anclado en un dashboard.

Para ello, marque el ícono de "Pin" en la visualización que desea exponer:

![Anclar un tile](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/power-bi-pin.png?raw=true)

Indique un dashboard existente o cree uno nuevo en donde la visualización será publicada:

![Seleccionar o crear un dashboard](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/pbi_pintoanotherdash.png?raw=true)

## Recomendaciones

## Conclusiones

## Referencias
bullets de links a cosas relacionadas.