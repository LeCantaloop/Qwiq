using System.Threading.Tasks;
using VerifyTests;
using Xunit;

namespace Qwiq.Package.Tests;

/// <summary>
/// Verify framework checks. These tests require packages to be built,
/// so they only run on Windows where net472 can be built.
/// </summary>
[Trait("TestCategory", "Package")]
public partial class VerifyChecksTests
{
    [Fact]
    public Task Run() =>
        VerifyChecks.Run();
}
