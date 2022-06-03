using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
namespace Chimera
{
    [Cmdlet("Get","PythonLibraryPaths")]
    public class GetPythonLibraryPaths : StaticPythonCmdlet
    {
        public GetPythonLibraryPaths()
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
            ICollection<string> paths = eng.GetSearchPaths();
            foreach(string path in paths)
            {
                WriteObject(path);
            }
        }
    }

    [Cmdlet("Add", "PythonLibraryPath")]
    public class AddPythonLibraryPath : StaticPythonCmdlet
    {
        [Parameter(Position = 0)]
        public string LibraryPath { get; set; }

        public AddPythonLibraryPath()
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
            if (eng.GetSearchPaths().Contains(LibraryPath))
            {
                Exception me = new Exception($"Library Path: {LibraryPath} already listed.");
                ErrorRecord errorRecord = new ErrorRecord(me, "1", ErrorCategory.ResourceExists, this);
                WriteError(errorRecord);
                return;
            }
            ICollection<string> paths = eng.GetSearchPaths();
            paths.Add(LibraryPath);
            eng.SetSearchPaths(paths);
            WriteObject(paths);
        }
    }

    [Cmdlet("Remove", "PythonLibraryPath")]
    public class RemovePythonLibraryPath : StaticPythonCmdlet
    {
        [Parameter(Position = 0)]
        public string LibraryPath { get; set; }

        public RemovePythonLibraryPath()
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
            ICollection<string> paths = eng.GetSearchPaths();
            if (!paths.Contains(LibraryPath))
            {
                Exception me = new Exception($"Library Path: {LibraryPath} Not Found.");
                ErrorRecord errorRecord = new ErrorRecord(me, "1", ErrorCategory.ObjectNotFound, this);
                WriteError(errorRecord);
                return;
            }
            paths.Remove(LibraryPath);
            eng.SetSearchPaths(paths);
            WriteObject(paths);
        }
    }
}
