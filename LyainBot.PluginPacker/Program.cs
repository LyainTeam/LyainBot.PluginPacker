using System.IO.Compression;

namespace LyainBot.PluginPacker;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: lypacker <output-directory> <zip-file-name>");
            return;
        }

        string outputDirectory = args[0];
        string zipFileName = args[1];

        try
        {
            string?[] depsFiles = Directory.GetFiles(outputDirectory, "*.deps.json");
            string? depsFileName = depsFiles.Length > 0 ? depsFiles[0] : null;
            if (depsFileName == null)
            {
                Console.WriteLine("No .deps.json file found in the output directory.");
                return;
            }
            string dllFileName = Path.GetFileNameWithoutExtension(depsFileName).Replace(".deps", "") + ".dll";
            if (!File.Exists(Path.Combine(outputDirectory, dllFileName)))
            {
                Console.WriteLine($"DLL file {dllFileName} not found in the output directory.");
                return;
            }

            using (ZipArchive zip = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(Path.Combine(outputDirectory, dllFileName), dllFileName);
                zip.CreateEntryFromFile(depsFileName, Path.GetFileName(depsFileName));
            }

            string lyPluginFileName = Path.ChangeExtension(zipFileName, ".lyplugin");
            File.Move(zipFileName, lyPluginFileName);
            Console.WriteLine($"Successfully created {zipFileName}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}