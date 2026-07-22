import os
import uuid
import random

def get_guid():
    return uuid.uuid4().hex

class DialogueNode:
    def __init__(self, name, speaker, text, translated="", trigger_event="", end_event=""):
        self.name = name
        self.guid = get_guid()
        self.speaker = speaker
        self.text = text
        self.translated = translated
        self.trigger_event = trigger_event
        self.end_event = end_event
        self.responses = []

    def add_response(self, text, next_guid, event="", stt_word=""):
        self.responses.append({
            "text": text,
            "next_guid": next_guid,
            "event": event,
            "stt_word": stt_word
        })

    def to_yaml(self):
        yaml = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: 73ff519377b990a4c81cd1461a3c1f62, type: 3}}
  m_Name: {self.name}
  m_EditorClassIdentifier: 
  speakerName: {self.speaker}
  speakerPortrait: {{fileID: 0}}
  dialogueText: "{self.text}"
  translatedText: "{self.translated}"
  animationTrigger: 
  triggerEventName: "{self.trigger_event}"
  endEventName: "{self.end_event}"
  choices:"""
        if not self.responses:
            yaml += " []\n"
        else:
            yaml += "\n"
            for r in self.responses:
                yaml += f"""  - choiceText: "{r['text']}"
    nextNode: {{fileID: 11400000, guid: {r['next_guid']}, type: 2}}
    isWrong: 0
    choiceEvent: "{r['event']}"
    expectedSTTWord: "{r['stt_word']}"\n"""
        return yaml

# ---------------------------------------------------------
# Curriculum
# ---------------------------------------------------------
curriculum = [
    {"en": "good morning", "il": "naimbag a bigat", "cat": "Greetings", "ctx": "greeting someone at sunrise"},
    {"en": "good afternoon", "il": "naimbag a malem", "cat": "Greetings", "ctx": "greeting someone during midday"},
    {"en": "good evening", "il": "naimbag a rabii", "cat": "Greetings", "ctx": "greeting someone at night"},
    {"en": "hello", "il": "kumusta", "cat": "Greetings", "ctx": "greeting someone politely"},
    {"en": "how are you?", "il": "kumusta ka?", "cat": "Greetings", "ctx": "checking on someone's well-being"},
    {"en": "I'm fine", "il": "nasayaat ak", "cat": "Greetings", "ctx": "responding positively"},
    {"en": "goodbye", "il": "agpakada akon", "cat": "Greetings", "ctx": "bidding farewell"},
    {"en": "thank you", "il": "agyamanak", "cat": "Gratitude", "ctx": "expressing appreciation"},
    {"en": "you're welcome", "il": "awan anuman", "cat": "Gratitude", "ctx": "responding to thanks"},
    {"en": "yes", "il": "wen", "cat": "Responses", "ctx": "agreeing or affirming"},
    {"en": "no", "il": "haan", "cat": "Responses", "ctx": "disagreeing or declining"},
    {"en": "maybe", "il": "nalabit", "cat": "Responses", "ctx": "expressing uncertainty"},
    {"en": "please", "il": "pangngaasi", "cat": "Responses", "ctx": "making a polite request"},
    {"en": "I don't know", "il": "diak ammu", "cat": "Responses", "ctx": "admitting lack of knowledge"},
    {"en": "I understand", "il": "maawatak", "cat": "Responses", "ctx": "confirming comprehension"},
    {"en": "what is your name?", "il": "ania ti nagan mo?", "cat": "Identity", "ctx": "asking for someone's name"},
    {"en": "my name is...", "il": "ti nagan ko ket...", "cat": "Identity", "ctx": "introducing yourself"},
    {"en": "how old are you?", "il": "mano ti tawen mon?", "cat": "Identity", "ctx": "asking for age"},
    {"en": "I am... years old", "il": "siak ket... tawen", "cat": "Identity", "ctx": "stating your age"},
    {"en": "where are you from?", "il": "taga-ano ka?", "cat": "Identity", "ctx": "asking about origin"},
    {"en": "I am from...", "il": "taga... ak", "cat": "Identity", "ctx": "stating your hometown"},
    {"en": "I like...", "il": "kayat ko...", "cat": "Requests", "ctx": "expressing a preference"},
    {"en": "I don't like...", "il": "diak kayat...", "cat": "Requests", "ctx": "expressing dislike"},
    {"en": "I want...", "il": "kayat ko...", "cat": "Requests", "ctx": "stating a desire"},
    {"en": "I need...", "il": "masapul ko...", "cat": "Requests", "ctx": "expressing a necessity"},
    {"en": "help", "il": "tulong", "cat": "Requests", "ctx": "asking for assistance"},
    {"en": "where is...", "il": "sadino ti...", "cat": "Directions", "ctx": "asking for a location"},
    {"en": "here", "il": "ditoy", "cat": "Directions", "ctx": "indicating current location"},
    {"en": "there", "il": "idiay", "cat": "Directions", "ctx": "indicating a distant location"},
    {"en": "left", "il": "kannigid", "cat": "Directions", "ctx": "indicating left direction"},
    {"en": "right", "il": "kannawan", "cat": "Directions", "ctx": "indicating right direction"},
    {"en": "straight", "il": "diretso", "cat": "Directions", "ctx": "indicating moving forward"},
    {"en": "one", "il": "maysa", "cat": "Count", "ctx": "counting to one"},
    {"en": "two", "il": "dua", "cat": "Count", "ctx": "counting to two"},
    {"en": "three", "il": "tallo", "cat": "Count", "ctx": "counting to three"},
    {"en": "four", "il": "uppat", "cat": "Count", "ctx": "counting to four"},
    {"en": "five", "il": "lima", "cat": "Count", "ctx": "counting to five"},
    {"en": "six", "il": "innem", "cat": "Count", "ctx": "counting to six"},
    {"en": "seven", "il": "pito", "cat": "Count", "ctx": "counting to seven"},
    {"en": "eight", "il": "walo", "cat": "Count", "ctx": "counting to eight"},
    {"en": "nine", "il": "siam", "cat": "Count", "ctx": "counting to nine"},
    {"en": "ten", "il": "sangapulo", "cat": "Count", "ctx": "counting to ten"},
    {"en": "eat", "il": "mangan", "cat": "Action Verbs", "ctx": "talking about having a meal"},
    {"en": "drink", "il": "uminom", "cat": "Action Verbs", "ctx": "talking about consuming a beverage"},
    {"en": "sleep", "il": "maturog", "cat": "Action Verbs", "ctx": "talking about resting"},
    {"en": "go", "il": "mapan", "cat": "Action Verbs", "ctx": "talking about leaving"},
    {"en": "come", "il": "umay", "cat": "Action Verbs", "ctx": "talking about arriving"},
    {"en": "is", "il": "ket", "cat": "Linking Verbs", "ctx": "connecting subjects to complements"},
    {"en": "are", "il": "ket", "cat": "Linking Verbs", "ctx": "connecting plural subjects"},
    {"en": "I", "il": "siak", "cat": "Pronouns", "ctx": "referring to yourself"},
    {"en": "you", "il": "sika", "cat": "Pronouns", "ctx": "referring to the person you are talking to"},
    {"en": "he/she", "il": "isuna", "cat": "Pronouns", "ctx": "referring to a third person"},
    {"en": "we", "il": "datayo", "cat": "Pronouns", "ctx": "referring to yourself and others"},
    {"en": "they", "il": "isuda", "cat": "Pronouns", "ctx": "referring to a group of others"},
    {"en": "what", "il": "ania", "cat": "Interrogatives", "ctx": "asking for information"},
    {"en": "who", "il": "sino", "cat": "Interrogatives", "ctx": "asking about a person"},
    {"en": "where", "il": "sadino", "cat": "Interrogatives", "ctx": "asking about a location"},
    {"en": "when", "il": "kaano", "cat": "Interrogatives", "ctx": "asking about time"},
    {"en": "why", "il": "apay", "cat": "Interrogatives", "ctx": "asking for a reason"},
    {"en": "how", "il": "kasano", "cat": "Interrogatives", "ctx": "asking about a method"},
    {"en": "how many", "il": "mano", "cat": "Interrogatives", "ctx": "asking for a quantity"}
]

# I will include all 8 NPCs. If any are in T-pose, you can easily remove them from this list later!
npc_chain = [
    {"key": "Kalaw", "name": "Kalaw", "cat": "Mixed"},
    {"key": "Rayo", "name": "Rayo", "cat": "Mixed"},
    {"key": "AlingRosa", "name": "Aling Rosa", "cat": "Mixed"},
    {"key": "ApoLakay", "name": "Apo Lakay", "cat": "Mixed"},
    {"key": "Neneng", "name": "Neneng", "cat": "Mixed"},
    {"key": "AlingRiza", "name": "Aling Riza", "cat": "Mixed"},
    {"key": "Pedro", "name": "Pedro", "cat": "Mixed"},
    {"key": "LolaBebang", "name": "Lola Bebang", "cat": "Mixed"}
]

# Distribute curriculum evenly across all NPCs
words_per_npc = len(curriculum) // len(npc_chain)
for i in range(len(npc_chain)):
    start_idx = i * words_per_npc
    # Give the last NPC any remaining words
    end_idx = start_idx + words_per_npc if i < len(npc_chain) - 1 else len(curriculum)
    npc_chain[i]["words"] = curriculum[start_idx:end_idx]

all_nodes = []

for i, npc in enumerate(npc_chain):
    words = npc.get("words", [])
    if not words: continue
    
    # 1. Intro Node
    if i == 0:
        start_node = DialogueNode(f"{npc['key']}_Start", npc['name'], "Wait... is that... a fruit?")
        give_fruit_node = DialogueNode(f"{npc['key']}_GiveFruit", npc['name'], "Oh, thank you so much! This is exactly what I needed. I see you are new here.")
        intro_node = DialogueNode(f"{npc['key']}_Intro", npc['name'], "I am Kalaw, and I will be your guide. Let me teach you some basic words so you can talk to the locals.")
        
        start_node.add_response("Give fruit", give_fruit_node.guid)
        give_fruit_node.add_response("Next", intro_node.guid)
        all_nodes.extend([start_node, give_fruit_node])
    else:
        intro_node = DialogueNode(f"{npc['key']}_Start", npc['name'], f"Hello there! Kalaw told me you were coming. I am {npc['name']}. Let's learn some more Ilokano!")
        
    all_nodes.append(intro_node)

    # 2. Teaching Nodes (Simulation)
    prev_node = intro_node
    for j, w in enumerate(words):
        if j > 10: break # don't overwhelm with 30 words per NPC, limit to 10
        
        teach_text = f"The word '{w['il']}' translates to '{w['en']}'. This is commonly used for {w['ctx']}. Try saying it after me: '{w['il']}'."
        teach_node = DialogueNode(f"{npc['key']}_Teach_{j}", npc['name'], teach_text)
        
        prev_node.add_response("Got it!", teach_node.guid)
        all_nodes.append(teach_node)
        prev_node = teach_node
        
    # 3. Transition to Next NPC
    if i < len(npc_chain) - 1:
        next_npc = npc_chain[i + 1]
        end_node = DialogueNode(f"{npc['key']}_End", npc['name'], f"You are learning quickly! Go find {next_npc['name']} next, he will teach you more.")
        # IMPORTANT: Use end_event to properly set the objective AFTER dialogue ends!
        end_node.end_event = f"SetObjective_Talk to {next_npc['key']}"
        prev_node.add_response("Okay!", end_node.guid)
        all_nodes.append(end_node)
    else:
        end_node = DialogueNode(f"{npc['key']}_End", npc['name'], "That is all I have to teach you! Kalaw is waiting for you now to test everything you've learned. Go back to him!")
        end_node.end_event = "SetObjective_Talk to Kalaw"
        prev_node.add_response("Will do!", end_node.guid)
        all_nodes.append(end_node)

# --- Add Kalaw's Final Test Node ---
# This node will trigger when the player returns to Kalaw with "Talk to Kalaw" objective
# In InteractableNPC, if the objective is "Talk to Kalaw", Kalaw's QuestDialogue can map it to "Kalaw_Final"
test_intro = DialogueNode("Kalaw_Final", "Kalaw", "Ah! You have returned from speaking with Apo Lakay. Are you ready to review what you've learned?")
all_nodes.append(test_intro)

prev_node = test_intro
test_words = random.sample(curriculum, 5)

for i, w in enumerate(test_words):
    q_node = DialogueNode(f"Kalaw_Test_{i}", "Kalaw", f"What is the Ilokano word for '{w['en']}'? Do you remember?")
    a_node = DialogueNode(f"Kalaw_TestAns_{i}", "Kalaw", f"That's right! It is '{w['il']}'. Good job!")
    
    prev_node.add_response("Ready!", q_node.guid)
    q_node.add_response(f"Is it... {w['il']}?", a_node.guid)
    
    all_nodes.extend([q_node, a_node])
    prev_node = a_node

test_end = DialogueNode("Kalaw_TestEnd", "Kalaw", "You've passed the final review! You are now ready to explore Vigan. The Language Crystal is fully rekindled!")
test_end.end_event = "SetObjective_Explore Vigan"
prev_node.add_response("Thank you, Kalaw!", test_end.guid)
all_nodes.append(test_end)


dir_path = "Assets/Dialogues/GDD_Flow"
os.makedirs(dir_path, exist_ok=True)
for node in all_nodes:
    with open(os.path.join(dir_path, f"{node.name}.asset"), "w") as f:
        f.write(node.to_yaml())
        
    meta_content = f"""fileFormatVersion: 2
guid: {node.guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    with open(os.path.join(dir_path, f"{node.name}.asset.meta"), "w") as f:
        f.write(meta_content)

print(f"Generated {len(all_nodes)} dialogue nodes!")
