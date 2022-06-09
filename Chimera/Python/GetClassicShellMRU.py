import sys
import traceback
from System.Collections import ArrayList

try:
    from Registry import Registry
except:
    print(traceback.format_exc())
    print("exception encountered")


def parseMRU(ntuserDat):
    print("Attempting to parse " + ntuserDat)
    reg = Registry.Registry(ntuserDat)
    obj = ArrayList()
    try:
        key = reg.open("SOFTWARE\\IvoSoft\\ClassicStartMenu\\MRU")
    except:
        print("Couldn't find Classic Start Menu MRU Key")
    for value in [v for v in key.values() \
                    if v.value_type() == Registry.RegSZ or v.value_type() == Registry.RegExpandSZ]:
        #print("%s: %s" % (value.name(), value.value()))
        obj.Add(value.value())
    return obj