from pathlib import Path
import re
import os
import glob
import codecs
from collections import defaultdict

# Path to localization files
LOCALE_DIR = Path(__file__).resolve().parents[1]
EN_FILE = LOCALE_DIR / 'en-US.hjson'
LANG_FILES = [f for f in glob.glob(os.path.join(LOCALE_DIR, "*.hjson")) if not f.endswith("en-US.hjson")]

# Regex patterns for various types of entries
LABEL_REGEX = re.compile(r'^\s*Label:\s*(.*)$', re.MULTILINE)
TOOLTIP_REGEX = re.compile(r'^\s*Tooltip:\s*(.*)$', re.MULTILINE)
SIMPLE_KEY_VALUE_REGEX = re.compile(r'^\s*([A-Za-z0-9_]+):\s*([^{].*)$', re.MULTILINE)

def extract_labels(hjson_text):
    """Extract all key-value pairs including Labels and Headers from the given hjson text."""
    translation_entries = {}
    stack = []
    path = []
    in_headers = False
    lines = hjson_text.splitlines()
    
    for i, line in enumerate(lines):
        # Skip comment lines
        if line.strip().startswith('//'):
            continue
            
        # Track object openings - adds to the path stack
        m = re.match(r'^\s*([A-Za-z0-9_]+)\s*:\s*{', line)
        if m:
            key = m.group(1)
            stack.append(key)
            path = stack.copy()
            if key == "Headers":
                in_headers = True
            continue
            
        # Track object closings - pops from the path stack
        if "}" in line:
            if stack:
                if stack[-1] == "Headers":
                    in_headers = False
                stack.pop()
            path = stack.copy()
            continue
        
        # Case 1: Extract Label entries
        m = LABEL_REGEX.match(line)
        if m:
            label_value = m.group(1).strip().strip('"')
            label_path = ".".join(path + ["Label"])
            translation_entries[label_path] = label_value
            continue
            
        # Case 2: Extract Tooltip entries
        m = TOOLTIP_REGEX.match(line)
        if m:
            tooltip_value = m.group(1).strip().strip('"')
            tooltip_path = ".".join(path + ["Tooltip"])
            translation_entries[tooltip_path] = tooltip_value
            continue
            
        # Case 3: Extract simple key-value pairs (especially for Headers)
        m = SIMPLE_KEY_VALUE_REGEX.match(line)
        if m and (in_headers or len(path) == 0):  # Either in Headers section or root level
            key = m.group(1).strip()
            value = m.group(2).strip().strip('"')
            if in_headers:
                entry_path = ".".join(path + [key])
                translation_entries[entry_path] = value
            else:
                # For root level or other simple key-value pairs
                if path:
                    entry_path = ".".join(path + [key])
                    translation_entries[entry_path] = value
    
    print(f"Extracted {len(translation_entries)} translation entries")
    return translation_entries

def compare_labels(en_labels, lang_labels):
    """Compare EN labels to another language's labels."""
    translated = {}
    untranslated = {}
    for key, en_val in en_labels.items():
        lang_val = lang_labels.get(key, None)
        if lang_val is None or lang_val == en_val:
            untranslated[key] = en_val
        else:
            translated[key] = lang_val
    return translated, untranslated

def main():
    # Read EN labels
    with codecs.open(EN_FILE, "r", "utf-8") as f:
        en_text = f.read()
    en_labels = extract_labels(en_text)
    total_labels = len(en_labels)
    print("Total entries:", total_labels)

    report_lines = []
    stats_lines = []

    # For each language file
    for lang_file in [EN_FILE] + LANG_FILES:
        lang_code = os.path.splitext(os.path.basename(lang_file))[0]
        with codecs.open(lang_file, "r", "utf-8") as f:
            lang_text = f.read()
        lang_labels = extract_labels(lang_text)

        if lang_code == "en-US":
            # Hardcode English to have full translation
            num_translated = total_labels
            untranslated = {}
        else:
            translated, untranslated = compare_labels(en_labels, lang_labels)
            num_translated = total_labels - len(untranslated)

        percent = int(num_translated / total_labels * 100)
        stats_lines.append(f"{lang_code}, {num_translated}/{total_labels}, {percent}%, missing {len(untranslated)}")

        if lang_code != "en-US":
            report_lines.append(f"\n=== {lang_code} ===")
            for key in sorted(untranslated):
                report_lines.append(f"{key}: {en_labels[key]}")

    # Clear the report file before writing
    report_file_path = os.path.join(LOCALE_DIR, "TranslationsNeeded.txt")
    with codecs.open(report_file_path, "w", "utf-8") as f:
        f.write("")  # Clear the file by writing an empty string

    # Write report
    with codecs.open(report_file_path, "w", "utf-8") as f:
        f.write("\n" + "\n".join(stats_lines) + "\n")
        print("\n" + "\n".join(stats_lines) + "\n")
        
    print(f"Report written to {report_file_path}")

if __name__ == "__main__":
    main()