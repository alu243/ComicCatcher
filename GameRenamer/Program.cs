using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
namespace GameRenamer
{
    class Program
    {
        static void Main(string[] args)
        {
            Rename(@".\");
        }

        static void Rename(string path)
        {
            DirectoryInfo rootDir = new DirectoryInfo(path);
            FileInfo[] files;
            files = rootDir.GetFiles();
            foreach (var file in files)
            {

                string dirName = file.DirectoryName;
                string fileName = file.Name;
                if (0 == String.Compare(fileName, "GameRenamer.exe", true)) continue;
                string fileName2 = GetNewName(fileName);
                file.MoveTo(Path.Combine(dirName, fileName2));
            }
            DirectoryInfo[] dirs;
            dirs = rootDir.GetDirectories();
            foreach (var dir in dirs)
            {
                Rename(dir.FullName);
                //string dirName = dir.Name;
                //if (0 == String.Compare(dirName, "GameRenamer.exe", true)) continue;
                //string dirName2 = GetNewName(dirName);
                //dir.MoveTo(dirName2);
            }
        }

        static string GetNewName(string oldName)
        {
            string newName = oldName;
            if (newName.Contains("18禁ゲーム"))
            {
                newName = "(18禁ゲーム)" + newName.Replace("(18禁ゲーム)", "").Replace("[18禁ゲーム]", "");
            }
            newName = newName.Replace(") [", ")[").Replace("] [", "][").Replace("] ", "]").Replace(" [", "[")
                .Replace(") ", ")").Replace(" (", "(");
            return newName;
        }

    }
}
