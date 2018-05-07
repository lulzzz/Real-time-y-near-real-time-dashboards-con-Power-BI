# Dashboards en real-time y near real-time con Power BI

Power BI admite como origen de datos un dataset que se alimente en modo streaming. Esta secuencia de datos puede ser visualizada en dashboards en forma desatendida, es decir, sin necesidad de hacer refresco de los tiles.

En estas líneas se explicarán los conceptos y detalle de los pasos para realizar dashboards con Power BI en real-time y near real-time.

## Resumen
Además de las series temporales, hay numerosos escenarios donde la posibilidad de contar con información en tiempo real o con unos pocos segundos de retardo es fundamental para la toma de decisiones.

## Contexto
Las [arquitecturas lambda](https://en.wikipedia.org/wiki/Lambda_architecture) fueron concebidas para diferenciar información analítica y/o histórica (cold data, cold path, low speed, batch-processing, batch layer, etc.) de la información que requiere un tratamiento inmediato (hot data, hot path, high speed, stream-processing, speed layer, etc.)

A continuación se analiza un escenario genérico y se describen los pasos para implementar el hot path a través de las herramientas de [Azure Event Hub](https://azure.microsoft.com/en-us/services/event-hubs/), [Azure Stream Analytics](https://azure.microsoft.com/en-us/services/stream-analytics/) y [Power BI](https://powerbi.microsoft.com/en-us/what-is-power-bi/).

## Software y herramientas

* [Subscripción de Azure](https://azure.microsoft.com/es-es/free)
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
Es importante hacer una consideración de alto nivel con respecto a los requisitos que son evaluados para la visualización de información en tiempo real. La necesidad de contar con datos actualizados no es condición necesaria para implementar una solución de streaming de datos. Muchas veces una solución de micro-batch es más adecuada si se precisan cálculos complejos, contrastes con datos históricos, cruces de información con múltiples orígenes de datos históricas, etc. **La visualización de dashboards en real-time está más asociada con los últimos datos más que con una fuente de datos actualizada.**

## Arquitectura de alto nivel
Como se mencionó anteriormente hablar de real-time o near real-time implica un modelo dinámico diseñado para tener en cuenta la menor latencia posible en el flujo de datos. Las técnicas y herramientas empleadas aquí siguen el flujo hot-path de una arquitectura [lambda](https://en.wikipedia.org/wiki/Lambda_architecture), tal como se describe a continuación.

![Diagrama de arquitectura](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/diagram.JPG?raw=true)

## Producción de eventos
<<<<<<< HEAD
Típicamente los eventos son generados por dispositivos o aplicaciones en forma independiente a través del tiempo. Para imitar este flujo de datos se incluye el código fuente de un emulador de eventos con el que se puede completar el ejercicio de visualización en real-time [(Ver carpeta EventHubSender](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/tree/master/EventHubSender) para el código fuente completo)
=======
Típicamente los eventos son generados por dispositivos o aplicaciones en forma independiente a través del tiempo. Para imitar este flujo de datos incluimos el código fuente de un emulador de eventos con el que se podrá completar el ejercicio de visualización en real-time [(Ver carpeta EventHubSender)](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/tree/master/EventHubSender) para el codigo fuente completo.
>>>>>>> 6e765200527acdf988672ccdd5a907828726b1c6

### Datos aleatorios generados ###
Se generan datos aleatorios de un barco con los siguientes atributos:
- Temperatura de 4 bodegas (valores numéricos que van de 15 - 20)
- Velocidad de barco (valores numéricos que van de 80-120)
- Latitud y longitud del barco (valores numéricos que representan una posición en el mar Caribe cerca a Cartagena, Colombia)
- Tiempo general, hora, minuto y segundo en el que se envían los datos
- ID de barco (valores numéricos que van de 1-5) 

### Código fuente ###
El proyecto de consola hecho en C# cumple con la premisa de que se envíen N eventos generados cada M milisegundos y funciona toda vez que una cadena de conexión a un EventHub Namespace y el nombre de un EventHub que exista dentro de ese Namespace hayan sido definidos en el archivo <b>App.Config</b>:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="eventHubNameSpaceConnectionString" value="yourEventHubNamespaceConnectionString" />
    <add key="eventHubName" value="yourEventHubName" />
  </appSettings>
</configuration>
```

El código fuente del emulador de eventos cuenta con solo un archivo <b>Program.cs</b> que contiene toda la lógica. Se cuentan con 4 atributos principales:
- Número de eventos a enviar
- Milisegundos que deben pasar entre cada evento
- Cadena de conexión de un EventHub Namespace
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

El método principal consta de 3 lineas de código en las que se definen el número de eventos, milisegundos entre eventos y llamada al método <b>SendEvents()</b> que se encarga de mandar datos al servicio de EventHub:
```csharp
static void Main(string[] args)
{
    milisecondsBetweenEvents = 1000;
    eventsToSend = 20;
    SendEvents().GetAwaiter().GetResult();
}
```

El método <b>SendEvents</b> está compuesto de 3 partes diferenciadas:
- Un constructor de cliente de EventHub
- Una llamada al método <b>PrepareRandomData</b> que genera datos aleatorios
- El envío de dichos datos generados a EventHub
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

Finalmente, el metodo <b>PrepareRandomData</b> genera los datos de temperatura, latitud, longitud, tiempo y ID para un barco ficticio y devuelve una cadena con formato JSON:
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

## Ingestión y encolamiento
Azure Event Hubs es un servicio para la ingestión masiva de eventos, en este tutorial se empleará como vía de entrada para los datos. Event Hubs posee varias características interesantes para soportar múltiples escenarios de control de flujo, contextos de publicación-subscripción, etc. Más información [aquí](https://docs.microsoft.com/es-es/azure/event-hubs/event-hubs-what-is-event-hubs).

### Crear un Event Hub en Azure
1. Inicie sesión en [Azure Portal](https://portal.azure.com/)
2. Inidque **Crear un recurso**


Desarrollar paso a paso cómo crear un event hubs, la aplicación cliente e intercalar conceptos y tips

## Análisis de streaming de datos
Desarrollar paso a paso una query en Stream Analytics

## Visualización
La visualización de datos en real-time implica la mínima latencia posible, desde que el evento es producido hasta que es visualizado. Esta latencia es la suma de las latencias acumuladas en cada etapa: el tiempo de detección del evento en el productor, el tiempo de transferencia, el tiempo de encolamiento, el tiempo de procesamiento, el tiempo de envío e inserción en el dataset del dashboard y el tiempo de refresco en el mismo.

Normalmente las visualizaciones de Power BI requieren algún tipo de cálculo o procesamiento, por ejemplo, un diagrama de pie, necesita sumarizar campos por categorías y calcular los porcentajes de cada una para poder diagramarlos. La suma del esfuerzo de cada visualización puede ser muy costoso para un dashboard en términos de tiempo de procesamiento, por tanto, para tener una visualización actualizada en real-time, se han acotado la variedad de visualizaciones. Básicamente los tiles que se pueden agregar en real-time son visualizaciones simples de series temporales. El resto de las visualizaciones podrán ser empleadas con una frecuencia de actualización menor, que conocemos como near real-time.

### Real-time
En esta solución, la latencia acumulada entre el momento del encolamiento hasta el del refresco en el dasboard es del orden de los **2 segundos**.

![Agregar nuevo tile](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/custom-streaming-data.png?raw=true)

![Agregar nuevo tile](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/your-streaming-dataset.png?raw=true)

Seleccione el tipo de visualización y continue la configuración de acuerdo al mismo.

![Agregar nuevo tile](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/pbi-dashboard-fraudulent-calls-finished.png?raw=true)

Estas visualizaciones se actualizan una vez por segundo.

### Near real-time
La latencia para la presentación de datos en near real-time va de entre los **5 y los 8 segundos** aproximadamente.

Una vez que Stream Analytics ha creado el dataset, éste puede ser utilizado en reportes convencionales empleando todos las visualizaciones disponibles. Notar sin embargo que los reportes no se refrescan automáticamente. Los reportes están concebidos para ser interactivos, es decir, para que el usuario realice operaciones de filtrado, drill down y búsquedas en forma manual. Para obtener un tile actualizado en near real-time éste debe ser anclado en un dashboard.

Para ello, marque el ícono de "Pin" en la visualización que desea exponer:

![Anclar un tile](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/power-bi-pin.png?raw=true)

Indique un dashboard existente o cree uno nuevo en donde la visualización será publicada:

![Seleccionar o crear un dashboard](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/pbi_pintoanotherdash.png?raw=true)

> Importante: si el dataset fue definido manualmente (y no a través de Stream Analytics), se debe activar la opción "Historic data analysis", en caso contrario no se podrán crear reportes con ese dataset.
> ![Seleccionar o crear un dashboard](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/real-time-streaming_0c.png?raw=true)

## Recomendaciones

## Referencias
bullets de links a cosas relacionadas.