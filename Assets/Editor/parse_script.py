import re
import json

def parse():
    with open(r'c:\Users\Asus\Desktop\GameDev\Assets\Editor\MassiveScript.txt', 'r', encoding='utf-8') as f:
        text = f.read()
    
    # Split the text by "Word X:"
    chunks = re.split(r'Word \d+:', text)
    
    intro_text = chunks[0]
    words_data = []
    
    for i, chunk in enumerate(chunks[1:]):
        # We need to extract:
        # Title: HELLO → KUMUSTA
        # Speaker (from "Speaker — Teach:")
        # Teach Text (lines between Teach: and Player STT:)
        # STT Expected Word (from Player STT: word)
        # Success Text (lines between Success: and next Word or end)
        
        lines = chunk.strip().split('\n')
        
        title_line = lines[0].strip()
        
        # Find Teach marker
        teach_idx = -1
        stt_idx = -1
        success_idx = -1
        speaker = "Unknown"
        
        for j, line in enumerate(lines):
            if '— Teach:' in line:
                teach_idx = j
                speaker = line.split('—')[0].strip()
            elif line.startswith('Player STT:'):
                stt_idx = j
            elif '— Success:' in line:
                success_idx = j
                
        if teach_idx != -1 and stt_idx != -1 and success_idx != -1:
            teach_text = '\n'.join(lines[teach_idx+1:stt_idx]).replace('"', '').strip()
            stt_expected = lines[stt_idx].replace('Player STT:', '').strip()
            # Success text might have other quest markers after it, we take everything until the end of this chunk
            # Actually, let's just take lines after success_idx until a line that starts with something like "Word", "QUEST", or is empty?
            # Wait, the chunk IS split by "Word X:". So the rest of the chunk belongs to Success, EXCEPT for Quest headers.
            # We can filter out lines that are all caps or start with QUEST or number.
            
            success_lines = []
            for line in lines[success_idx+1:]:
                if line.startswith('QUEST') or re.match(r'^\d+\.', line) or 'MILESTONE UNLOCKED' in line:
                    success_lines.append(line.replace('"', '').strip())
                else:
                    success_lines.append(line.replace('"', '').strip())
            
            success_text = '\n'.join(success_lines).strip()
            
            words_data.append({
                "id": i + 1,
                "title": title_line,
                "speaker": speaker,
                "teachText": teach_text,
                "expectedSTT": stt_expected,
                "successText": success_text
            })
            
    root = { "words": words_data }
    with open(r'c:\Users\Asus\Desktop\GameDev\Assets\Editor\MassiveScript.json', 'w', encoding='utf-8') as f:
        json.dump(root, f, indent=4)

if __name__ == '__main__':
    parse()
