using System;
using System.IO;
using Microsoft.Win32;


namespace freeqHunter
{
    internal class WorkstationRegistration
    {
        private Guid m_WorkstationId;
        private string m_StorageDirectory;
        private string m_ServerUrl;

        public string ServerUrl
        {
            get { return m_ServerUrl; }
        }
        public Guid WorkstationId
        {
            get { return m_WorkstationId; }

        }

        public string StorageDirectory
        {
            get { return m_StorageDirectory; }


        }





        public WorkstationRegistration()
        {
            try
            {
                string SUB_KEY_NAME = @"SOFTWARE\ThreatInformatics";
                RegistryKey key = Registry.LocalMachine.OpenSubKey(SUB_KEY_NAME);
                if (key == null)
                {
                    key = Registry.LocalMachine.CreateSubKey(SUB_KEY_NAME);
                }


                object tempValue = key.GetValue("HardwareId");
                if (tempValue == null)
                {
                    m_WorkstationId = Guid.NewGuid();
                    key.SetValue("HardwareId", m_WorkstationId.ToString(), RegistryValueKind.String);

                }
                else
                {
                    m_WorkstationId = Guid.Parse(tempValue.ToString());
                }

                m_StorageDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SysmonForwarder");
                EvLog.WriteLog(String.Format("Set Storage Directory to {0}", m_StorageDirectory), 1000);
                if (!Directory.Exists(m_StorageDirectory))
                {
                    Directory.CreateDirectory(m_StorageDirectory);
                }


                m_ServerUrl = DataForwarder.GetUploadServer(Guid.NewGuid().ToString());

                EvLog.WriteLog(String.Format("Received serverurl : {0}", m_ServerUrl), 1005);

            }
            catch (Exception error)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "freeqHunter";
                appLog.WriteEntry(error.Message, System.Diagnostics.EventLogEntryType.Error, 1000);

            }
        }
    }
}
