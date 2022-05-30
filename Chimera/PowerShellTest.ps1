Import-Module .\Chimera.dll -Force

Get-Command -Module Chimera
Get-Module Chimera

Invoke-InternalPython -FileName "Test.py"
#Invoke-InternalPython -FileName "New.Code.py"

$StickyNoteFile = "Example_StickyNotes.snt"
$Names = "John,Mark,Luke,Cayley,Paul,Harry Potter,Steven,Karl,Jarvis,Navi,Bert,Alex,Cleopatra,Charlie,SniffyCat" -split ","
$Names | Invoke-ExamplePyCmdlet -TimeOfDay 1
$Names | Invoke-ExamplePyCmdlet -TimeOfDay 2
$Names | Invoke-ExamplePyCmdlet -TimeOfDay 3
# If you are using windows 8 or earlier, and you have available sticky notes, this will work.
# Windows 10, the sticky notes are contained in a SQLite3 database. 
# $StickyNoteFile =  "$($env:AppData)\Microsoft\Sticky Notes\StickyNotes.snt" 

#$data = Get-StickyNoteContents -FilePath $StickyNoteFile
#$data

#Write-Host "you can now experiment with the Get-StickyNoteContents cmdlet. You can hit breakpoints if this was launched from visual studio in debug mode. "