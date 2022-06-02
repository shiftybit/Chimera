using System.Management.Automation;
namespace Chimera
{
    [Cmdlet(VerbsLifecycle.Invoke, "ExamplePyCmdlet")]
    public class ExampleCmdlet : StaticPythonCmdlet
    {
        private string code;

        // Create the InputString Parameter 
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string InputString { get; set; }

        // Create the Time of Day parameter
        [Parameter(Mandatory = true)]
        public int TimeOfDay { get; set; }
        public ExampleCmdlet() // Constructor MUST be public, otherwise the cmdlet will not be exported
        {
            string fileName = "ExamplePyCmdlet.py"; // Currently Case Sensitive. 
            code = GetEmbeddedPythonScript(fileName);
            eng.Execute(code, scope); // Should immediately print "hello world from ExamplePyCmdlet"
        }
        protected override void ProcessRecord()
        {
            dynamic greetings = scope.GetVariable("greetings");
            WriteObject(greetings(InputString, TimeOfDay)); 

        }
    }
}
