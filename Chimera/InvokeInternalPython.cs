using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
namespace Chimera
{
    [Cmdlet(VerbsLifecycle.Invoke, "InternalPython")]
    public class InvokeInternalPython: StaticPythonCmdlet
    {
        [Parameter()]
        public string FileName { get; set; }

        delegate void ResolverDelegate(object s, EventArgs e);
        private bool Errors = false;

        public InvokeInternalPython()
        {
            //ResolverDelegate d = new ResolverDelegate(Handle_DLLResolverRegistered); 
            DllResolversAdded += Handle_DLLResolverRegistered;
        }
        public void Handle_DLLResolverRegistered(object s,EventArgs eventArgs)
        {
            WriteObject("DLL Resolver Register Event Called");
        }
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin processing for Internal Python");
            if (!ResourceExists(FileName))
            {
                WriteObject($"{FileName} either does not exist, or there are multiple instances.");
                Errors = true;
            }
        }
        protected override void ProcessRecord()
        {
            if(Errors) return;
            string code = GetEmbeddedPythonScript(FileName);
            eng.Execute(code, scope);
        }

    }
}
