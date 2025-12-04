---
applyTo: "**"
---

# Instruction File Index

This table maps file patterns in the repository to their corresponding instruction files.
Use this as a quick reference to ensure you are following the correct guidance for each file type.

| Pattern                | Instruction File                                             | Description                     |
| ---------------------- | ------------------------------------------------------------ | ------------------------------- |
| `*.cs`                 | [csharp.instructions.md](csharp.instructions.md)             | C# source files                 |
| `*.csproj`, `*.sln`    | [project.instructions.md](project.instructions.md)           | Project/solution files          |
| `*.props`, `*.targets` | [msbuild.instructions.md](msbuild.instructions.md)           | MSBuild property/target files   |
| `.editorconfig`        | [editorconfig.instructions.md](editorconfig.instructions.md) | EditorConfig rules              |
| `*.md`                 | [markdown.instructions.md](markdown.instructions.md)         | Markdown documentation          |
| `*.yml`, `*.yaml`      | [yaml.instructions.md](yaml.instructions.md)                 | CI/CD workflows                 |
| `*.ps1`                | [shell.instructions.md](shell.instructions.md)               | PowerShell scripts              |
| `*.json`               | [json.instructions.md](json.instructions.md)                 | JSON configuration              |
| Other                  | [generic.instructions.md](generic.instructions.md)           | Fallback for unknown file types |

## How to Use

1. Before editing any file, find the matching instruction file from this table
2. Read the instruction file completely before making changes
3. Follow the validation checklist in each instruction file
4. If uncertain, stop and request guidance

## Related Files

- [copilot-instructions.md](../copilot-instructions.md) - Main repository instructions
- [codacy.instructions.md](codacy.instructions.md) - Codacy MCP integration rules
