import olefile
import json
import pprint
import olefile
from striprtf.striprtf import rtf_to_text
from System.Collections.Generic import Dictionary
from System.Collections import ArrayList

# The code below is a mixture adapted from the following sources
# Devon's 2nd Article (Most Relevant) https://blogs.ethz.ch/bkeitch/2015/02/05/read-a-windows-7-sticky-note/
# Jeff's Article https://github.com/dingtoffee/StickyParser/blob/master/stickyparser.py
# Devon's First Article https://gist.github.com/daddycocoaman/e1a4f31109e17188d5ce8fd0ca15b63e
# Devon's First Article and Jeffs Article both reference https://www.tutorialspoint.com/python_digital_forensics/python_digital_forensics_important_artifacts_in_windows

class Note:
    def __init__(self):
        self.created = None
        self.modified = None
        self.content = None
        self.guid = None


def SNTParse(file):
    ole = olefile.OleFileIO(file)
    names = ole.listdir(True, True);
    magic = bytearray("{\\rtf", "ascii")  # magic number for RTF
    Notes = ArrayList()
    for storage in names:
        st_type = ole.get_type(storage)
        if st_type == olefile.STGTY_STREAM:
            note = ole.openstream(storage)
            data = note.read()
            dab = bytearray(data)
            if dab[:5] == magic:
                guid = storage[0]
                rtf = data.decode()
                text = rtf_to_text(data.decode()).rstrip('\x00').rstrip('\n')
                Note= {
                    "content": text,
                    "created": ole.getctime(guid),
                    "modified": ole.getmtime(guid),
                    "guid": guid,
                    "rtf": rtf
                }
                #thisNote = Note()
                #thisNote.created = getctime(guid)
                #thisNote.modified = getmtime(guid)
                #thisNote.content = text
                #thisNote.guid = guid
                #Notes.Add(thisNote)
                dN = Dictionary[str, object](Note)
                Notes.Add(dN)
    ole.close()
    return Notes