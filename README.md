# Dashboards en real-time y near real-time con Power BI

Power BI admite como origen de datos un dataset que se alimente en modo streaming. Esta secuencia de datos puede ser visualizada en dashboards en forma desatendida, es decir, sin necesidad de hacer refresco de los tiles.

En estas líneas intentaremos explicar los conceptos y detallar los pasos para realizar dashboards con Power BI en tiempo real y casi tiempo real.

## Resumen
Además de las series temporales, hay numerosos escenarios donde la posibilidad de contar con información en tiempo real o con unos pocos segundos de retardo es fundamental para la toma de decisiones.

## Contexto
Las [arquitecturas lambda](https://en.wikipedia.org/wiki/Lambda_architecture) fueron concebidas para diferenciar información analítica y/o histórica (cold data, cold path, low speed, batch-processing, batch layer, etc.) de la información que requiere un tratamiento inmediato (hot data, hot path, high speed, stream-processing, speed layer, etc.)

A continuación se analiza un escenario genérico y se describen los pasos para implementar el hot path a través de las herramientas de [Azure Event Hub](https://azure.microsoft.com/en-us/services/event-hubs/), [Azure Stream Analytics](https://azure.microsoft.com/en-us/services/stream-analytics/) y [Power BI](https://powerbi.microsoft.com/en-us/what-is-power-bi/).

## Software y herramientas

* [Visual Studio]()
* [Azure Event Hub](https://azure.microsoft.com/en-us/services/event-hubs/)
* [Azure Stream Analytics](https://azure.microsoft.com/en-us/services/stream-analytics/)
* [Power BI](https://powerbi.microsoft.com/en-us/what-is-power-bi/)
* [Azure SQL Database](https://azure.microsoft.com/en-us/services/sql-database/) (opcional)

## Fases del proyecto
bullets

## Gestión de requerimientos
Explicar aquí la conveniencia o no de tener datos en real time y near real time. Los escenarios en los que conviene tener esta información, etc.

## Arquitectura de alto nivel
Mostrar aquí el diagrama y los principales componentes
![Diagrama de arquitectura](https://github.com/CSELATAM/Real-time-y-near-real-time-dashboards-con-Power-BI/blob/master/images/diagram.JPG?raw=true)

## Ingestión de datos
Desarrollar paso a paso cómo crear un event hubs, la aplicación cliente e intercalar conceptos y tips

## Análisis de streaming data
Desarrollar paso a paso una query en Stream Analytics

## Visualización
Describir la parte de Power BI, datasets, etc.

## Recomendaciones

## Conclusiones

## Referencias
bullets de links a cosas relacionadas.