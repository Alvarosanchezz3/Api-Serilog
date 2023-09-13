using Serilog;
using System.Text;
using Newtonsoft.Json;
using Pruebaaaa;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();

var app = builder.Build();

// Creación de lista de logs y config de Serilog

List<string> logs = new();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // IMPORTANTE --> Aqui especificamos el nivel máximo de los logs que queremos, ya sea informacion, advertencias, errores graves...
    .WriteTo.Console()
    .WriteTo.Sink(new FileListSink(logs))
    .CreateLogger();

// Peticion http a la Api de Serilog
async Task SendLogsToApi()
{
    while (true)
    {
        if (logs.Count > 0)
        {
            using HttpClient client = new();
            string json = JsonConvert.SerializeObject(logs);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://localhost:7095/Logs", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Log enviados con éxito");
                logs.Clear();
            }
            else
            {
                Console.WriteLine("Error al enviar el log");
            }
        }
        await Task.Delay(TimeSpan.FromSeconds(2));
    }
}

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

_ = SendLogsToApi(); // Aquí se hace la llamada para que cargue la app antes de hacer la llamada de la tarea

app.Run();