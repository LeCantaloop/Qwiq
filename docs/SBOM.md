# Software Bill of Materials (SBOM)

This document explains the Software Bill of Materials (SBOM) provided with Qwiq releases.

## What is an SBOM?

A Software Bill of Materials (SBOM) is a comprehensive inventory of all components, libraries, and dependencies included in Qwiq packages. It provides:

- **Transparency**: Complete visibility into what's inside each package
- **Compliance**: Meets Executive Order 14028 requirements
- **Security**: Enables rapid vulnerability assessment
- **License tracking**: Documents all dependency licenses

## Obtaining SBOM Files

SBOM files are provided in two ways:

### 1. Build Pipeline (Validation)

Every build generates a validation SBOM as an artifact. This helps catch dependency issues early.

**Access validation SBOMs**:

- Navigate to the [Actions tab](https://github.com/rjmurillo/Qwiq/actions)
- Select a workflow run
- Download the `sbom-validation` artifact

### 2. Release Pipeline (Authoritative)

Each GitHub Release includes an authoritative SBOM in SPDX format:

1. Navigate to the [Releases page](https://github.com/rjmurillo/Qwiq/releases)
2. Select the release version
3. Download the SBOM file: `_manifest.spdx.json`

## SBOM Format

Qwiq uses the **SPDX 2.3** format, which is:

- Industry-standard format maintained by the Linux Foundation
- Compatible with NTIA Minimum Elements requirements
- Machine-readable JSON format
- Widely supported by security scanning tools

## SBOM Contents

Each SBOM includes:

### Package Information

- **Name**: Package identifier (e.g., Qwiq.Core)
- **Version**: Semantic version number
- **Supplier**: Package maintainer information
- **License**: SPDX license identifier (MIT)

### Dependencies

- **Direct dependencies**: Packages explicitly referenced
- **Transitive dependencies**: Dependencies of dependencies
- **Dependency relationships**: Complete dependency graph

### Component Details

- **Package URLs (PURL)**: Standard identifiers for each component
- **File hashes**: SHA256/SHA1 checksums
- **License information**: SPDX license identifiers

## Using SBOM Files

### Security Scanning

Use the SBOM to quickly identify vulnerable dependencies:

```bash
# Example: Scan SBOM with Grype (Anchore)
grype sbom:./qwiq-sbom.spdx.json

# Example: Scan SBOM with Trivy (Aqua Security)
trivy sbom ./qwiq-sbom.spdx.json
```

### License Compliance

Analyze dependency licenses:

```bash
# Example: Extract license information with SBOM tool
# This requires the SPDX tools from https://github.com/spdx/tools

# View all licenses
jq '.packages[].licenseDeclared' qwiq-sbom.spdx.json | sort -u

# Find packages with specific license
jq '.packages[] | select(.licenseDeclared == "Apache-2.0") | .name' qwiq-sbom.spdx.json
```

### Dependency Analysis

Examine the dependency tree:

```bash
# List all direct dependencies
jq '.packages[] | select(.name == "Qwiq.Core") | .relationships[] | select(.relationshipType == "DEPENDS_ON") | .relatedSpdxElement' qwiq-sbom.spdx.json

# Count total dependencies
jq '.packages | length' qwiq-sbom.spdx.json
```

## Automated Tooling

### CI/CD Integration

Integrate SBOM verification into your pipeline:

```yaml
# Example GitHub Actions workflow
- name: Download Qwiq SBOM
  run: curl -LO https://github.com/rjmurillo/Qwiq/releases/download/v10.0.0/_manifest.spdx.json

- name: Scan for vulnerabilities
  run: grype sbom:./_manifest.spdx.json
```

### Dependency Tracking

Use SBOM for ongoing dependency monitoring:

```bash
# Track dependency changes between versions
diff v9.0.0-sbom.spdx.json v10.0.0-sbom.spdx.json
```

## Compliance

Qwiq SBOMs meet or exceed:

- **Executive Order 14028**: SBOM minimum elements
- **NTIA SBOM Guidelines**: Includes all required fields
- **SPDX 2.3**: ISO/IEC 5962:2021 standard

### Minimum Elements Coverage

| Element           | Included | Location in SBOM          |
| ----------------- | -------- | ------------------------- |
| Supplier Name     | ✅       | `.packages[].supplier`    |
| Component Name    | ✅       | `.packages[].name`        |
| Version           | ✅       | `.packages[].versionInfo` |
| Dependencies      | ✅       | `.relationships[]`        |
| Author            | ✅       | `.creationInfo.creators`  |
| Timestamp         | ✅       | `.creationInfo.created`   |
| Unique Identifier | ✅       | `.packages[].SPDXID`      |

## SBOM Generation Process

### Build Pipeline (Validation)

1. Build completes and creates packages
2. Microsoft SBOM Tool scans build output
3. Generates SBOM with all dependencies
4. Uploads as workflow artifact for inspection

### Release Pipeline (Authoritative)

1. Release is triggered (tag or GitHub Release)
2. Packages are downloaded from build artifacts
3. SBOM Tool generates authoritative SBOM with release version
4. SBOM is attached to GitHub Release alongside packages

## Verifying SBOM Integrity

SBOM files themselves are not signed, but they are:

1. **Generated deterministically**: Same build = same SBOM
2. **Uploaded via GitHub Actions**: Audit trail in workflow logs
3. **Attached to releases**: Immutable once published
4. **Covered by SLSA provenance**: Build environment is verified

To verify the SBOM matches the packages:

```bash
# 1. Verify SLSA provenance (proves build integrity)
slsa-verifier verify-artifact --provenance-path qwiq-provenance.intoto.jsonl Qwiq.Core.10.0.0.nupkg

# 2. Check SBOM package hashes match actual package
# Extract hash from SBOM
jq -r '.files[] | select(.fileName | contains("Qwiq.Core")) | .checksums[] | select(.algorithm == "SHA256") | .checksumValue' qwiq-sbom.spdx.json

# Compare with actual package hash
sha256sum Qwiq.Core.10.0.0.nupkg
```

## SBOM Updates

SBOMs are generated:

- **Every build**: Validation SBOM in CI pipeline
- **Every release**: Authoritative SBOM attached to release
- **Automatically**: No manual intervention required

When dependencies change, the SBOM reflects the changes in the next build.

## Questions?

For questions about SBOMs or to report issues, please [open an issue](https://github.com/rjmurillo/Qwiq/issues/new).

## Resources

- [SPDX Specification](https://spdx.github.io/spdx-spec/)
- [Microsoft SBOM Tool](https://github.com/microsoft/sbom-tool)
- [NTIA SBOM Guidelines](https://www.ntia.gov/report/2021/minimum-elements-software-bill-materials-sbom)
- [Executive Order 14028](https://www.whitehouse.gov/briefing-room/presidential-actions/2021/05/12/executive-order-on-improving-the-nations-cybersecurity/)
