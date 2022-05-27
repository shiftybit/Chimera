using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Microsoft.Scripting.Hosting;
using System.Management.Automation;
using IronPython.Runtime;
using IronPython.Hosting;

namespace Chimera
{
    [Cmdlet(VerbsCommon.Get, "StickyNoteContents")]
    public class SNT : StaticCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        private string _filePath;

        static bool EventsRegistered = false;

        private ScriptEngine eng;
        private ScriptScope scope;


        public SNT()
        {
            if (!EventsRegistered)
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
                AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                EventsRegistered = true;
            }
            LoadPythonModules();
        }

        protected override void BeginProcessing()
        {
            // same thing as Powershell's begin{} block. 
        }

        protected override void ProcessRecord()
        {
            // same thing as PowerShell's process{} block.
            ProcessSNT(FilePath);
        }

        public string getCode(string fileName)
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

        public void ProcessSNT(string sntFile)
        {

            string code = getCode("code.py");
            eng.Execute(code, scope);

            dynamic SNTParse = scope.GetVariable("SNTParse");
            dynamic value = SNTParse(sntFile);

            foreach (Dictionary<string, object> item in value)
            {
                PSObject obj = new PSObject();

                foreach (string key in item.Keys)
                {
                    object itemVal = item[key];
                    PSNoteProperty np = new PSNoteProperty(key, itemVal);
                    obj.Properties.Add(np);
                }
                    
                WriteObject(obj);
            }
        }

    }
}
