using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Management.Automation;


namespace Chimera
{
    [Cmdlet(VerbsCommon.Get, "StickyNoteContents")]
    public class SNT : StaticPythonCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        private string _filePath;

        static bool EventsRegistered = false;

        public SNT()
        {
            if (!EventsRegistered)
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
                AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                EventsRegistered = true;
            }
            
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

        public void ProcessSNT(string sntFile)
        {

            string code = GetEmbeddedPythonScript("code.py");
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
