# Prueba 4 del Caso de estudio (ESI)

En este repositorio se encuentran el código fuente utilizado para generar los archivos a subir en la Prueba 4 como el código fuente utilizado para automatizar la subida del archivo usando Selenium.WebDriver. Caso de estudio de ESI (Período 2022) Grupo 4.

## Proceso de generación de archivos

- Se generan aleatoriamente los pesos de los archivos.
- Se generan una cantidad aleatoria de bytes de igual tamaño que el peso generado anteriormente.
- Se escriben dichos bytes a un archivo específico

## Proceso de automatización de la subida de archivos

- Se generan tantos drivers de conexión con selenium como usuarios concurrentes a simular
- Se inicia sesión en Aula Digital y se navega a la URL de la actividad a donde subir el archivo
- Se ingresa el camino compĺeto del archivo y se hace click en el botón de envío.
- Se mide el tiempo entre que se hizo click al botón de envío y que la página haya recargado exitosamente, así como la longitud del archivo enviado
- Se vacían las observaciones en un archivo .csv
