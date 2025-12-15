# Pattern: Package Metadata Changes

**Pattern Type**: Validation  
**Applies To**: Build configuration changes  
**Priority**: High (affects package publishing)

---

## When to Use This Pattern

Apply this pattern any time you modify:

- `Directory.Build.props`
- `Directory.Build.targets`
- `Directory.Packages.props`
- Build-affecting packages:
  - DotNet.ReproducibleBuilds
  - Microsoft.SourceLink.\*
  - Nerdbank.GitVersioning
- Project-level package metadata properties:
  - `<DebugType>`
  - `<IncludeSymbols>`
  - `<SymbolPackageFormat>`
  - `<PackageReadmeFile>`
  - `<PackageIcon>`
  - etc.

---

## Why This Matters

Changes to build configuration can affect:

1. **Package manifests** (.nuspec metadata)
2. **Package contents** (files included in .nupkg)
3. **Dependency declarations** (runtime dependencies)

QWIQ uses snapshot testing (via Verify) to ensure package outputs remain consistent. When configuration changes, baselines must be reviewed and updated.

---

## Validation Steps

### 1. Build in Release Mode

```bash
dotnet build Qwiq.sln /m:1 /nodeReuse:false -c Release
```

This creates packages in `artifacts/package/release/`.

### 2. Run Package Tests

```bash
dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj --no-build -c Release
```

Package tests compare:

- Package manifest (.nuspec) against `*.verified.nuspec` files
- Package contents (file list) against `*.verified.txt` files

### 3. Review Differences

If tests fail, check what changed:

```bash
# View received files (actual output)
ls test/Qwiq.Package.Tests/*.received.*

# Compare with verified baselines
diff test/Qwiq.Package.Tests/PackageTests.Baseline_Qwiq.Core#manifest.{received,verified}.nuspec
```

**Ask yourself**:

- Are these changes expected given my configuration change?
- Do the changes make sense (e.g., removing snupkg → no `<SymbolPackageFormat>` in manifest)?
- Are there any unexpected side effects?

### 4. Rebaseline if Changes Are Expected

**Interactive (requires TTY)**:

```bash
dotnet verify accept -w test/Qwiq.Package.Tests
```

**Non-interactive (CI/automation)**:

```bash
cd test/Qwiq.Package.Tests
for file in *.received.*; do
    verified="${file/received/verified}"
    cp "$file" "$verified"
    echo "Accepted: $verified"
done
```

### 5. Commit Baseline Updates with Configuration Changes

```bash
git add test/Qwiq.Package.Tests/*.verified.*
git commit -m "test: update package baselines after [configuration change]"
```

**Important**: Baseline updates should be committed **together with** the configuration changes that caused them.

---

## Common Scenarios

### Scenario 1: Changing DebugType

**Change**: `DebugType=portable` → `DebugType=embedded`

**Expected baseline updates**:

- Manifest: No `<SymbolPackageFormat>snupkg</SymbolPackageFormat>`
- Manifest: No separate symbol package reference
- Contents: PDB files may be embedded in DLLs (size increase)

### Scenario 2: Adding Package Readme

**Change**: Added `<PackageReadmeFile>README.md</PackageReadmeFile>`

**Expected baseline updates**:

- Manifest: `<readme>README.md</readme>` element appears
- Contents: `README.md` file in package root

### Scenario 3: Building from Feature Branch

**Change**: Working on `feature/my-branch` instead of `master`

**Expected baseline updates**:

- Manifest: `<repository ... branch="refs/heads/feature/my-branch" .../>`

**Note**: This is **expected behavior** from DotNet.ReproducibleBuilds and improves traceability.

### Scenario 4: Updating Package Dependency

**Change**: Updated `Microsoft.VisualStudio.Services.Client` version in Directory.Packages.props

**Expected baseline updates**:

- Manifest: `<dependency id="Microsoft.VisualStudio.Services.Client" version="16.170.0" .../>` (new version)
- Manifest: Transitive dependency versions may change

---

## Checklist

Use this checklist for build configuration changes:

- [ ] Configuration change made (Directory.Build.props, etc.)
- [ ] Build in Release mode
- [ ] Run package tests
- [ ] Review .received vs .verified diffs
- [ ] Verify changes are expected
- [ ] Rebaseline using `dotnet verify accept` or manual copy
- [ ] Commit baseline updates with configuration changes
- [ ] Document reason for baseline update in commit message

---

## Related Documentation

- [msbuild.instructions.md](/.github/instructions/msbuild.instructions.md) - MSBuild file editing guidelines
- [project.instructions.md](/.github/instructions/project.instructions.md) - Project file validation
- [copilot-instructions.md](/copilot-instructions.md) - Package Tests section (lines 641-680)

---

## Example Commit Messages

```text
test: update package baselines after switching to embedded symbols

- Manifest no longer includes SymbolPackageFormat (no separate snupkg)
- Repository metadata includes branch name (DotNet.ReproducibleBuilds behavior)
```

```text
test: rebaseline package tests after adding package readme

- Manifest now includes <readme>README.md</readme> element
- Contents include README.md in package root
```

---

**Last Updated**: 2025-12-14  
**Trigger for Update**: Retrospective analysis of missed baseline update
