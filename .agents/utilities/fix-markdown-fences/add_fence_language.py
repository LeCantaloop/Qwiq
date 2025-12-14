#!/usr/bin/env python3
"""Add default language specifier to code fences without one.

Problem: Opening fences without language identifiers (``` instead of ```text)
Solution: Scan markdown files and add 'text' as default language
"""

import re
import sys
from pathlib import Path


def add_fence_language(content: str, default_lang: str = "text") -> str:
    """Add language specifier to code fences that don't have one.

    Args:
        content: Raw markdown string
        default_lang: Language to add to bare fences (default: text)

    Returns:
        Fixed markdown string
    """
    lines = content.splitlines()
    result: list[str] = []
    in_code_block = False

    # Pattern for opening fence with language
    opening_with_lang = re.compile(r'^(\s*)```(\w+)')
    # Pattern for bare opening fence (no language)
    bare_opening = re.compile(r'^(\s*)```\s*$')
    # Pattern for closing fence
    closing = re.compile(r'^(\s*)```\s*$')

    for line in lines:
        if not in_code_block:
            # Check if this is an opening fence
            if opening_with_lang.match(line):
                # Already has language
                result.append(line)
                in_code_block = True
            elif bare_opening.match(line):
                # Bare fence - add language
                indent = bare_opening.match(line).group(1)
                result.append(f"{indent}```{default_lang}")
                in_code_block = True
            else:
                result.append(line)
        else:
            # Inside code block - look for closing fence
            if closing.match(line):
                result.append(line)
                in_code_block = False
            else:
                result.append(line)

    return '\n'.join(result)


def fix_markdown_files(
    directory: Path,
    pattern: str = "**/*.md",
    default_lang: str = "text",
    dry_run: bool = False
) -> list[str]:
    """Fix all markdown files in directory.

    Args:
        directory: Root directory to scan
        pattern: Glob pattern for markdown files
        default_lang: Language to add to bare fences
        dry_run: If True, report changes without writing

    Returns:
        List of fixed file paths
    """
    fixed: list[str] = []

    for file_path in directory.glob(pattern):
        content = file_path.read_text(encoding='utf-8')
        fixed_content = add_fence_language(content, default_lang)

        if content != fixed_content:
            if not dry_run:
                file_path.write_text(fixed_content, encoding='utf-8')
            fixed.append(str(file_path))

    return fixed


def main() -> int:
    """CLI entry point."""
    import argparse

    parser = argparse.ArgumentParser(
        description='Add language specifier to bare code fences'
    )
    parser.add_argument(
        'directories',
        nargs='+',
        help='Directories to scan for markdown files'
    )
    parser.add_argument(
        '--pattern',
        default='**/*.md',
        help='Glob pattern for markdown files (default: **/*.md)'
    )
    parser.add_argument(
        '--language',
        default='text',
        help='Language to add to bare fences (default: text)'
    )
    parser.add_argument(
        '--dry-run',
        action='store_true',
        help='Report changes without writing files'
    )

    args = parser.parse_args()

    total_fixed = 0
    for dir_path in args.directories:
        directory = Path(dir_path)
        if not directory.exists():
            print(f"Warning: {dir_path} does not exist", file=sys.stderr)
            continue

        fixed = fix_markdown_files(
            directory, args.pattern, args.language, args.dry_run
        )
        for f in fixed:
            prefix = "[dry-run] " if args.dry_run else ""
            print(f"{prefix}Fixed: {f}")
        total_fixed += len(fixed)

    if total_fixed == 0:
        print("No files needed fixing")
    else:
        action = "would fix" if args.dry_run else "fixed"
        print(f"\nTotal: {action} {total_fixed} file(s)")

    return 0


if __name__ == '__main__':
    sys.exit(main())
