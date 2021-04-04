using System.IO;
using System.Linq;

namespace Flightbook.Generator.Export
{
    internal class FlightbookExporter : IFlightbookExporter
    {
        public void Export(string flightbookJson)
        {
            string flightbookDir = "flightbook";
            string configDir = "config";
            string outputDir = GetOutputDir();

            CopyFramework(flightbookDir, outputDir);
            ExportJson(flightbookJson, outputDir);
            CopyOtherFiles(configDir, outputDir);
        }

        private string GetOutputDir()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string[] dirs = Directory.GetDirectories(currentDirectory);

            return dirs.First(d => d.Replace($"{currentDirectory}\\", "").StartsWith("flightbook."));
        }

        private void CopyFramework(string frameworkDir, string outputDir)
        {
            DirectoryCopy(Path.Join(frameworkDir, "public"), Path.Join(outputDir, "public"), true);
            DirectoryCopy(Path.Join(frameworkDir, "src"), Path.Join(outputDir, "src"), true);
            CopyFile(frameworkDir, outputDir, "package.json");
            CopyFile(frameworkDir, outputDir, "tsconfig.json");
            CopyFile(frameworkDir, outputDir, ".editorconfig");
            CopyFile(frameworkDir, outputDir, ".prettierrc");
            CopyFile(frameworkDir, outputDir, ".prettierignore");
            CopyFile(frameworkDir, outputDir, ".eslintrc");
            CopyFile(frameworkDir, outputDir, "yarn.lock");
        }

        private void CopyFile(string frameworkDir, string outputDir, string file)
        {
            File.Copy(Path.Join(frameworkDir, file), Path.Join(outputDir, file), true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        private void ExportJson(string flightbookJson, string outputDir)
        {
            using StreamWriter outputFile = new(Path.Join(outputDir, @"src\data\flightbook.json"));
            outputFile.Write(flightbookJson);
            outputFile.Flush();
        }

        private void CopyOtherFiles(string configDir, string outputDir)
        {
            CopyIfExists(configDir, "icon.png", outputDir, @"public\icon.png");
            CopyIfExists(configDir, "logo.svg", outputDir, @"public\logo.svg");
            if (Directory.Exists(Path.Join(configDir, "aircrafts")))
            {
                DirectoryCopy(Path.Join(configDir, "aircrafts"), Path.Join(outputDir, @"public\aircrafts"), true);
            }
        }

        private void CopyIfExists(string configDir, string file, string outputDir, string outputPath)
        {
            string sourcePath = Path.Join(configDir, file);
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, Path.Join(outputDir, outputPath), true);
            }
        }
    }
}
