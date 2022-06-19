// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

/// Cantidad de usuarios concurrentes, modificar a conveniencia.
const int concurrentUsers = 6;
/// Nombre de usuario de Aula Digital, modificar a conveniencia.
const string username = "";
/// Contraseña de Aula Digital, modificar a conveniencia.
const string password = "";
/// Camino del archivo de vaciado de las observaciones, modificar a conveniencia.
const string outputFilePath = "";
/// <summary>
///     Método que crea el Driver de Selenium utilizado para enviar instrucciones al explorador
/// </summary>
IWebDriver CreateChromeWebDriver()
{
    var chromeOptions = new ChromeOptions();

    chromeOptions.AddArgument("no-sandbox");

    var webdriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), chromeOptions, TimeSpan.FromDays(1));

    return webdriver;
}
/// <summary>
///     Método que manipula el Driver de Selenium para realizar los pasos necesarios para acceder a
///     la página web donde se subirán los archivos (Iniciar sesión, navegar a la URL...)
/// </summary>
void Setup(IWebDriver webdriver)
{
    webdriver.Manage().Timeouts().PageLoad = TimeSpan.FromDays(1);

    webdriver.Navigate().GoToUrl("https://auladigital.ucab.edu.ve/login/canvas");

    var email = webdriver.FindElement(By.Id("pseudonym_session_unique_id"));

    var passwordInput = webdriver.FindElement(By.Id("pseudonym_session_password"));
    
    email.SendKeys(username);
    passwordInput.SendKeys(password);

    var button = webdriver.FindElement(By.CssSelector("button.Button.Button--login"));
    
    button.Submit();

    webdriver.Navigate().GoToUrl("https://auladigital.ucab.edu.ve/courses/3329/assignments/38934"); 
}
/// <summary>
///     Método que lista los archivos que van a ser enviados al servidor
/// </summary>
IEnumerable<string> ListFiles()
{
    /// Camino del directorio donde se encuentran los archivos a subir, modificar a conveniencia.
    const string path = "";
    return Directory.GetFiles(path).AsEnumerable().Take(13);
}
/// <summary>
///     Método envía el archivo y mide cuánto tiempo se ha tardado, mide la longitud del archivo y si la subida
///     resultó en un fallo.
/// </summary>
(long, bool, long) MeassureUploadTime(IWebDriver webdriver, string file)
{
    var submitButton =
        webdriver.FindElement(By.CssSelector("button.Button.Button--primary.submit_assignment_link"));
    
    submitButton.Click();

    var loadButton =
        webdriver.FindElement(By.XPath(
            "/html/body/div[2]/div[2]/div[2]/div[3]/div[1]/div/div[3]/div/form[1]/table/tbody/tr[2]/td/div[1]/div/button[1]"));

    loadButton.Click();
    
    var fileInput =
        webdriver.FindElement(By.CssSelector("input.input-file"));
        
    fileInput.SendKeys(file);
    
    var sendButton =
        webdriver.FindElement(By.Id("submit_file_button"));

    var stopwatch = new Stopwatch();

    stopwatch.Start();

    var isError = false;

    try
    {
        // var shortTimeoutInCaseOfError = new WebDriverWait(webdriver, TimeSpan.FromMinutes(1));
        var longTimeout = new WebDriverWait(webdriver, TimeSpan.FromDays(1));
        sendButton.Submit();
        // shortTimeoutInCaseOfError.Until(d => d.FindElement(By.CssSelector("svg.wIZqC_cJVF")).Displayed);
        longTimeout.Until(d => d.FindElement(By.CssSelector("button.Button.Button--primary.submit_assignment_link")).Displayed);
    }
    catch
    {
        isError = true;
    }
    finally
    {
        stopwatch.Stop();
    }
    
    return (stopwatch.ElapsedMilliseconds, isError, new FileInfo(file).Length);
}

/// Obtenemos los archivos que se van a subir
var uploadFiles = ListFiles();

var webDrivers = Enumerable
    .Range(0, concurrentUsers)
    .Select(i  =>
    {
        var webdriver = CreateChromeWebDriver();
        Setup(webdriver);

        return webdriver;
    }) /// Creamos tantos drivers como usuarios concurrentes se deseen (si son 8 usuarios concurrentes, serán 8 exploradores abiertos)
    .ToArray();

var benchmarkResult = webDrivers 
    .AsParallel()
    .WithDegreeOfParallelism(concurrentUsers) /// Paralelizamos la colección, de forma que cada paso siguiente será ejecutado en paralelo
    .SelectMany<IWebDriver, (long, bool, long, int)>((webdriver, i) =>
        uploadFiles.Select(file =>
        {
            var (ms, isError, length) = MeassureUploadTime(webdriver, file);
            return (ms, isError, length, i);
        })
    ) /// Medimos la subida de cada archivo
    .Select((benchmarkMeassure) =>
    {
        var (ms, isError, length, index) = benchmarkMeassure;
        var measureTemplate = new StringBuilder()
            .AppendFormat("{0}ms", ms)
            .Append(",")
            .Append(isError)
            .Append(",")
            .AppendFormat("{0}bytes", length)
            .Append(",")
            .Append(index)
            .ToString(); /// Construimos el formato .csv de las observaciones;
        
        Console.WriteLine(measureTemplate.ToString());
        return measureTemplate.ToString();
    })
    .ToList();

/// Vaciamos las observaciones en un archivo .csv;
File.WriteAllLines(outputFilePath, benchmarkResult);
