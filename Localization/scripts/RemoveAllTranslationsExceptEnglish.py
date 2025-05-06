import os

# Get the absolute path of the script's directory
script_dir = os.path.dirname(os.path.abspath(__file__))
localization_dir = os.path.join(script_dir, '../')

# Loop through all ".hjson" files in the localization directory
# and delete all except "en-US.hjson"
for root, dirs, files in os.walk(localization_dir):
    for file in files:
        if file.endswith('.hjson') and file != 'en-US.hjson':
            os.remove(os.path.join(root, file))
            print(file)
        elif file == 'TranslationsNeeded.txt':
            os.remove(os.path.join(root, file))
            print(file)