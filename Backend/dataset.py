import os
import json
import re

class ControlledDataset:
    def __init__(self):
        self.phrases = []
        self.load_dataset()

    def load_dataset(self):
        # Resolve path to Assets/Resources/LuminangPhrases.json
        current_dir = os.path.dirname(os.path.abspath(__file__))
        json_path = os.path.join(current_dir, "..", "Assets", "Resources", "LuminangPhrases.json")
        
        if not os.path.exists(json_path):
            # Fallback path if run from elsewhere
            json_path = os.path.join(current_dir, "LuminangPhrases.json")
            
        if os.path.exists(json_path):
            try:
                with open(json_path, 'r', encoding='utf-8') as f:
                    data = json.load(f)
                    self.phrases = data.get("phrases", [])
                print(f"Successfully loaded {len(self.phrases)} phrases from dataset.")
            except Exception as e:
                print(f"Error loading dataset JSON: {e}")
        else:
            print(f"Dataset JSON not found at {json_path}!")


    def get_all_targets(self, region_mode):
        """
        Returns all valid phrases for evaluation under a specific region mode.
        """
        targets = []
        for entry in self.phrases:
            is_template = entry.get('type') == 'template'
            
            if region_mode == "Ilokano":
                val = entry.get('ilokano_target') if is_template else entry.get("ilokano", "")
                targets.append((entry, "ilokano", val))
            elif region_mode == "Cebuano":
                val = entry.get('cebuano_target') if is_template else entry.get("cebuano", "")
                targets.append((entry, "cebuano", val))
            elif region_mode == "BossBattle":
                # In Boss Battle, all regional languages are active
                for lang in ["ilokano", "cebuano"]:
                    val = entry.get(f'{lang}_target') if is_template else entry.get(lang, "")
                    if val and val != "___":
                        targets.append((entry, lang, val))
            else:
                # Default, include regional
                for lang in ["ilokano", "cebuano"]:
                    val = entry.get(f'{lang}_target') if is_template else entry.get(lang, "")
                    if val and val != "___":
                        targets.append((entry, lang, val))
        return targets

# Singleton instance
dataset = ControlledDataset()
