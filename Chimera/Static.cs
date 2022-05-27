﻿using System;
using System.Collections.Generic;

using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;

namespace Chimera
{
    public class StaticCmdlet : Cmdlet
    {
        private static List<Assembly> LoadedAssemblies = new List<Assembly>();


        protected static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(args.Name);
            var ResourceNames = executingAssembly.GetManifestResourceNames();
            Trace.WriteLine($"Trying to load - {args.Name}");
            foreach (Assembly current in LoadedAssemblies)
            {
                if (current.GetName().Name == assemblyName.Name)
                {
                    Trace.WriteLine($"{args.Name} already loaded in App Domain. Passing Reference to - {current.GetName().Name}");
                    return current;
                }
            }

            byte[] myLoad;
            myLoad = TryGetBytes(executingAssembly, args.Name);
            if (myLoad == null)
            {
                return null;
            }
            Assembly myLoadedAssembly = Assembly.Load(myLoad);

            LoadedAssemblies.Add(myLoadedAssembly);
            return myLoadedAssembly;
        }

        private static byte[] TryGetBytes(Assembly executingAssembly, string name)
        {
            AssemblyName assemblyName = new AssemblyName(name);
            string path1 = "WPF_GUI.Assemblies." + assemblyName.Name + ".dll";
            string path2 = assemblyName.Name + ".dll";
            byte[] assemblyRawBytes;
            using (Stream stream = executingAssembly.GetManifestResourceStream(path1))
            {
                if (stream != null)
                {
                    assemblyRawBytes = new byte[stream.Length];
                    stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                    Trace.WriteLine($"Static - Found Embedded Resource - {name} at {path1}");
                    return assemblyRawBytes;
                }
            }
            using (Stream stream = executingAssembly.GetManifestResourceStream(path2))
            {
                if (stream != null)
                {
                    Trace.WriteLine($"Static - Found Embedded Resource - {name} at {path2}");
                    assemblyRawBytes = new byte[stream.Length];
                    stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                    return assemblyRawBytes;
                }

            }
            Trace.WriteLine($"failed to load - {name}");
            return null;
        }

        protected static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Trace.WriteLine($"Loaded - {args.LoadedAssembly.FullName}");
            return;
        }

        protected static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string message = $"Last Chance - Unhandled Exception Handler Encountered: {e}";
            var exception = (e.ExceptionObject as Exception);
            if (exception != null)
            {
                message += $"\nException Message:\n{exception.Message}";
            }
            Trace.WriteLine(message);
        }
    }
}
