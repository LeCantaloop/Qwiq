using System.Reflection;

namespace Qwiq.Package.Tests;

public class PackageTests
{
    public static TheoryData<string> GetPackages()
    {
        DirectoryInfo directory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!;
        FileInfo[] packages = directory.GetFiles("Qwiq*.nupkg", SearchOption.AllDirectories)
            .OrderBy(fileInfo => fileInfo.Name, StringComparer.Ordinal)
            .ToArray();

        if (packages.Length == 0)
        {
            throw new InvalidOperationException(
                "No Qwiq*.nupkg files were found. Ensure the pack step runs before executing this test. " +
                $"Searched in: {directory.FullName}");
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

    private static string ExtractPackageName(string fullName)
    {
        int lastDotIndex = fullName.LastIndexOf('.');
        while (lastDotIndex > 0 && char.IsDigit(fullName[lastDotIndex - 1]))
        {
            lastDotIndex = fullName.LastIndexOf('.', lastDotIndex - 1);
        }

        return lastDotIndex > 0 ? fullName[..lastDotIndex] : fullName;
    }
}
