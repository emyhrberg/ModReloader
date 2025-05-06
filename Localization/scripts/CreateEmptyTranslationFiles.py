from pathlib import Path

# get orig_file from parent directory at en-US.hjson
orig_file = Path(__file__).resolve().parents[1] / 'en-US.hjson'

# check if orig_file exists
if not orig_file.exists():
    raise FileNotFoundError(f"Original file {orig_file} does not exist.")

# languages to translate to
langs = ['de-DE','it-IT','fr-FR','es-ES',
         'ru-RU','zh-Hans','pt-BR','pl-PL','uk-UA']

for lang in langs:
    # copy original file and rename it to the language code
    lang_file = orig_file.parent / f'{lang}.hjson'
    if not lang_file.exists():
        lang_file.write_text(orig_file.read_text(), encoding='utf-8')
        print(f"Created {lang_file.name} from {orig_file.name}.")
    else:
        print(f"{lang_file.name} already exists. Skipping.")