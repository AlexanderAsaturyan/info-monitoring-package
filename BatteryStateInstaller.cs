#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class BatteryStateInstaller
{
    static BatteryStateInstaller()
    {
        string sourcePath = "Packages/com.alexanderasaturyan.batterystate/Runtime/Android";
        string destinationPath = "Assets/Plugins/Android";

        try
        {
            if (!Directory.Exists(sourcePath))
            {
                Debug.LogWarning($"BatteryStateInstaller: Source path does not exist: {sourcePath}");
                return; // Exit if the source path is missing
            }

            // Ensure destination directory exists
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            // Copy everything from source to destination
            CopyDirectory(sourcePath, destinationPath);

            Debug.Log("BatteryStateInstaller: All files and directories copied successfully.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"BatteryStateInstaller: Failed to copy files. {ex.Message}");
        }
    }

    // Recursive method to copy directories (handles empty directories)
    static void CopyDirectory(string sourceDir, string destDir)
    {
        // Copy all directories
        foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            string relativePath = dir.Substring(sourceDir.Length + 1);
            string destinationSubDir = Path.Combine(destDir, relativePath);
            if (!Directory.Exists(destinationSubDir))
            {
                Directory.CreateDirectory(destinationSubDir);
            }
        }

        // Copy all files
        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            string relativePath = file.Substring(sourceDir.Length + 1);
            string destinationFile = Path.Combine(destDir, relativePath);
            File.Copy(file, destinationFile, true); // Overwrite if exists
        }
    }
}
#endif