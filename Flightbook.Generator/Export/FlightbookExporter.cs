using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Flightbook.Generator.Export
{
    public interface IFlightbookExporter
    {
        bool Export(string flightbookJson, string trackLogListJson, Dictionary<string, string> trackLogFileJson, string heatmapJson, string airportsToCollect, string cfAnalytics);
    }

    internal class FlightbookExporter : IFlightbookExporter
    {
        public bool Export(string flightbookJson, string trackLogListJson, Dictionary<string, string> trackLogFileJson, string heatmapJson, string airportsToCollect, string cfAnalytics)
        {
            string flightbookDir = "flightbook";
            string configDir = "config";
            string outputDir = GetOutputDir();

            if (string.IsNullOrEmpty(outputDir))
            {
                return false;
            }

            CleanTarget(outputDir);
            CopyFramework(flightbookDir, outputDir);
            ExportJson(flightbookJson, outputDir);
            ExportTrackLogs(trackLogListJson, trackLogFileJson, outputDir);
            ExportHeatmap(heatmapJson, outputDir);
            ExportAirportsToCollect(airportsToCollect, outputDir);
            CopyOtherFiles(configDir, outputDir);
            InjectCfAnalytics(outputDir, cfAnalytics);

            return true;
        }

        private string GetOutputDir()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string[] dirs = Directory.GetDirectories(currentDirectory);

            return dirs.FirstOrDefault(d => d.Replace($"{currentDirectory}\\", "").StartsWith("flightbook."));
        }

        private void CleanTarget(string outputDir)
        {
            DirectoryInfo directory = new(outputDir);
            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Name == ".git")
                {
                    continue;
                }

                file.Delete();
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                if (subDirectory.Name is ".git" or "node_modules")
                {
                    continue;
                }

                subDirectory.Delete(true);
            }
        }

        private void CopyFramework(string frameworkDir, string outputDir)
        {
            DirectoryCopy(Path.Join(frameworkDir, "public"), Path.Join(outputDir, "public"), true);
            DirectoryCopy(Path.Join(frameworkDir, "src"), Path.Join(outputDir, "src"), true);
            DirectoryCopy(Path.Join(frameworkDir, ".yarn", "patches"), Path.Join(outputDir, ".yarn", "patches"), true);
            DirectoryCopy(Path.Join(frameworkDir, ".yarn", "plugins"), Path.Join(outputDir, ".yarn", "plugins"), true);
            DirectoryCopy(Path.Join(frameworkDir, ".yarn", "releases"), Path.Join(outputDir, ".yarn", "releases"), true);
            DirectoryCopy(Path.Join(frameworkDir, ".yarn", "versions"), Path.Join(outputDir, ".yarn", "versions"), true);
            CopyFile(frameworkDir, outputDir, "package.json");
            CopyFile(frameworkDir, outputDir, ".yarnrc.yml");
            CopyFile(frameworkDir, outputDir, "tsconfig.json");
            CopyFile(frameworkDir, outputDir, ".editorconfig");
            CopyFile(frameworkDir, outputDir, ".prettierrc");
            CopyFile(frameworkDir, outputDir, ".prettierignore");
            CopyFile(frameworkDir, outputDir, ".gitignore");
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
                foreach (DirectoryInfo subDir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subDir.Name);
                    DirectoryCopy(subDir.FullName, tempPath, true);
                }
            }
        }

        private void ExportJson(string flightbookJson, string outputDir)
        {
            using StreamWriter outputFile = new(Path.Join(outputDir, @"src\data\flightbook.json"), false, Encoding.UTF8);
            outputFile.Write(flightbookJson);
            outputFile.Flush();
        }

        private void ExportTrackLogs(string trackLogListJson, Dictionary<string, string> trackLogFileJson, string outputDir)
        {
            using StreamWriter outputFile = new(Path.Join(outputDir, @"src\data\tracklogs.json"), false, Encoding.UTF8);
            outputFile.Write(trackLogListJson);
            outputFile.Flush();

            Directory.CreateDirectory(Path.Join(outputDir, @"public\tracklogs"));

            foreach ((string filename, string content) in trackLogFileJson)
            {
                using StreamWriter trackLogOutputFile = new(Path.Join(outputDir, @"public\tracklogs\", filename), false, Encoding.UTF8);
                trackLogOutputFile.Write(content);
                trackLogOutputFile.Flush();
            }
        }

        private void ExportHeatmap(string heatmapJson, string outputDir)
        {
            using StreamWriter outputFile = new(Path.Join(outputDir, @"src\data\heatmap.json"), false, Encoding.UTF8);
            outputFile.Write(heatmapJson);
            outputFile.Flush();
        }

        private void ExportAirportsToCollect(string airportsToCollect, string outputDir)
        {
            using StreamWriter outputFile = new(Path.Join(outputDir, @"src\data\airports.json"), false, Encoding.UTF8);
            outputFile.Write(airportsToCollect);
            outputFile.Flush();
        }

        private void CopyOtherFiles(string configDir, string outputDir)
        {
            CopyIfExists(configDir, "icon.png", outputDir, @"public\icon.png");
            CopyIfExists(configDir, "icon.svg", outputDir, @"public\icon.svg");
            CopyIfExists(configDir, "logo.svg", outputDir, @"public\logo.svg");
            CopyIfExists(configDir, "home.md", outputDir, @"public\home.md");

            if (Directory.Exists(Path.Join(configDir, "aircraft")))
            {
                DirectoryCopy(Path.Join(configDir, "aircraft"), Path.Join(outputDir, @"public\aircraft"), true);
            }

            if (Directory.Exists(Path.Join(configDir, "operators")))
            {
                DirectoryCopy(Path.Join(configDir, "operators"), Path.Join(outputDir, @"public\operators"), true);
            }

            if (Directory.Exists(Path.Join(configDir, "airports")))
            {
                DirectoryCopy(Path.Join(configDir, "airports"), Path.Join(outputDir, @"public\airports"), true);
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

        private void InjectCfAnalytics(string outputDir, string cfAnalytics)
        {
            if (string.IsNullOrEmpty(cfAnalytics))
            {
                return;
            }

            string indexPath = Path.Join(outputDir, @"public\index.html");

            string indexHtml = File.ReadAllText(indexPath);
            indexHtml = indexHtml.Replace("</body>", $"{cfAnalytics}{Environment.NewLine}</body>");
            File.WriteAllText(indexPath, indexHtml);
        }
    }
}
