# Modified from https://github.com/dingtoffee/StickyParser/blob/master/stickyparser.py
import json, sqlite3, olefile, datetime, argparse, sys, os, struct
# import pandas as pd # Pandas does not work in ironPython3


def snt(file):
    # https://www.tutorialspoint.com/python_digital_forensics/python_digital_forensics_important_artifacts_in_windows
    if not olefile.isOleFile(file):
        return "Invalid OLE file"

    ole = olefile.OleFileIO(file)
    note = {}
    now = datetime.datetime.now().strftime("%Y%m%d%H%M")
    for stream in ole.listdir():
        if stream[0].count("-") == 3:
            if stream[0] not in note:
                    note[stream[0]] = {"created": str(ole.getctime(stream[0])), "modified": str(ole.getmtime(stream[0]))}
                    content = None
            if stream[1] == '3':
                    content = ole.openstream(stream).read().decode("utf-16").rstrip("\u0000")
            if content:
                    note[stream[0]][stream[1]] = content

    db_df =  pd.read_json(json.dumps(note, indent=4, sort_keys=True))
    data_df= []
    column_df = []

    column_df = list(db_df.index.values)
    column_df[0] = 'text'
    column_df.insert(0,'id')
    temp_list = db_df.columns
    for i in range(len(list(db_df.columns.values))):
           data_df.append([temp_list[i],db_df.iloc[0,i],db_df.iloc[1,i],db_df.iloc[2,i]])
    final_df = pd.DataFrame(data = data_df , columns = column_df)
    print("StickyParser: Saving the csv file")
   
    final_df.to_csv(args.d+ 'stickynoteresultsnt-'+ now + '.csv', index=False)
    print("StickyParser: File saved.")

       
if __name__ == "__main__":

    parser = argparse.ArgumentParser(description="""StickyParser: Parses sticky note files in legacy snt formats.""")
    parser.add_argument('-s', nargs='?',metavar="snt file", help='Sticky note .snt file.', type=argparse.FileType('r'))
    parser.add_argument('-d' ,nargs='?',metavar="File Directory", help='Specify the directory where the output should write to. Example: StickyParser -p <path> -d C:\\Users\\User\\Desktop\\')
    args = parser.parse_args
           
    if args.s is not None:

        print('StickyPraser: Parsing the SNT File...')
        snt(args.s.name)

    if args.p is not None:

        print('StickyPraser: Parsing the sqlite file ....')
        plum(args.p.name)
       
    if len(sys.argv) < 2:
        parser.print_usage()
        sys.exit(1)
