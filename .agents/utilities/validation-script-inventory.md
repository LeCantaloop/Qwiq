# Validation Script Inventory

**Purpose:** Map validation scripts to their MSBuild/configuration dependencies to prevent misses during build system changes.

**Update Trigger:** Any change to `Directory.Build.props`, `Directory.Packages.props`, or build scripts.

---

## Inventory

| Script                         | Purpose                        | Config Dependencies                                                     | Updated By ADR | Last Review |
| ------------------------------ | ------------------------------ | ----------------------------------------------------------------------- | -------------- | ----------- |
| Validate-PackageOutput.ps1     | Verifies package files exist   | `DebugType`, `IncludeSymbols`, `SymbolPackageFormat`, `GeneratePackage` | ADR-012        | 2025-12-15  |
| Verify-SourceLink.ps1          | Tests PDB SourceLink metadata  | `DebugType`, `PublishRepositoryUrl`, `EmbedUntrackedSources`            | ADR-011        | 2025-12-13  |
| Count-NullableWarnings.ps1     | Counts CS86xx warnings         | `Nullable`, `TreatWarningsAsErrors`                                     | -              | 2025-11-01  |
| Migrate-PublicApiToShipped.ps1 | Moves unshipped API to shipped | (None - process script)                                                 | -              | 2025-11-01  |

---

## Update Protocol

When changing MSBuild properties in `Directory.Build.props`:

1. **Consult this inventory** for affected scripts
2. **Test each affected script** after build:

   ```powershell
   dotnet build Qwiq.sln -c Release
   dotnet pack Qwiq.sln -c Release --no-build
   ./build/scripts/Validate-PackageOutput.ps1
   ./build/scripts/Verify-SourceLink.ps1
   # etc.
   ```

3. **Update scripts** if needed to reflect new config
4. **Update this inventory** with "Updated By ADR" and "Last Review" date
5. **Commit together** with the config change

---

## Script Details

### Validate-PackageOutput.ps1

**Configuration Dependencies:**

- `DebugType` - Affects symbol package generation
  - `embedded` → No `.snupkg` files (symbols in `.dll`)
  - `portable` → Generates `.snupkg` files (with `IncludeSymbols=true`)
- `IncludeSymbols` - Controls `.snupkg` generation
  - `true` → Expects `.snupkg` files
  - `false` → No `.snupkg` expected
- `SymbolPackageFormat` - Format of symbol packages (if generated)
- `GeneratePackageOnBuild` - Whether projects pack on build

**Script Parameters:**

- `-ExpectSymbolPackages $false` (default) - Embedded symbols mode
- `-ExpectSymbolPackages $true` - Portable symbols mode

**Current Qwiq Configuration:**

- `DebugType=embedded` (Release)
- `IncludeSymbols=false`
- Result: **No `.snupkg` files generated**

### Verify-SourceLink.ps1

**Configuration Dependencies:**

- `DebugType` - Affects PDB location
  - `embedded` → PDBs inside `.dll` files
  - `portable` → PDBs in separate `.pdb` files
- `PublishRepositoryUrl` - Adds repository URL to packages
- `EmbedUntrackedSources` - Embeds source files in PDBs

**Notes:**

- Comment in script: "We test PDBs directly because with snupkg format, PDBs are not embedded in nupkg files"
- This comment is now **outdated** (we use embedded symbols, not snupkg)
- Script still works correctly (tests PDBs regardless of format)

### Count-NullableWarnings.ps1

**Configuration Dependencies:**

- `Nullable` - Enables nullable reference types
- `TreatWarningsAsErrors` - Whether CS86xx warnings block build

**Notes:**

- Script is informational (tracks migration progress)
- Not affected by build system changes

---

## Impact Analysis Template

When making ADRs that change build configuration, include this section:

```markdown
## Impact Analysis - Affected Files

### Validation Scripts

- [ ] `Validate-PackageOutput.ps1` - Needs update (DebugType change)
- [ ] `Verify-SourceLink.ps1` - No change (informational comment only)
- [ ] `Count-NullableWarnings.ps1` - No impact

### CI/CD Workflows

- [ ] `.github/workflows/main.yml` - Update artifact paths
- [ ] `.github/workflows/release.yml` - Update hash computation comment

### Documentation

- [ ] `CLAUDE.md` - Add build configuration section
- [ ] ADR documentation - Update rationale
```

---

## Related Documents

- [AGENT-INSTRUCTIONS.md](../AGENT-INSTRUCTIONS.md) - Agent execution guidelines
- [Session 41 Retrospective](../retrospective/2025-12-15-session-41-validation-script-miss.md) - Root cause of this inventory
- [ADR-012](../../docs/architecture/decisions/ADR-012-embedded-debug-symbols.md) - Embedded symbols decision

---

## Maintenance

**Review Frequency:** After every ADR that changes MSBuild properties

**Owner:** QA Agent (primary), DevOps Agent (secondary)

**Last Updated:** 2025-12-15 (Session 41)
