using System;
using System.Management.Automation;

namespace Chimera
{
    [Cmdlet("Get","ClassicShellMRU")]
    public class GetClassicShellMRU : StaticPythonCmdlet
    {
        [Parameter(Position=0)]
        public string NTUserPath { get; set; }
        public GetClassicShellMRU()
        {
            if (!EventsRegistered)
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
                AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                EventsRegistered = true;
            }
        }

        protected override void ProcessRecord()
        {
            string code = GetEmbeddedPythonScript("GetClassicShellMRU.py");
            eng.Execute(code, scope);
            dynamic parseMRU = scope.GetVariable("parseMRU");
            dynamic me = parseMRU(NTUserPath);
            WriteObject(me);
        }


    }
}
