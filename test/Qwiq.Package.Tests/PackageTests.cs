using System.Reflection;

using NuGet.Versioning;

namespace Qwiq.Package.Tests;

public class PackageTests
{
    public static TheoryData<string> GetPackages()
    {
        // Find the solution root by looking for src directory
        // Start from the test assembly location and walk up
        DirectoryInfo? directory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
        DirectoryInfo? srcDirectory = null;

        while (directory != null)
        {
            DirectoryInfo candidate = new(Path.Combine(directory.FullName, "src"));
            if (candidate.Exists)
            {
                srcDirectory = candidate;
                break;
            }
            directory = directory.Parent;
        }

        if (srcDirectory == null)
        {
            throw new InvalidOperationException(
                "Could not find 'src' directory. Unable to locate NuGet packages.");
        }

        // Search for packages in src/**/bin/Release/**/*.nupkg
        FileInfo[] packages = srcDirectory.GetFiles("Qwiq*.nupkg", SearchOption.AllDirectories)
            .Where(f => f.FullName.Contains(Path.Combine("bin", "Release"), StringComparison.OrdinalIgnoreCase))
            .OrderBy(fileInfo => fileInfo.Name, StringComparer.Ordinal)
            .ToArray();

        if (packages.Length == 0)
        {
            throw new InvalidOperationException(
                "No Qwiq*.nupkg files were found. Ensure the pack step runs before executing this test. " +
                $"Searched in: {srcDirectory.FullName}");
        }

        TheoryData<string> theoryData = new();
        foreach (FileInfo package in packages)
        {
            theoryData.Add(package.FullName);
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(GetPackages))]
    public Task Baseline(string packagePath)
    {
        FileInfo package = new(packagePath);

        string discriminator = GetPackageDiscriminator(package.Name);

        return VerifyFile(package)
            .ScrubNuspec()
            .UseTextForParameters(discriminator);
    }

    private static string GetPackageDiscriminator(string packageName)
    {
        if (packageName.Contains(".symbols.nupkg", StringComparison.Ordinal))
        {
            string baseName = packageName.Replace(".symbols.nupkg", string.Empty, StringComparison.Ordinal);
            return $"{ExtractPackageName(baseName)}_symbols";
        }

        string name = packageName.Replace(".nupkg", string.Empty, StringComparison.Ordinal);
        return ExtractPackageName(name);
    }

    /// <summary>
    /// Extracts the package ID from a package filename (without extension).
    /// Uses NuGet's version parser to robustly handle semantic versions including
    /// prerelease tags (e.g., "1.0.0-beta") and package IDs that end with digits.
    /// </summary>
    /// <param name="fullName">Package filename without extension (e.g., "Qwiq.Core.10.0.32" or "Qwiq.Linq.Identity.10.0.32-beta")</param>
    /// <returns>The package ID portion (e.g., "Qwiq.Core" or "Qwiq.Linq.Identity")</returns>
    private static string ExtractPackageName(string fullName)
    {
        string[] parts = fullName.Split('.');
        if (parts.Length < 2)
        {
            return fullName;
        }

        // Find the earliest split point where the right-hand side parses as a NuGet version.
        // This handles package IDs with dots (e.g., "Qwiq.Linq.Identity") and IDs ending
        // with digits (e.g., "Qwiq.Core4") correctly.
        for (int i = 1; i < parts.Length; i++)
        {
            string candidateVersion = string.Join(".", parts, i, parts.Length - i);
            if (NuGetVersion.TryParse(candidateVersion, out _))
            {
                return string.Join(".", parts, 0, i);
            }
        }

        // If nothing looks like a version, fall back to returning the whole name.
        return fullName;
    }
}
