using System.Reflection;

using NuGet.Versioning;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Qwiq.Package.Tests;

/// <summary>
/// Tests that verify NuGet package contents against verified baselines.
/// These tests require all target frameworks to be built and packed, so they
/// only run on Windows where net472 can be built.
/// </summary>
[Trait("TestCategory", "Package")]
public class PackageTests
{
    public static TheoryData<string> GetPackages()
    {
        // Find the solution root by looking for artifacts/package directory
        // Start from the test assembly location and walk up
        DirectoryInfo? directory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
        DirectoryInfo? packageDirectory = null;

        while (directory != null)
        {
            // Look for artifacts/package/release (where SDK places packages when ArtifactsPath is set)
            DirectoryInfo candidate = new(Path.Combine(directory.FullName, "artifacts", "package", "release"));
            if (candidate.Exists)
            {
                packageDirectory = candidate;
                break;
            }
            directory = directory.Parent;
        }

        if (packageDirectory == null)
        {
            // Fall back to looking for src directory for legacy compatibility
            directory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            throw new InvalidOperationException(
                "Could not find 'artifacts/package/release' directory. Unable to locate NuGet packages. " +
                $"Searched from: {directory?.FullName}");
        }

        // Search for both .nupkg and .snupkg packages in artifacts/package/release/
        FileInfo[] nupkgPackages = packageDirectory.GetFiles("Qwiq*.nupkg", SearchOption.TopDirectoryOnly);

        FileInfo[] snupkgPackages = packageDirectory.GetFiles("Qwiq*.snupkg", SearchOption.TopDirectoryOnly);

        if (nupkgPackages.Length == 0)
        {
            throw new InvalidOperationException(
                "No Qwiq*.nupkg or Qwiq*.snupkg files were found. Ensure the pack step runs before executing this test. " +
                $"Searched in: {packageDirectory.FullName}");
        }

        if (snupkgPackages.Length > 0)
        {
            Trace.TraceInformation(
                "Skipping baseline verification for {0} symbol packages pending Verify.Nupkg support. See https://github.com/MattKotsenas/Verify.Nupkg/issues/38.",
                snupkgPackages.Length);
        }

        FileInfo[] packages = nupkgPackages
            .GroupBy(fileInfo => GetPackageDiscriminator(fileInfo.Name), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.OrderByDescending(fileInfo => fileInfo.LastWriteTimeUtc).First())
            .OrderBy(fileInfo => fileInfo.Name, StringComparer.Ordinal)
            .ToArray();

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

        var settings = new VerifySettings();
        settings.UseTextForParameters(discriminator);

        return VerifyFile(package, settings)
            .ScrubNuspec();
    }

    private static string GetPackageDiscriminator(string packageName)
    {
        // For all package types, extract just the package name without version or extension
        string baseName = packageName;

        // Remove .symbols.nupkg extension (legacy)
        if (baseName.Contains(".symbols.nupkg", StringComparison.Ordinal))
        {
            baseName = baseName.Replace(".symbols.nupkg", string.Empty, StringComparison.Ordinal);
        }
        // Remove .nupkg extension
        else if (baseName.EndsWith(".nupkg", StringComparison.Ordinal))
        {
            baseName = baseName.Replace(".nupkg", string.Empty, StringComparison.Ordinal);
        }

        return ExtractPackageName(baseName);
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
