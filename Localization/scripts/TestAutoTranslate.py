from pathlib import Path
import re
import os
import glob
import codecs
import time
from google.cloud import translate_v2 as translate
from collections import defaultdict

# Path to localization files
LOCALE_DIR = Path(__file__).resolve().parents[1]
EN_FILE = LOCALE_DIR / 'en-US.hjson'
LANG_FILES = [f for f in glob.glob(os.path.join(LOCALE_DIR, "*.hjson")) if not f.endswith("en-US.hjson")]

# Language code mapping for Google Translate
LANG_CODES = {
    "de-DE": "de",
    "es-ES": "es",
    "fr-FR": "fr",
    "it-IT": "it",
    "pl-PL": "pl",
    "pt-BR": "pt",
    "ru-RU": "ru",
    "uk-UA": "uk",
    "zh-Hans": "zh-CN"
}

# Regex patterns for extraction
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
        if m and (in_headers or len(path) == 0):
            key = m.group(1).strip()
            value = m.group(2).strip().strip('"')
            if in_headers:
                entry_path = ".".join(path + [key])
                translation_entries[entry_path] = value
            elif path:
                entry_path = ".".join(path + [key])
                translation_entries[entry_path] = value
    
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

def update_hjson_with_translations(file_path, keys_to_update, translations):
    """Update a HJSON file with new translations."""
    # Read the entire file
    with codecs.open(file_path, 'r', 'utf-8') as f:
        content = f.read()
        
    lines = content.splitlines()
    stack = []
    path = []
    updated_lines = []
    
    for line in lines:
        # Keep track of object nesting
        if re.match(r'^\s*([A-Za-z0-9_]+)\s*:\s*{', line):
            key = re.match(r'^\s*([A-Za-z0-9_]+)\s*:\s*{', line).group(1)
            stack.append(key)
            path = stack.copy()
            updated_lines.append(line)
            continue
        
        if "}" in line:
            if stack:
                stack.pop()
            path = stack.copy()
            updated_lines.append(line)
            continue
        
        # Check if this line is one we need to update
        for pattern, key_suffix in [
            (LABEL_REGEX, "Label"), 
            (TOOLTIP_REGEX, "Tooltip"),
            (SIMPLE_KEY_VALUE_REGEX, None)
        ]:
            m = pattern.match(line)
            if m:
                if key_suffix:
                    cur_key = ".".join(path + [key_suffix])
                else:
                    # For simple key-value, extract key
                    simple_key = m.group(1).strip()
                    cur_key = ".".join(path + [simple_key])
                
                if cur_key in keys_to_update:
                    # Replace this line with the translated version
                    indent = re.match(r'(\s*)', line).group(1)
                    if key_suffix:
                        new_line = f"{indent}{key_suffix}: \"{translations[cur_key]}\""
                    else:
                        new_line = f"{indent}{simple_key}: \"{translations[cur_key]}\""
                    updated_lines.append(new_line)
                    break
            else:
                # No match, keep the line as is
                updated_lines.append(line)
                break
        else:
            # No matches in any pattern, keep the line
            updated_lines.append(line)
    
    # Write the updated content back
    with codecs.open(file_path, 'w', 'utf-8') as f:
        f.write("\n".join(updated_lines))

def translate_text(texts, target_language):
    """Translate a list of texts to the target language using Google Cloud Translate."""
    client = translate.Client()
    
    # Batch translate (more efficient)
    results = client.translate(texts, target_language=target_language)
    
    # Extract translated text
    translations = [result['translatedText'] for result in results]
    return translations

def main():
    print("Auto-Translation Starting...")
    
    # Read English source
    with codecs.open(EN_FILE, "r", "utf-8") as f:
        en_text = f.read()
    en_labels = extract_labels(en_text)
    
    # Check if Google Cloud credentials are set
    if not os.environ.get('GOOGLE_APPLICATION_CREDENTIALS'):
        print("WARNING: GOOGLE_APPLICATION_CREDENTIALS environment variable not set.")
        print("Please set this to your Google Cloud service account key file path.")
        print("For example: export GOOGLE_APPLICATION_CREDENTIALS='/path/to/your-project-credentials.json'")
        return
    
    # Process each language file
    for lang_file in LANG_FILES:
        lang_code = os.path.splitext(os.path.basename(lang_file))[0]
        if lang_code not in LANG_CODES:
            print(f"Skipping {lang_code} - no mapping defined.")
            continue
        
        google_lang_code = LANG_CODES[lang_code]
        print(f"Processing {lang_code} -> {google_lang_code}...")
        
        # Read existing translations
        with codecs.open(lang_file, "r", "utf-8") as f:
            lang_text = f.read()
        lang_labels = extract_labels(lang_text)
        
        # Find missing translations
        _, untranslated = compare_labels(en_labels, lang_labels)
        if not untranslated:
            print(f"No missing translations for {lang_code}!")
            continue
        
        print(f"Found {len(untranslated)} missing translations for {lang_code}.")
        
        # Translate missing entries
        to_translate = list(untranslated.values())
        keys_to_translate = list(untranslated.keys())
        
        try:
            # Split into smaller batches if needed (Google Translate has limits)
            batch_size = 100
            translated_texts = []
            
            for i in range(0, len(to_translate), batch_size):
                batch = to_translate[i:i+batch_size]
                print(f"Translating batch {i//batch_size + 1}/{(len(to_translate)-1)//batch_size + 1}...")
                batch_translations = translate_text(batch, google_lang_code)
                translated_texts.extend(batch_translations)
                
                # Be nice to the API with a short pause between batches
                if i + batch_size < len(to_translate):
                    time.sleep(1)
            
            # Create dictionary of translations
            translations = dict(zip(keys_to_translate, translated_texts))
            
            # Update the HJSON file
            print(f"Updating {lang_file} with {len(translations)} translations...")
            update_hjson_with_translations(lang_file, keys_to_translate, translations)
            
            print(f"Successfully translated {len(translations)} entries for {lang_code}.")
            
        except Exception as e:
            print(f"Error translating {lang_code}: {str(e)}")

    print("Auto-translation complete!")

if __name__ == "__main__":
    main()