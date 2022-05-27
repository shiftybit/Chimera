# Chimera
Chimera is a PowerShell Module, that entirely self contains IronPython3, the site packages, and any pip packages. 
Additionally, python source files can be embedded as resources into the DLL __OR__ it can reference files on the local file system.
 It can be imported with 
```PowerShell
Import-Module .\Chimera.dll
```

At this time of writing, it only has one available command. Get-StickyNoteContents. Get-StickyNoteContents is a wrapper around code.py, which can be found in the 📁``.\Chimera\Python`` Folder. You can add more python files and extend however you want. I will be adding more to this later, but for now, I've got it hard coded to work with this one specific use case, of migrating Windows 8.1 sticky note files. 

# Requirements for Building
IronPython3 Must be installed in order to compile the DLL, however this is not required for running the module. 
- https://github.com/IronLanguages/ironpython3/releases/tag/v3.4.0-beta1