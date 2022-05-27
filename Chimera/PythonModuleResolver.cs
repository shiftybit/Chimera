using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Reflection;
using System.IO;

namespace Chimera
{
    public class PythonModuleResolver
    {
        public static string getLibZip(string fileName = "Lib.zip")
        {
            var assembly = Assembly.GetExecutingAssembly();
            string result;
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }


        /// <summary>
        /// Extracts the contents of a zip file to the 
        /// Temp Folder
        /// </summary>
        public static void ExtractZip(string tempfilename, string tempdirname,  string filename = "Lib.zip")
        {
            try
            {
                //string _tempPath = Environment.GetEnvironmentVariable("TEMP") + @"\";
                //string _zipPath = Environment.GetEnvironmentVariable("TEMP") + @"\" + @"MyZip.zip";
                string zipPath = tempfilename;
                string zipPath2 = Path.Combine(tempdirname, filename);
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (FileStream bw = new FileStream(zipPath, FileMode.Create))
                    {
                        //read until we reach the end of the file
                        while (stream.Position < stream.Length)
                        {
                            //byte array to hold file bytes
                            byte[] bits = new byte[stream.Length];
                            //read in the bytes
                            stream.Read(bits, 0, (int)stream.Length);
                            //write out the bytes
                            bw.Write(bits, 0, (int)stream.Length);
                        }
                    }
                    stream.Close();
                }

                //extract the contents of the file we created
                //UnzipFile(_zipPath, _tempPath);
                //or
                try
                {
                    Tools.DeleteDirectory(tempdirname);
                    ZipFile.ExtractToDirectory(zipPath, tempdirname);
                }
                catch (IOException ex)
                {

                }
                File.Delete(zipPath);

            }
            catch (Exception e)
            {
                //handle the error
            }
        }



        static void ExtractLib(string fileName = "Lib.zip")
        {
            var assembly = Assembly.GetExecutingAssembly();
            string result;
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            //using (ZipArchive zipArchive = new ZipArchive(stream))
            //{
            //    zipArchive.Ex
            //}
        }
    }
}
