using Serilog.Core;
using Serilog.Events;

namespace Pruebaaaa
{
    public class FileListSink : ILogEventSink
    {
        private readonly List<string> _logs;

        public FileListSink(List<string> logEvents)
        {
            _logs = logEvents;
        }

        public void Emit(LogEvent logEvent)
        {
            string programName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToUpper(); // Se añade el nombre del programa para la columna "Source" de los logs en Oracle

            if (logEvent.Exception == null)
            {
                Exception ExceptionVacía = new("Exception null");

                string formatedLogs = $"{logEvent.Timestamp} -> {logEvent.Level} -> {logEvent.RenderMessage()} -> {ExceptionVacía.Message} -> {logEvent.Properties} -> {programName}";
                _logs.Add(formatedLogs);              
            }
            else
            {
                string formatedLogs = $"{logEvent.Timestamp} -> {logEvent.Level} -> {logEvent.RenderMessage()} -> {logEvent.Exception.Message} -> {logEvent.Properties} -> {programName}";
                _logs.Add(formatedLogs);               
            }
        }
    }
}