using System;
using System.IO;
using System.IO.Compression;

namespace SolderPackager
{
    class Program
    {
        static void Main(string[] args)
        {

            WriteCenter("Solder Packager app");
            WriteCenter("by StormMC team");
            WriteCenter("It is recommended that you maximize your console window.", 1);
            WriteCenter("Please, specify the path to your mods folder.", 2);
            Console.Write("\n Path to mods folder> ");
            var path = Console.ReadLine();

            try
            {
                DirectoryInfo d = new DirectoryInfo(path);
                if (d.GetFiles("*.jar").Length == 0)
                {
                    WriteCenter("There are no jar files in that directory!!");
                }
                else
                {
                    WriteCenter($"All jar files in directory {path}: ", 1);

                    foreach (var item in d.GetFiles("*.jar"))
                    {
                        WriteCenter($"{Path.GetFileNameWithoutExtension(item.Name)} - {item.CreationTime.ToShortDateString()} {item.CreationTime.ToShortTimeString()}");
                    }

                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    WriteCenter("This program will create all zip files in a folder as the path specified,", 2);
                    WriteCenter("called \"solderzips\". Are you sure you want to do that? [y/N]");
                    Console.SetCursorPosition(Console.WindowWidth / 2, Console.CursorTop);
                    var confirmation = Console.ReadKey();

                    if (confirmation.KeyChar != 'y')
                    {
                        WriteCenter("Aborted.", 1);
                        Environment.Exit(0);
                    }

                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    WriteCenter("OK, working on that...", 2);
                    Console.ResetColor();
                    var zipPath = path + "/solderzips/";
                    Directory.CreateDirectory(zipPath);
                    foreach (var item in d.GetFiles("*.jar"))
                    {
                        var ms = new MemoryStream();
                        var archive = new ZipArchive(ms, ZipArchiveMode.Create, true);
                        archive.CreateEntryFromFile(item.FullName, $"mods/{item.Name}");
                        ms.Seek(0, SeekOrigin.Begin);
                        File.WriteAllBytes($"{zipPath}{Path.GetFileNameWithoutExtension(item.Name)}.zip", ms.ToArray());
                    }

                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.Black;
                    WriteCenter("Success!", 2);
                    Console.BackgroundColor = ConsoleColor.Green;
                    WriteCenter("Keep in mind that file names are the same from the");
                    WriteCenter("jar files, but with the zip extension. You might");
                    WriteCenter("need to change the file names.");
                }
            }
            catch
            {
                WriteCenter("There is no directory by that path!");
            }

        }

        internal static void WriteCenter(string value, int skipline = 0)
        {
            for (int i = 0; i < skipline; i++)
                Console.WriteLine();

            var pos = (Console.WindowWidth - value.Length) / 2;
            if (pos < 0)
            {
                Console.WriteLine(value);
            }
            else
            {
                Console.SetCursorPosition(pos, Console.CursorTop);
                Console.WriteLine(value);
            }

        }
    }
}
