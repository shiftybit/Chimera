using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Chimera
{
    public class Tools
    {

        /// <summary>
        /// Recursively Delete Directory
        /// </summary>
        /// <param name="target_dir">Target Directory</param>
        public static void DeleteDirectory(string target_dir)
        {
            string[] files;
            string[] dirs;
            try
            {
                files = Directory.GetFiles(target_dir);
                dirs = Directory.GetDirectories(target_dir);
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            catch (FileNotFoundException)
            {
                return;
            }


            foreach (string file in files)
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch (IOException e)
                {
                    Trace.WriteLine($"Cannot delete {file}. File may be open elsewhere and locked for editing.");
                }
                catch (UnauthorizedAccessException)
                {

                }
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
            try
            {
                Directory.Delete(target_dir, true);
            }
            catch (IOException)
            {
                Trace.WriteLine($"Cannot delete {target_dir}. Folder may be open elsewhere and locked for editing.");
            }
        }
    }
}
