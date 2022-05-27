Import-Module .\Chimera.dll -Force

Get-Command -Module Chimera
Get-Module Chimera
$StickyNoteFile = "Example_StickyNotes.snt"
# If you are using windows 8 or earlier, and you have available sticky notes, this will work.
# Windows 10, the sticky notes are contained in a SQLite3 database. 
# $StickyNoteFile =  "$($env:AppData)\Microsoft\Sticky Notes\StickyNotes.snt" 

$data = Get-StickyNoteContents -FilePath $StickyNoteFile
$data

Write-Host "you can now experiment with the Get-StickyNoteContents cmdlet. You can hit breakpoints if this was launched from visual studio in debug mode. "