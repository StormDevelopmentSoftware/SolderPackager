using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

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
                    WriteCenter("There are no jar files in that directory!!");

                else
                {
                    WriteCenter($"All jar files in directory {path}: ", 1);

                    foreach (var item in d.GetFiles("*.jar"))
                        WriteCenter($"{Path.GetFileNameWithoutExtension(item.Name)} - {item.CreationTime.ToShortDateString()} {item.CreationTime.ToShortTimeString()}");

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

                    var basehtml = "<!DOCTYPE html><html><head> <title>Solder Packager Information</title> <meta charset=\"utf-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"> <link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css\"> <script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js\"></script> <script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js\"></script></head><body> <div class=\"container\"> <div class=\"row text-center\" style=\"\"> <h1>Solder Packager report<p>&nbsp; </p></h1> <table id=\"tablePreview\" class=\"table\"> <thead> <tr> <th>No</th> <th style=\"text-align: center;\">Mod Name</th> <th style=\"text-align: center;\">Mod Version</th> </tr></thead> <tbody>";
                    var endhtml = @"</tr></tbody><!--Table body--></table><!--Table--></div></div></body></html>";

                    int modNumber = 0;

                    var files = d.GetFiles("*.jar").OrderBy(fi => fi.Name).ToArray();
                    foreach (var item in files)
                    {
                        var content = Path.GetFileNameWithoutExtension(item.Name);
                        var replaced = (content.Replace('+', '-')).Replace('_', '-').Replace("'", "");
                        Regex regexDash = new Regex(@"(?<=-).*$");
                        Match matchDash = regexDash.Match(replaced);
                        modNumber++;
                        try
                        {
                            var foldername = replaced.Trim(new char[] { '+', '-', '_', '\'' }).Replace(matchDash.Value, "").Trim(new char[] { '+', '-', '_', '\'' });
                            Directory.CreateDirectory(zipPath + foldername.ToLower());
                            var ms = new MemoryStream();
                            var archive = new ZipArchive(ms, ZipArchiveMode.Create, true);
                            archive.CreateEntryFromFile(item.FullName, $"mods/{item.Name}");
                            ms.Seek(0, SeekOrigin.Begin);
                            File.WriteAllBytes($"{zipPath}{foldername.ToLower()}/{replaced.ToLower()}.zip", ms.ToArray());
                            basehtml += $"<tr> <th scope=\"row\">{modNumber}</th> <td>{foldername}</td><td>{matchDash.Value}</td></tr>";
                        }
                        catch
                        {
                            basehtml += $"<tr> <th scope=\"row\">{modNumber}</th> <td>{Path.GetFileNameWithoutExtension(item.Name)}</td><td>(no more info available)</td></tr>";
                            WriteCenter($"Couldn't figure out a folder to the mod {item.Name}", 2);
                            WriteCenter("so it's in the root folder of your solderzips.");
                            var ms = new MemoryStream();
                            var archive = new ZipArchive(ms, ZipArchiveMode.Create, true);
                            archive.CreateEntryFromFile(item.FullName, $"mods/{item.Name}");
                            ms.Seek(0, SeekOrigin.Begin);
                            File.WriteAllBytes($"{zipPath}{Path.GetFileNameWithoutExtension(item.Name).ToLower()}.zip", ms.ToArray());
                        }
                    }

                    basehtml += endhtml;
                    File.WriteAllText(zipPath + "report.html", basehtml);
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.Black;
                    WriteCenter("Success!", 2);
                    Console.BackgroundColor = ConsoleColor.Green;
                    WriteCenter("Keep in mind that file names are the same from the", 1);
                    WriteCenter("jar files, but with the zip extension. You might");
                    WriteCenter("need to change the file names.");
                    WriteCenter("A HTML file has been created with information.", 1);
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
