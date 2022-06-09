Import-Module .\Chimera.dll -Force


## Testing Basic Registration of cmdlets
	Get-Command -Module Chimera
	Get-Module Chimera

## Test Get-ClassicShellMRU
	Get-ClassicShellMRU -NTUserPath "C:\Users\shiftybit\workspace\DLE2\NTUSER.DAT"

## Test Invoke-InternalPython
	Invoke-InternalPython -FileName "Test.py"
	#Invoke-InternalPython -FileName "New.Code.py"


## Test ExamplePyCmdlet
	$Names = "John,Mark,Luke,Cayley,Paul,Harry Potter,Steven,Karl,Jarvis,Navi,Bert,Alex,Cleopatra,Charlie,SniffyCat" -split ","
	#$Names | Invoke-ExamplePyCmdlet -TimeOfDay 1
	#$Names | Invoke-ExamplePyCmdlet -TimeOfDay 2
	#$Names | Invoke-ExamplePyCmdlet -TimeOfDay 3



## Test Get-StickyNoteContents
	# If you are using windows 8 or earlier, and you have available sticky notes, this will work.
	# Windows 10, the sticky notes are contained in a SQLite3 database. 
	### # $StickyNoteFile =  "$($env:AppData)\Microsoft\Sticky Notes\StickyNotes.snt"  
	$StickyNoteFile = "Example_StickyNotes.snt"
	#$data = Get-StickyNoteContents -FilePath $StickyNoteFile
	#$data

