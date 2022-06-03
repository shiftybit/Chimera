using System;
using System.Management.Automation;
using System.IO;

namespace Chimera
{
    [Cmdlet(VerbsLifecycle.Invoke, "Python")]
    public class InvokePython: StaticPythonCmdlet
    {
        [Parameter()]
        public string FileName { get; set; }

        delegate void ResolverDelegate(object s, EventArgs e);
        private bool Errors = false;

        public InvokePython()
        {
            //ResolverDelegate d = new ResolverDelegate(Handle_DLLResolverRegistered); 
            if (!EventsRegistered)
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
                AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                EventsRegistered = true;
            }
            DllResolversAdded += Handle_DLLResolverRegistered;
        }
        public void Handle_DLLResolverRegistered(object s,EventArgs eventArgs)
        {
            WriteObject("DLL Resolver Register Event Called");
        }
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin processing for Internal Python");
            if (!File.Exists(FileName)) {
                WriteVerbose($"Error finding {FileName}");    
                Errors = true; 
            }
        }
        protected override void ProcessRecord()
        {
            if(Errors) return;
            string code = File.ReadAllText(FileName);
            eng.Execute(code, scope);
        }

    }
}
