using System;
using System.Net;

namespace freeqHunter
{
    static class DataForwarder
    {
        static string url = "http://<serverip>:<serverport>/api/1.0/uploadFile/";

        internal static int SendFile(string filePath)
        {
            try
            {

                WebClient syncClient = new WebClient();
                byte[] result = syncClient.UploadFile(url, filePath);
                EvLog.WriteLog(String.Format("{0} File Uploaded.", System.Text.ASCIIEncoding.ASCII.GetString(result)), 1011);

            }
            catch (WebException e)
            {

                HttpWebResponse response = (System.Net.HttpWebResponse)e.Response;
                EvLog.WriteLog(String.Format("Could Not Uplod File ERROR_CODE: {0}", response.StatusCode), 1011);
            }
            catch
            {
                EvLog.WriteLog("Could Not Upload File: " + filePath, 1006);
            }

            return 0;
        }

        internal static string GetUploadServer(string customer_id)
        {
            string url = "http://<serverip>:<serverport>/api/1.0/getServerName/" + customer_id;
            WebClient syncClient = new WebClient();
            string content = syncClient.DownloadString(url);
            if (content == null || content == string.Empty)
            {
                return Program.DEFAULT_URL;
            }
            else
            {
                return content;
            }
        }
    }
}
