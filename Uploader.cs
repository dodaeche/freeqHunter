using System;
using System.IO;
using System.Threading;

namespace freeqHunter
{
    internal static class Uploader
    {
        internal static void UploadFiles()
        {

            while (true)
            {
                try
                {
                    foreach (string fileName in Directory.GetFiles(Program.Endpoint.StorageDirectory, "*.gz"))
                    {
                        DataForwarder.SendFile(fileName);
                        File.Delete(fileName);

                    }
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    EvLog.WriteLog("Upload Failed\n" + ex.Message, 1000);
                }
            }



        }
    }
}
