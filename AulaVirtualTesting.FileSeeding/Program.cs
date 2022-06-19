// See https://aka.ms/new-console-template for more information
using System.Text;

/// 1Kb = 1024b
const int kilobytes = 1024;
/// Cantidad de archivos a generar, manipular a conveniencia.
const int amountOfFiles = 100;
/// Camino del directorio donde se guardaran los archivos.
const string path = "";
/// Crear el directorio si no existe
Directory.CreateDirectory(path);
/// Creamos un PRNG
var random = new Random();
/// Iteramos por la misma cantidad de archivos a generar
foreach (var i in Enumerable.Range(0, amountOfFiles))
{
    /// Obtenemos (aleatoriamente) el peso del archivo, entre 128Kb y 1Mb
    var length = random.Next(128 * kilobytes, 1024 * kilobytes);
    /// Generamos el nombre del archivo
    var filename = new StringBuilder("Testing_File_")
        .Append(i)
        .ToString();
    /// Generamos el camino completo hacia el nuevo archivo a generar
    var filepath = Path.Combine(path, filename);
    /// Creamos un buffer de bytes con la cantidad anteriormente generada
    Span<byte> bytes = new byte[length];
    /// Creamos el descriptor del archivo a generar.
    using var file = File.Create(filepath, bytes.Length);
    /// Llenamos el buffer previamente construido con bytes aleatorios.
    random.NextBytes(bytes);
    /// Escribimos al archivo los bytes generados
    file.Write(bytes);
}