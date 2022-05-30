using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Runtime;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Reflection;

namespace Chimera
{
    public class StaticPythonCmdlet : StaticCmdlet
    {
        protected static ScriptEngine eng;
        protected static ScriptScope scope;
        public StaticPythonCmdlet()
        {
            LoadPythonModules();
        }

        public string GetEmbeddedPythonScript(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string result;
            string[] resourceNames = assembly.GetManifestResourceNames();
            string resourceName = resourceNames.Single(str => str.EndsWith(fileName));
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        public bool ResourceExists(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            try
            {
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
                return true;

            }
            catch
            {
                return false;
            }
        }

        public void LoadPythonModules()
        {
            string tempFile = Path.GetTempFileName();
            string tempFolder = Path.GetTempPath();
            string IronPyLib = Path.Combine(tempFolder, "IronPyLib\\");
            string sitePackages = Path.Combine(IronPyLib, "site-packages");

            PythonModuleResolver.ExtractZip(tempFile, IronPyLib);

            eng = Python.CreateEngine();
            scope = eng.CreateScope();
            ICollection<string> searchPaths = eng.GetSearchPaths();
            searchPaths.Add(IronPyLib);
            searchPaths.Add(sitePackages);
            eng.SetSearchPaths(searchPaths);
        }

    }
}
