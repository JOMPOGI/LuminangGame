import re
import os

scene_path = r'C:\Users\Asus\Desktop\GameDev\Assets\Scenes\Environments\Calle_Crisologo.unity'
assets_dir = r'C:\Users\Asus\Desktop\GameDev\Assets'

with open(scene_path, 'r', encoding='utf-8') as f:
    content = f.read()

guids = set(re.findall(r'm_Script: \{fileID: 11500000, guid: ([a-f0-9]{32}), type: 3\}', content))

found_guids = set()
for root, dirs, files in os.walk(assets_dir):
    for file in files:
        if file.endswith('.meta'):
            with open(os.path.join(root, file), 'r', encoding='utf-8') as f:
                try:
                    meta_content = f.read()
                    match = re.search(r'guid: ([a-f0-9]{32})', meta_content)
                    if match:
                        found_guids.add(match.group(1))
                except:
                    pass

missing = guids - found_guids
print('Missing GUIDs:', missing)
