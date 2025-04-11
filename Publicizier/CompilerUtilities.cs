using ModHelper.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;

namespace ModHelper.Publicizier;
public static class CompilerUtilities
{
    public static Dictionary<string, string?> FindReferencePaths(List<string> references, List<string> targetNames)
    {
        var results = new Dictionary<string, string?>();
        var remainingToFind = new HashSet<string>(targetNames);

        // 1. Trying to find by filename
        foreach (var reference in references)
        {
            var filename = Path.GetFileNameWithoutExtension(reference);
            if (remainingToFind.Contains(filename))
            {
                results[filename] = reference;
                remainingToFind.Remove(filename);

                Log.Info($"Found {filename} by filename in {reference}");
            }
        }

        // 2. Trying to find by inner AssemblyName
        if (remainingToFind.Count > 0)
        {
            foreach (var reference in references)
            {
                if (results.ContainsValue(reference)) continue; // вже знайдено

                try
                {
                    using var stream = File.OpenRead(reference);
                    using var peReader = new PEReader(stream);
                    if (!peReader.HasMetadata) continue;

                    var metadataReader = peReader.GetMetadataReader();
                    var assemblyDef = metadataReader.GetAssemblyDefinition();
                    var internalName = metadataReader.GetString(assemblyDef.Name);

                    if (remainingToFind.Contains(internalName))
                    {
                        results[internalName] = reference;
                        remainingToFind.Remove(internalName);

                        Log.Info($"Found {internalName} by internal name in {reference}");

                        if (remainingToFind.Count == 0)
                            break;
                    }
                }
                catch
                {
                    // not a valid .dll or corrupted, ignore
                }
            }
        }

        foreach (var missing in remainingToFind)
        {
            results[missing] = null;
            Log.Warn($"Failed to find {missing}");
        }

        return results;
    }

    public static string FindCsprojFile(string name, string[] files)
    {
        // 1. Find the shortest path of a .cs files
        string shortestPath = files.OrderBy(f => f.Count(c => c == Path.DirectorySeparatorChar)).FirstOrDefault()
            ?? throw new Exception("No .cs files found.");

        string currentDirectory = Path.GetDirectoryName(shortestPath);
        if (currentDirectory == null)
            throw new Exception("Cannot determine directory.");

        // 2. Check if the current directory matches the mod name
        string currentFolderName = Path.GetFileName(currentDirectory);
        if (string.Equals(currentFolderName, name, StringComparison.OrdinalIgnoreCase))
        {
            string expectedCsproj = Path.Combine(currentDirectory, name + ".csproj");
            if (File.Exists(expectedCsproj))
                Log.Info($"Found {expectedCsproj} by shortest file");
            return expectedCsproj;
        }

        // 3. Check if the current directory is a subdirectory of the mod name
        var matchingDirectories = Directory.GetDirectories(currentDirectory, name, SearchOption.AllDirectories);

        // 4. Checking all directories under the current directory that match the mod name
        foreach (var dir in matchingDirectories.OrderByDescending(d => d.Count(c => c == Path.DirectorySeparatorChar)))
        {
            string expectedCsproj = Path.Combine(dir, name + ".csproj");
            if (File.Exists(expectedCsproj))
            {
                Log.Info($"Found {expectedCsproj} by searching in directories (this is bad actually)");
                return expectedCsproj;
            }
        }


        throw new FileNotFoundException($"Could not find a {name}.csproj file.");
    }

    // Create a hash of the file using MD5
    public static string ComputeHash(string filePath)
    {
        using var algorithm = MD5.Create();
        using var stream = File.OpenRead(filePath);
        byte[] computedHash = algorithm.ComputeHash(stream);
        var sb = new StringBuilder();
        foreach (byte b in computedHash)
        {
            sb.Append($"{b:X2}");
        }
        string hexadecimalHash = sb.ToString();
        return hexadecimalHash;
    }

    // TODO: create a better folder management system
    public static string GetPRFolderPath(string fileName)
    {
        return Utilities.GetFolderPath("ModHelper/PublicizedReferences", fileName);
    }

}
