using System;
using System.IO;
using System.Collections.Generic;
//using TagLib;
using System.Linq;

namespace MusicSort
{
    class Program
    {
        static byte[] StringToByteArray(string str)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(str);
        }
        static void getFiles(string dirPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);

            List<FileInfo> mp3Files = new List<FileInfo>(directoryInfo.GetFiles("*.mp3"));      //Alle Dateien bekommen
            List<DirectoryInfo> directories = new List<DirectoryInfo>(directoryInfo.GetDirectories()); //Alle Directories bekommen
            string logFilePath = dirPath + "\\MusicSortLog.txt";
            StreamWriter logFile = File.AppendText(logFilePath);
            foreach(FileInfo x in mp3Files)  //Jeden File bearbeiten
            {
                bool addedToDir =false;
                //Console.WriteLine(x.Name);
                string artist = "";
                try
                {
                    var currFile = TagLib.File.Create(dirPath + "\\" + x.Name);  //Tag File createn, um Tags zu überprüfen
                    try
                    {
                        artist = currFile.Tag.Performers[0];  //Künstlertag
                    }
                    catch (IndexOutOfRangeException indexOutOfRangeException)
                    {
                        logFile.WriteLine(x.Name);
                        int splitIndex = x.Name.LastIndexOf('-');
                        if (splitIndex > 0)
                        {
                            string newArtist = x.Name.Substring(0, splitIndex);
                            currFile.Tag.Performers = new string[] { newArtist };
                            currFile.Save();
                        }
                        Console.WriteLine("No Artist " + indexOutOfRangeException);
                        continue;  //Iteration überspringen
                    }

                    foreach (DirectoryInfo dirInfo in directories)
                    {
                        if (dirInfo.Name == artist)  //Wenn bereits Ordner mit Künstlernamen existiert
                        {
                            Console.WriteLine("Moved: {0} to Directory {1}", x.Name, dirInfo.Name);
                            File.Move(dirPath + "\\" + x.Name, dirPath + "\\" + dirInfo.Name + "\\" + x.Name);  //Datei in Ordner moven
                            addedToDir = true;
                            break;
                        }

                    }
                    if (addedToDir == false)
                    {
                        try
                        {
                            DirectoryInfo newDir = Directory.CreateDirectory(dirPath + "\\" + currFile.Tag.Performers[0]);
                            directories.Add(newDir);
                            File.Move(dirPath + "\\" + x.Name, dirPath + "\\" + newDir.Name + "\\" + x.Name);  //Datei in Ordner moven
                        }
                        catch (IOException iOException)
                        {
                            Console.WriteLine("Fehler beim Erstellen eines neuen Ordners " + iOException);
                            logFile.WriteLine(x.Name);
                            continue;
                        }
                    }
                }
                catch (FileNotFoundException fileNotFoundException)
                {
                    Console.WriteLine("File does not exist " + fileNotFoundException);
                    logFile.WriteLine(x.Name);
                    continue;
                }
 //               str += x.Name + ", \n";
            }
            logFile.Close();
            
//            Console.WriteLine(str);
        }
        static void Main(string[] args)
        {
            //Console.WriteLine(test.Tag.Artists[0]);
            getFiles(args[0]);
        }
    }
}
