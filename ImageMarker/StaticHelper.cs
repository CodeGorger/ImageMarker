using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMarker
{
    class StaticHelper
    {
        public static void SplitNameAndPath(string inPathAndName, out string outPath, out string outName)
        {
            if (inPathAndName == "")
            {
                outPath = "";
                outName = "";
                return;
            }
            string tmpPath = "";
            string[] inPathSplit = inPathAndName.Split(new Char[] { '\\' });
            for (int i = 0; i <= inPathSplit.Count() - 3; i++)
            {
                tmpPath += inPathSplit[i];
                tmpPath += "\\";
            }
            tmpPath += inPathSplit[inPathSplit.Count() - 2];
            outPath = tmpPath;
            outName = inPathSplit[inPathSplit.Count() - 1];
        }
        public static bool CheckIsArchive(string fileName)
        {
            string[] inPathSplit = fileName.Split(new Char[] { '.' });
            string fileType = inPathSplit[inPathSplit.Count() - 1].ToLower();
            if (fileType == "zip")
            {
                return true;
            }
            if (fileType == "rar")
            {
                return true;
            }
            if (fileType == "7z")
            {
                return true;
            }
            return false;
        }

        // if its folder marking, the filename is markings.xml
        public static bool IsFolderMarking(string fileName)
        {
            return (fileName == "markings.xml");
        }
    }
}
