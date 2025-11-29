using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Qwiq.Package.Tests;

public static class ModuleInitializer
{
    // Regex to match Qwiq.* dependency version attributes
    // Matches: <dependency id="Qwiq.Core" version="10.0.31" />
    // Replaces version with "*" to make tests stable across GitVersion bumps
    private static readonly Regex QwiqDependencyVersionRegex = new(
        @"(<dependency\s+id=""Qwiq\.[^""]+""[^>]*\s+version="")[^""]+("")",
        RegexOptions.Compiled);

    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyNupkg.Initialize();

        // Add a global scrubber to normalize internal Qwiq dependency versions
        // This ensures tests don't fail when GitVersion bumps the version number
        // External dependency versions (e.g., Newtonsoft.Json, Castle.Core) are preserved
        VerifierSettings.AddScrubber(
            (builder, _) =>
            {
                string content = builder.ToString();
                if (content.Contains("<dependency id=\"Qwiq."))
                {
                    string scrubbed = QwiqDependencyVersionRegex.Replace(content, "$1*$2");
                    builder.Clear();
                    builder.Append(scrubbed);
                }
            });
    }
}
