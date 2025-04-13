using ModHelper.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
    internal static string ComputeHash(string assemblyPath, PublicizerAssemblyContext assemblyContext)
    {
        var sb = new StringBuilder();
        sb.Append(assemblyContext.AssemblyName);
        sb.Append(assemblyContext.IncludeCompilerGeneratedMembers);
        sb.Append(assemblyContext.IncludeVirtualMembers);
        sb.Append(assemblyContext.ExplicitlyPublicizeAssembly);
        sb.Append(assemblyContext.ExplicitlyDoNotPublicizeAssembly);
        foreach (string publicizePattern in assemblyContext.PublicizeMemberPatterns)
        {
            sb.Append(publicizePattern);
        }
        foreach (string doNotPublicizePattern in assemblyContext.DoNotPublicizeMemberPatterns)
        {
            sb.Append(doNotPublicizePattern);
        }
        if (assemblyContext.PublicizeMemberRegexPattern is not null)
        {
            sb.Append(assemblyContext.PublicizeMemberRegexPattern.ToString());
        }

        byte[] patternBytes = Encoding.UTF8.GetBytes(sb.ToString());
        byte[] assemblyBytes = File.ReadAllBytes(assemblyPath);
        byte[] allBytes = assemblyBytes.Concat(patternBytes).ToArray();

        return ComputeHash(allBytes);
    }

    private static string ComputeHash(byte[] bytes)
    {
        using var algorithm = MD5.Create();

        byte[] computedHash = algorithm.ComputeHash(bytes);
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

    public static Dictionary<string, PublicizerAssemblyContext> GetPublicizerAssemblyContexts(XDocument csproj)
    {
        var contexts = new Dictionary<string, PublicizerAssemblyContext>();
        XNamespace ns = csproj.Root!.Name.Namespace;

        var publicizeElements = csproj.Descendants(ns + "Publicize");
        var doNotPublicizeElements = csproj.Descendants(ns + "DoNotPublicize");

        foreach (var elem in publicizeElements)
        {
            string? itemSpec = elem.Attribute("Include")?.Value;
            if (string.IsNullOrEmpty(itemSpec)) continue;

            int index = itemSpec.IndexOf(':');
            bool isAssemblyPattern = index == -1;
            string assemblyName = isAssemblyPattern ? itemSpec : itemSpec[..index];

            if (!contexts.TryGetValue(assemblyName, out var assemblyContext))
            {
                assemblyContext = new PublicizerAssemblyContext(assemblyName);
                contexts.Add(assemblyName, assemblyContext);
            }

            if (isAssemblyPattern)
            {
                assemblyContext.IncludeCompilerGeneratedMembers =
                    bool.TryParse(elem.Attribute("IncludeCompilerGeneratedMembers")?.Value, out var icgm) && icgm;
                assemblyContext.IncludeVirtualMembers =
                    bool.TryParse(elem.Attribute("IncludeVirtualMembers")?.Value, out var ivm) && ivm;
                assemblyContext.ExplicitlyPublicizeAssembly = true;
                var pattern = elem.Attribute("MemberPattern")?.Value;
                if (!string.IsNullOrEmpty(pattern))
                {
                    assemblyContext.PublicizeMemberRegexPattern = new Regex(pattern);
                }

                Log.Info($"Publicize: {itemSpec}, virtual members: {assemblyContext.IncludeVirtualMembers}, " +
                         $"compiler-generated members: {assemblyContext.IncludeCompilerGeneratedMembers}, " +
                         $"member pattern: {assemblyContext.PublicizeMemberRegexPattern}");
            }
            else
            {
                string memberPattern = itemSpec[(index + 1)..];
                assemblyContext.PublicizeMemberPatterns.Add(memberPattern);
                Log.Info($"Publicize: {itemSpec}");
            }
        }

        foreach (var elem in doNotPublicizeElements)
        {
            string? itemSpec = elem.Attribute("Include")?.Value;
            if (string.IsNullOrEmpty(itemSpec)) continue;

            int index = itemSpec.IndexOf(':');
            bool isAssemblyPattern = index == -1;
            string assemblyName = isAssemblyPattern ? itemSpec : itemSpec[..index];

            if (!contexts.TryGetValue(assemblyName, out var assemblyContext))
            {
                assemblyContext = new PublicizerAssemblyContext(assemblyName);
                contexts.Add(assemblyName, assemblyContext);
            }

            if (isAssemblyPattern)
            {
                assemblyContext.ExplicitlyDoNotPublicizeAssembly = true;
            }
            else
            {
                string memberPattern = itemSpec[(index + 1)..];
                assemblyContext.DoNotPublicizeMemberPatterns.Add(memberPattern);
            }

            Log.Info($"DoNotPublicize: {itemSpec}");
        }

        return contexts;
    }

}
