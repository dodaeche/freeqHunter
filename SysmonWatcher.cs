using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Timers;

namespace freeqHunter
{
    static class SysmonWatcher
    {

        static EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset);
        static System.Timers.Timer uploadTimer = new System.Timers.Timer(30000);
        static bool uploadTimerElapsed;
        static long m_ByteCounter = 0;
        static long maxFileSize = 32768;
        static StreamWriter m_FileStream;
        static string TempDirectory;
        static string m_TempFileName;

        internal static void Run()
        {
            uploadTimer.AutoReset = true;
            uploadTimer.Elapsed += UploadTimer_Elapsed;
            uploadTimer.Enabled = true;

            TempDirectory = Path.Combine(Program.Endpoint.StorageDirectory, "Temp");
            if (!Directory.Exists(TempDirectory))
                Directory.CreateDirectory(TempDirectory);


            m_TempFileName = Utility.GetTempFile("event", TempDirectory);
            m_FileStream = new StreamWriter(m_TempFileName);



            EventLogWatcher watcher = null;
            try
            {
                EventLogQuery qry = new EventLogQuery("Microsoft-Windows-Sysmon/Operational", PathType.LogName);
                watcher = new EventLogWatcher(qry);
                watcher.EventRecordWritten += new EventHandler<EventRecordWrittenEventArgs>(EventLogEventRaised);
                watcher.Enabled = true;
                while (true)
                {
                    Thread.Sleep(100);
                }

            }


            catch (EventLogReadingException e)
            {
                Console.WriteLine("Error reading the log: {0}", e.Message);
            }
            finally
            {
                // Stop listening to events
                watcher.Enabled = false;

                if (watcher != null)
                {
                    watcher.Dispose();
                }
            }

        }



        private static void UploadTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            uploadTimerElapsed = true;

        }

        internal static void EventLogEventRaised(object obj, EventRecordWrittenEventArgs arg)
        {
            EventRecord e = arg.EventRecord;
            parseEventRecord(e);
        }
        internal static void parseEventRecord(EventRecord evt)
        {
            string eventName;
            Dictionary<string, string> finalValues = new Dictionary<string, string>();
            finalValues["machineName"] = evt.MachineName;
            finalValues["userId"] = evt.UserId.Value;
            finalValues["eventId"] = evt.Id.ToString();

            switch (evt.Id)
            {
                case 1:
                    eventName = "Process Creation";
                    break;
                case 2:
                    eventName = "File Creation Time Changed";
                    break;
                case 3:
                    eventName = "Network Connection";
                    break;
                case 4:
                    eventName = "Sysmon Service State";
                    break;
                case 5:
                    eventName = "Process Terminated";
                    break;
                case 6:
                    eventName = "Driver Loaded";
                    break;
                case 7:
                    eventName = "Image Loaded";
                    break;
                case 8:
                    eventName = "Create Remote Thread";
                    break;
                case 9:
                    eventName = "Raw Access Read";
                    break;
                case 10:
                    eventName = "Process Access";
                    break;
                case 11:
                    eventName = "File Create";
                    break;
                case 12:
                    eventName = "Registry Event (Create/Delete)";
                    break;
                case 13:
                    eventName = "Registry Event (Value Set)";
                    break;
                case 14:
                    eventName = "Registry Event (Key/Value Rename)";
                    break;
                case 15:
                    eventName = "File Create Stream Hash";
                    break;
                default:
                    eventName = "None";
                    break;

            }
            finalValues["EventName"] = eventName;
            finalValues["provider"] = evt.ProviderName;
            string resString = evt.FormatDescription();
            string[] splitString = resString.Split(new char[] { '\n' }).Skip(1).ToArray();
            foreach (string val in splitString)
            {
                string[] result = val.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                finalValues[result[0].Trim()] = result[1].Trim();

            }

            string serializedValue = JsonConvert.SerializeObject(finalValues);
            writeData(serializedValue);


        }


        private static void writeData(string fileLine)
        {
            waitHandle.WaitOne();
            EvLog.WriteLog("Checking for upload timer or data size", 7000);
            if (m_ByteCounter >= maxFileSize || (uploadTimerElapsed && m_ByteCounter > 0))
            {
                uploadTimer.Stop();
                m_FileStream.Close();
                Utility.ZipFile(m_TempFileName);
                m_TempFileName = Utility.GetTempFile("sysevt", TempDirectory);
                m_FileStream = new StreamWriter(m_TempFileName);
                m_ByteCounter = 0;
                uploadTimerElapsed = false;
                uploadTimer.Start();

            }

            m_FileStream.WriteLine(fileLine);
            m_ByteCounter = m_ByteCounter + fileLine.Length;
            waitHandle.Set();

        }
    }
}
