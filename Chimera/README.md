# Chimera
Chimera is a PowerShell Module, that has an embedded IronPython3 interpreter, as well as the site packages, and any pip packages. 
Additionally, python source files can be embedded as resources into the DLL __OR__ it can reference files on the local file system.

The chimera module can be imported in powershell like this:
```PowerShell
Import-Module .\Chimera.dll
```

The first use case of this library was to create the cmdlet ``Get-StickyNoteContents``. Get-StickyNoteContents is a wrapper around code.py, which can be found in the 📁``.\Chimera\Python`` Folder. Windows 7/8 Desktop Sticky notes are using an ole2 file format. Extracting data from this file format was difficult in c# and powershell, but was incredibly straightforward with the olefile python library. As a result, rather than reverse engineering olefile to work in c# or powershell, I found it easier and more interesting to just bundle an IronPython3 interpreter right into a powershell module, and let python do the magic. 

I enjoyed the work so much, and found IronPython3 and the C# Dynamic Language Runtime, that I decided to extend the use of this code, so that it would be beneficial for future projects. 

# Building Chimera
## Requirements:
IronPython3 Must be installed in order to compile the DLL, however this is not required for running the module. 
- https://github.com/IronLanguages/ironpython3/releases/tag/v3.4.0-beta1

## Embedding Python Scripts
All python files added in the 📁``.\Chimera\Python`` folder will be embedded into the resulting chimera.dll. 


# Chimera - Guided Tour
## How Chimera Embeds Resources
There are 3 parts to Chimera's resource embedding. First are the embedded references / class libraries. Second is the lib.zip, which should contain the necessary builtin python libraries and site-packages (anything installed with pip, easy_install or setup.py/distutils). Third are the embedded python scripts. The embedding is controlled by the chimera.csproj file at build time.


### Embedded Python Scripts 🐍
The following line in chimera.csproj will embed python scripts:
```xml
	<ItemGroup>
	<EmbeddedResource Include="Python\*.py" />
	</ItemGroup>
```

The scripts can then be used from within the class library by subclassing ``StaticPythonCmdlet``. 
The following functions and variables will be made available within your class.
1. ``GetEmbeddedPythonScript`` - Get's the embedded python script
2. eng - [IronPython Engine](https://documentation.help/IronPython/engine.html). Static Instance
3. scope - [IronPython Scope](https://documentation.help/IronPython/scopes.html). Static Instance.

❌ - I will eventually wrap these in an interface or other methods, but for now, this is what works. 

### Embedded References and Assemblies
Any class library or resource that would normally be copied to the bin directory, will instead embed those assemblies into chimera.dll if the property StaticBuild is set to true.. This is handled by 📁``chimera.csproj`` as well. This code happens below the line ``<Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />``. For Chimera, static build is set to true in both Debug and Release modes. 

The chimera.csproj file will embed any assemblies installed via NuGet, and should in theory, embed any internal resources that are linked, including links to other projects. For example, ``System.Management.Automation`` is embedded into the resulting chimera.dll. 

The BEST way to think of this, is like static compiling and linking in languages like C++ or C. 

#### MSBuild / chimera.csproj source.
```xml
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <!-- Code Omitted --> 
    <StaticBuild>true</StaticBuild>
    <!-- Code Omitted --> 
  </PropertyGroup>
    <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <!-- Code Omitted --> 
    <StaticBuild>true</StaticBuild>
    <!-- Code Omitted --> 
  </PropertyGroup>
```

```xml
<!-- Code Omitted --> 
<Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
  <!-- Begin Shifty -->
  <Target Name="EmbedReferencedAssemblies" AfterTargets="ResolveAssemblyReferences" Condition=" '$(StaticBuild)' == 'true' ">
    <ItemGroup>
      <!-- get list of assemblies marked as CopyToLocal -->
      <FilesToEmbed Include="@(ReferenceCopyLocalPaths)" Condition="('%(ReferenceCopyLocalPaths.Extension)' == '.dll' Or '%(ReferenceCopyLocalPaths.Extension)' == '.pdb')" />
      <FilesToExclude Include="@(ReferenceCopyLocalPaths)" Condition="'%(ReferenceCopyLocalPaths.Extension)' == '.xml'" />
      <!-- add these assemblies to the list of embedded resources -->
      <EmbeddedResource Include="@(FilesToEmbed)">
        <LogicalName>%(FilesToEmbed.DestinationSubDirectory)%(FilesToEmbed.Filename)%(FilesToEmbed.Extension)</LogicalName>
      </EmbeddedResource>
      <!-- no need to copy the assemblies locally anymore -->
      <ReferenceCopyLocalPaths Remove="@(FilesToEmbed)" />
      <ReferenceCopyLocalPaths Remove="@(FilesToExclude)" />
    </ItemGroup>
    <Message Importance="high" Text="[Shifty] Static Build Enabled. Embedding Resources." />
    <Message Importance="high" Text="Embedding: @(FilesToEmbed->'%(Filename)%(Extension)', ', ')" />
  </Target>
  <!-- End Shifty -->
```


### PowerShell Cmdlet with Embedded Python Example
Let's make a basic PowerShell cmdlet with an embedded python script.
Files to create: 
1. ExamplePyCmdlet.py - Add to project under the Python Folder.
2. ExampleCmdlet.cs - Add under the root directory of the project

#### ExamplePyCmdlet.py Source
```python
def greetings(name, timeOfDay): # greetings will only get called from ExampleCmdlet.cs
    # Time of Day:
    # 1 - Morning, 2 - Afternoon, 3 - Evening.  Any other value is replaced with "Day"
    greeting = None
    if timeOfDay == 1:
        greeting = "Morning"
    elif timeOfDay == 2:
        greeting = "Afternoon"
    elif timeOfDay == 3:
        greeting = "Evening"
    else:
        greeting = "Day"
    value = "Good {0} {1}".format(greeting, name)
    return value
    
print("hello world from ExamplePyCmdlet") # Gets called when loaded. Ideal for some scenarios
```
#### ExampleCmdlet.cs
```csharp
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

```
#### Running the cmdlet
We create a list of 15 random names, and run that list against ExamplePyCmdlet.py.
```PowerShell
$Names = "John,Mark,Luke,Cayley,Paul,Harry Potter,Steven,Karl,Jarvis,Navi,Bert,Alex,Cleopatra,Charlie,SniffyCat" -split ","
$Names | Invoke-ExamplePyCmdlet -TimeOfDay 1
$Names | Invoke-ExamplePyCmdlet -TimeOfDay 2
$Names | Invoke-ExamplePyCmdlet -TimeOfDay 3
```