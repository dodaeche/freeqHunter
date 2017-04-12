using System.Diagnostics;

namespace freeqHunter
{
    static class EvLog
    {
        static EventLog log = new EventLog("Application", System.Environment.MachineName, "freeqHunter");

        public static void WriteLog(string message, int Id)
        {
            log.WriteEntry(message, EventLogEntryType.Information, Id);
        }
    }
}
