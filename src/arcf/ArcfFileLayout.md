# Arcf file layout

string extension - String used for identifying arcf files
uint32 version - Arcf file version number

Keep reading the next string until \eaf.

## \d - start directory
    followed by:
        string name - directory name

## \f - start file
    followed by:
        string name - file name
        long fullLength - length of the data in bytes
        long deflatedLength - length of the DEFLATED data in bytes (stored in file)
        data

## \ed - end directory

## \eaf - end archive file