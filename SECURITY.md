# Security Policy

## Supported Versions

We release patches for security vulnerabilities in the following versions:

| Version | Supported          |
| ------- | ------------------ |
| 10.x    | :white_check_mark: |
| < 10.0  | :x:                |

## Reporting a Vulnerability

**Please do not report security vulnerabilities through public GitHub issues.**

Instead, please report them through GitHub Security Advisories:

### GitHub Security Advisories

Report a vulnerability privately through GitHub:

1. Go to the [Security tab](https://github.com/rjmurillo/Qwiq/security)
2. Click "Report a vulnerability"
3. Fill out the form with details about the vulnerability

### What to Expect

- **Acknowledgment**: You will receive a response within 48 hours
- **Updates**: We will keep you informed about the progress
- **Credit**: Security researchers will be credited (unless they prefer to remain anonymous)
- **Timeline**: We aim to release a fix within 90 days for critical vulnerabilities

## Security Best Practices

When using Qwiq in your applications:

### Credential Management

- **Never** hardcode credentials in source code
- Use Azure Key Vault, environment variables, or secure credential managers
- Rotate Personal Access Tokens (PATs) regularly
- Use PATs with minimum required scopes

### Authentication

- Prefer OAuth over basic authentication when possible
- Use Windows authentication for on-premises TFS when available
- Set appropriate token expiration periods

### Network Security

- Use HTTPS connections to Azure DevOps/TFS
- Validate SSL certificates in production environments
- Consider using private endpoints for Azure DevOps

### Access Control

- Follow the principle of least privilege
- Review and audit PAT permissions regularly
- Use service accounts with limited permissions for automated systems

## Known Security Considerations

### Personal Access Tokens (PATs)

- PATs grant access to your Azure DevOps organization
- Treat PATs like passwords - never commit them to repositories
- Use short-lived tokens when possible
- Store tokens securely using credential managers

### TFS/Azure DevOps API

- This library connects to Azure DevOps/TFS APIs
- Ensure your Azure DevOps organization has appropriate security policies
- Review Azure DevOps audit logs regularly

## Security Updates

Security updates will be:

- Released as patch versions (e.g., 10.0.x)
- Announced in release notes
- Tagged with `security` label in GitHub releases
- Documented in CHANGELOG.md

## Additional Resources

- [Azure DevOps Security Best Practices](https://docs.microsoft.com/en-us/azure/devops/organizations/security/security-best-practices)
- [GitHub Security Advisories](https://github.com/rjmurillo/Qwiq/security/advisories)
- [Secure Coding Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/security/secure-coding-guidelines)
