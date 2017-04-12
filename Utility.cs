using System;
using System.IO;
using System.IO.Compression;

namespace freeqHunter
{
    internal static class Utility
    {
        internal static string GetTempFile(string suffix, string l_FilePath)
        {

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string tempFileName = unixTimestamp.ToString();
            string fileName = string.Format("{0}-{1}-{2}.tmp", Program.Endpoint.WorkstationId.ToString(), tempFileName, suffix);

            string fullPath = Path.Combine(l_FilePath, fileName);
            return fullPath;
        }

        static internal void ZipFile(string fileName)
        {

            string compressedFileName = String.Empty;
            FileInfo fileToCompress = new FileInfo(fileName);
            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((File.GetAttributes(fileToCompress.FullName) &
                   FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    compressedFileName = fileToCompress.FullName + ".gz";
                    using (FileStream compressedFileStream = File.Create(compressedFileName))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                           CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);

                        }



                    }
                }


            }

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            File.Move(compressedFileName, Path.Combine(Program.Endpoint.StorageDirectory, String.Format("{0}-{1}-sysevt.gz", Program.Endpoint.WorkstationId, unixTimestamp.ToString())));
            File.Delete(fileName);
        }
    }
}
