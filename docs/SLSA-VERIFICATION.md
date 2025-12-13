# SLSA Provenance Verification

This document explains how to verify the SLSA provenance for Qwiq NuGet packages.

## What is SLSA?

SLSA (Supply-chain Levels for Software Artifacts) is a security framework that provides end-to-end supply chain integrity. Qwiq generates SLSA Level 3 provenance for all releases, providing cryptographic proof of:

- **Build integrity**: The packages were built from the claimed source code
- **Build environment**: The build ran in a trusted GitHub Actions environment
- **Non-repudiation**: The provenance cannot be forged

## Obtaining Provenance Files

SLSA provenance files are attached to each GitHub Release:

1. Navigate to the [Releases page](https://github.com/rjmurillo/Qwiq/releases)
2. Select the release version you want to verify
3. Download the provenance file: `qwiq-provenance.intoto.jsonl`

## Verifying Provenance

### Prerequisites

Install the SLSA verifier tool:

```bash
# Using Go
go install github.com/slsa-framework/slsa-verifier/v2/cli/slsa-verifier@latest

# Or download pre-built binary from:
# https://github.com/slsa-framework/slsa-verifier/releases
```

### Verification Steps

#### Step 1: Download the package and provenance

```bash
# Download the package you want to verify
wget https://www.nuget.org/api/v2/package/Qwiq.Core/10.0.0

# Download the provenance from GitHub Release
wget https://github.com/rjmurillo/Qwiq/releases/download/v10.0.0/qwiq-provenance.intoto.jsonl
```

#### Step 2: Verify the provenance

```bash
slsa-verifier verify-artifact \
  --provenance-path qwiq-provenance.intoto.jsonl \
  --source-uri github.com/rjmurillo/Qwiq \
  Qwiq.Core.10.0.0.nupkg
```

**Expected output:**

```txt
Verified signature against tlog entry index 123456789 at URL: https://rekor.sigstore.dev/api/v1/log/entries/...
Verified build using builder "https://github.com/slsa-framework/slsa-github-generator/.github/workflows/generator_generic_slsa3.yml@refs/tags/v2.0.0" at commit <commit-sha>
Verifying artifact Qwiq.Core.10.0.0.nupkg: PASSED

PASSED: Verified SLSA provenance
```

### What Does Verification Prove?

A successful verification proves:

1. **Authenticity**: The package was built by the official Qwiq repository
2. **Integrity**: The package hasn't been tampered with since it was built
3. **Traceability**: The exact source commit used to build the package
4. **Build environment**: The package was built in GitHub Actions, not on a developer's machine

## Verification Failures

If verification fails, you will see an error message. Common reasons:

- **Hash mismatch**: The package file has been modified
- **Source mismatch**: The package wasn't built from the claimed repository
- **Invalid signature**: The provenance file has been tampered with
- **Expired certificate**: The signing certificate has expired (contact maintainers)

**⚠️ DO NOT USE packages that fail verification.**

## Provenance File Format

The provenance file is in [in-toto](https://in-toto.io/) format with SLSA v1.0 predicate. It contains:

- **Subject**: SHA256 hashes of all packages
- **Predicate**: Build metadata including:
  - Builder: `slsa-framework/slsa-github-generator`
  - Source repository: `github.com/rjmurillo/Qwiq`
  - Commit SHA used for the build
  - Build parameters and environment
- **Signature**: Cryptographic signature using Sigstore

## Automated Verification

To automate verification in your CI/CD pipeline:

```yaml
# Example GitHub Actions workflow
- name: Verify Qwiq package
  run: |
    slsa-verifier verify-artifact \
      --provenance-path qwiq-provenance.intoto.jsonl \
      --source-uri github.com/rjmurillo/Qwiq \
      Qwiq.Core.10.0.0.nupkg
```

## Resources

- [SLSA Framework](https://slsa.dev/)
- [SLSA Verifier Tool](https://github.com/slsa-framework/slsa-verifier)
- [in-toto Provenance Format](https://in-toto.io/)
- [Sigstore](https://www.sigstore.dev/)

## Questions?

If you have questions about SLSA provenance or encounter verification issues, please [open an issue](https://github.com/rjmurillo/Qwiq/issues/new).
