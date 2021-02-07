using System;
using System.IO;
using System.Text.RegularExpressions;
using static WordProcessor.ExtensionsAndHelpers;

namespace WordProcessor
{
    public static class FileHandler
    {
        public static bool IsValidFilePath(string potentialFilePath) 
        { 
            var filePathRegex = new Regex(@"^(?:[\w]\:|\\)(\\[a-z_\-\s0-9\.]+)+\.txt$", RegexOptions.IgnoreCase);
            if (filePathRegex.IsMatch(potentialFilePath))
            {
                return true;
            }
            else
            {
                PromptKey($"File path format is not valid: {potentialFilePath}");
                return false;
            }
        }

        public static bool IsValidDirectory(string potentialDirectory) 
        { 
            return IsValidFilePath(Path.Combine(potentialDirectory, "FileName.txt"));
        }
        public static Document Open(string filePath)
        {
            if (File.Exists(filePath))
            {
                string title = Path.GetFileName(filePath);
                string body = File.ReadAllText(filePath);
                return new Document(title, body);
            }
            else
            {
                Console.WriteLine($"Could not find file path: {filePath}");
                return null;
            }
        }

        public static void Save(Document docToSave, string directory = null)
        {
            string checkAndResolvePathConflict(string filePath)
            {
                bool canOverwriteFile = false;
                while (File.Exists(filePath) && !canOverwriteFile)
                {
                    string overwriteResponse = PromptLineLoop($"A file already exists at {filePath}. Do you wish to overwrite it? (Y/N)", isYOrN);
                    if (overwriteResponse.ToLower() == "n")
                    {
                        
                        filePath = PromptLineLoop($"Enter new file path to save to:\n(Previous file path: {filePath})", IsValidFilePath);
                    }
                    else
                    {
                        canOverwriteFile = true;
                    }
                }
                return filePath;
            }

            if (directory == null) directory = Directory.GetCurrentDirectory();
            Directory.CreateDirectory(directory);
            string filePath = Path.Combine(directory, docToSave.Title + ".txt");
            filePath = checkAndResolvePathConflict(filePath);
            File.WriteAllText(filePath, docToSave.Body);
            PromptKey("Document saved!");
        }
    }
}