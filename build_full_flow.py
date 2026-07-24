import os
import uuid

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
        self.choices = []

    def add_choice(self, text, next_node_guid, is_wrong=False, event=""):
        self.choices.append({
            'text': text,
            'next_guid': next_node_guid,
            'is_wrong': is_wrong,
            'event': event
        })

    def write_yaml(self, dir_path):
        asset_content = f"""%YAML 1.1
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
  m_EditorClassIdentifier: Assembly-CSharp::DialogueNode
  speakerName: {self.speaker}
  speakerPortrait: {{fileID: 0}}
  dialogueText: {self.text}
  translatedText: {self.translated}
  animationTrigger: 
  triggerEventName: {self.trigger_event}
  endEventName: {self.end_event}
  choices:
"""
        for choice in self.choices:
            next_str = f"{{fileID: 11400000, guid: {choice['next_guid']}, type: 2}}" if choice['next_guid'] else "{fileID: 0}"
            asset_content += f"""  - choiceText: {choice['text']}
    nextNode: {next_str}
    isWrong: {1 if choice['is_wrong'] else 0}
    choiceEvent: {choice['event']}
"""
        with open(os.path.join(dir_path, f"{self.name}.asset"), "w") as f:
            f.write(asset_content)

        meta_content = f"""fileFormatVersion: 2
guid: {self.guid}
MonoImporter:
  externalObjects: {{}}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {{instanceID: 0}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
        with open(os.path.join(dir_path, f"{self.name}.asset.meta"), "w") as f:
            f.write(meta_content)

# 81 Words with Calle Crisologo (Vigan) Cultural Contexts!
curriculum = [
    # Kalaw (Basics & Greetings)
    {"en": "hello", "il": "kumusta", "cat": "Greetings", "story": "Welcome to Calle Crisologo! The heritage houses here have stood for centuries. To make friends with the locals, start with 'hello' (kumusta)."},
    {"en": "how are you?", "il": "kumusta ka?", "cat": "Greetings", "story": "Ilocanos are very hospitable. It is always polite to ask 'how are you?' (kumusta ka?)."},
    {"en": "I'm fine", "il": "nasayaat ak", "cat": "Greetings", "story": "If they ask about your trip to Vigan, tell them 'I'm fine' (nasayaat ak)."},
    {"en": "good morning", "il": "naimbag a bigat", "cat": "Greetings", "story": "The cobblestone streets are beautiful at sunrise. Greet the early vendors with 'good morning' (naimbag a bigat)."},
    {"en": "good afternoon", "il": "naimbag a malem", "cat": "Greetings", "story": "The Ilocos heat is strong! If you buy a cold drink, say 'good afternoon' (naimbag a malem)."},
    # Kyros (Souvenir Vendor)
    {"en": "good evening", "il": "naimbag a rabii", "cat": "Greetings", "story": "The ancestral houses look magical when the warm streetlamps turn on. Say 'good evening' (naimbag a rabii)."},
    {"en": "Good day", "il": "Naimbag nga aldaw", "cat": "Greetings", "story": "I am selling miniature wooden kalesas. Wish me a 'Good day' (Naimbag nga aldaw)."},
    {"en": "goodbye", "il": "agpakada akon", "cat": "Greetings", "story": "Enjoy your walk down the heritage street. Bid me 'goodbye' (agpakada akon)."},
    {"en": "thank you", "il": "agyamanak", "cat": "Gratitude", "story": "Here is your souvenir! Please show your appreciation by saying 'thank you' (agyamanak)."},
    {"en": "thank you very much", "il": "agyamanak unay", "cat": "Gratitude", "story": "I even gave you a discount! Say 'thank you very much' (agyamanak unay)."},
    # Irah (Inabel Weaver)
    {"en": "thank you for your help", "il": "agyamanak iti tulong mo", "cat": "Gratitude", "story": "Thank you for helping me fold this heavy Inabel blanket. Please say 'thank you for your help' (agyamanak iti tulong mo)."},
    {"en": "I am sorry", "il": "pakawanen nak", "cat": "Gratitude", "story": "Oh no, you accidentally dropped a woven scarf! Apologize by saying 'I am sorry' (pakawanen nak)."},
    {"en": "excuse me", "il": "dispensaren nak", "cat": "Gratitude", "story": "The tourists are blocking my display. Politely ask them to move. Say 'excuse me' (dispensaren nak)."},
    {"en": "yes", "il": "wen", "cat": "Responses", "story": "Would you like to see how the traditional loom works? Answer 'yes' (wen)."},
    # Jom (Empanada Vendor)
    {"en": "no", "il": "saan", "cat": "Responses", "story": "Do you want extra vinegar on your Vigan Empanada? If it's too sour, say 'no' (saan)."},
    {"en": "okay", "il": "okay", "cat": "Responses", "story": "I will make it crispy with Vigan longganisa. Acknowledge this with 'okay' (okay)."},
    {"en": "I understand", "il": "maawatan ko", "cat": "Responses", "story": "Our empanada has a unique orange crust made of achuete. Do you know how it's made? Say 'I understand' (maawatan ko)."},
    {"en": "I don't understand", "il": "diak maawatan", "cat": "Responses", "story": "The recipe is a secret passed down for generations. It's fine to admit 'I don't understand' (diak maawatan)."},
    # Ronnie (Tourist)
    {"en": "what is your name", "il": "ania ti nagan mo", "cat": "Identity", "story": "I just arrived from Manila! You look like a local. Ask me 'what is your name' (ania ti nagan mo)."},
    {"en": "my name is ___", "il": "ti nagan ko ket ___", "cat": "Identity", "story": "I'm Ronnie! What about you? Say 'my name is ___' (ti nagan ko ket ___)."},
    {"en": "where are you from", "il": "taga sadino ka", "cat": "Identity", "story": "I love the Spanish colonial architecture here. Ask me 'where are you from' (taga sadino ka)."},
    {"en": "I am from ___", "il": "taga ___ ak", "cat": "Identity", "story": "Now tell me where you grew up! Say 'I am from ___' (taga ___ ak)."},
    # Sally (Local Resident)
    {"en": "help me", "il": "tulunganak", "cat": "Requests", "story": "I dropped my basket of Chichacorn on the cobblestones! Please 'help me' (tulunganak)."},
    {"en": "can you help me", "il": "mabalin kadi a tulunganak", "cat": "Requests", "story": "It is better to ask the neighbors politely. Say 'can you help me' (mabalin kadi a tulunganak)."},
    {"en": "please wait", "il": "urayennak", "cat": "Requests", "story": "I need to pick up all the scattered corn! Tell the passing kalesa to 'please wait' (urayennak)."},
    {"en": "give me ___", "il": "ikanmo man ___", "cat": "Requests", "story": "Can you hand me that broom? Say 'give me ___' (ikanmo man ___)."},
    # Lito (Tour Guide)
    {"en": "can I ask", "il": "mabalin kadi agsaludsod", "cat": "Requests", "story": "You want to know the history of the Syquia Mansion? Say 'can I ask' (mabalin kadi agsaludsod)."},
    {"en": "go straight", "il": "agdiretso", "cat": "Directions", "story": "To reach the Plaza Salcedo, just follow the cobblestone road. Tell your group to 'go straight' (agdiretso)."},
    {"en": "turn left", "il": "agliko iti kannigid", "cat": "Directions", "story": "To find the best burnay pottery, you must 'turn left' (agliko iti kannigid) at the next intersection."},
    {"en": "turn right", "il": "agliko iti kannawan", "cat": "Directions", "story": "To visit the Vigan Cathedral, tell the driver to 'turn right' (agliko iti kannawan)."},
    # Apo Lakay (Community Elder)
    {"en": "go up", "il": "umuli iti ngato", "cat": "Directions", "story": "Ah, the Bantay Bell Tower is beautiful. To see the view of the province, you must 'go up' (umuli iti ngato)."},
    {"en": "go down", "il": "bumaba", "cat": "Directions", "story": "Be careful on the brick stairs. It's time to 'go down' (bumaba)."},
    {"en": "stop here", "il": "agsardeng ditoy", "cat": "Directions", "story": "This ancestral house is a museum. Tell the tour group to 'stop here' (agsardeng ditoy)."},
    {"en": "come here", "il": "umay ditoy", "cat": "Directions", "story": "Look at the beautiful capiz shell windows! Call your friends and say 'come here' (umay ditoy)."},
    # Tomas (Pottery Maker)
    {"en": "go there", "il": "mapan idiay", "cat": "Directions", "story": "The clay kiln is extremely hot. For your safety, please 'go there' (mapan idiay)."},
    {"en": "follow me", "il": "surotennak", "cat": "Directions", "story": "I will show you how we mold the Burnay jars. Say 'follow me' (surotennak)."},
    {"en": "wait here", "il": "uray ditoy", "cat": "Directions", "story": "The clay needs time to dry. Tell the tourists to 'wait here' (uray ditoy)."},
    {"en": "one", "il": "maysa", "cat": "Count", "story": "I can only mold 'one' (maysa) large jar at a time."},
    # Klara (Antique Shop Owner)
    {"en": "two", "il": "dua", "cat": "Count", "story": "I have 'two' (dua) antique wooden chairs from the Spanish era."},
    {"en": "three", "il": "tallo", "cat": "Count", "story": "And 'three' (tallo) vintage oil lamps to light the house."},
    {"en": "four", "il": "uppat", "cat": "Count", "story": "You want to buy 'four' (uppat) silver spoons for your collection?"},
    {"en": "five", "il": "lima", "cat": "Count", "story": "That will cost 'five' (lima) hundred pesos, please."},
    # Tala (Bagnet Seller)
    {"en": "six", "il": "innem", "cat": "Count", "story": "I am slicing 'six' (innem) kilos of crispy Vigan Bagnet today!"},
    {"en": "seven", "il": "pito", "cat": "Count", "story": "We open at 'seven' (pito) in the morning for the early market goers."},
    {"en": "eight", "il": "walo", "cat": "Count", "story": "I have 'eight' (walo) bottles of spicy sukang Iloko left."},
    {"en": "nine", "il": "siam", "cat": "Count", "story": "By 'nine' (siam) o'clock, the street gets very busy."},
    # Mang Lance (Kutsero / Kalesa Driver)
    {"en": "ten", "il": "sangapulo", "cat": "Count", "story": "A ride around the heritage village costs several hundred, but the memories are a perfect 'ten' (sangapulo)."},
    {"en": "eat", "il": "mangan", "cat": "Action Verbs", "story": "After the kalesa ride, you must be hungry. It's time to 'eat' (mangan) longganisa!"},
    {"en": "drink", "il": "uminom", "cat": "Action Verbs", "story": "Try the local sugarcane juice to 'drink' (uminom)."},
    {"en": "go", "il": "mapan", "cat": "Action Verbs", "story": "Hop in! Where do you want to 'go' (mapan)?"},
    # Rayo (Photographer)
    {"en": "come", "il": "umay", "cat": "Action Verbs", "story": "The lighting is perfect! 'Come' (umay) stand by the vintage wooden doors."},
    {"en": "sleep", "il": "maturog", "cat": "Action Verbs", "story": "You look tired from walking. Make sure you 'sleep' (maturog) well tonight."},
    {"en": "see", "il": "makita", "cat": "Action Verbs", "story": "Look through my camera lens. What do you 'see' (makita)?"},
    {"en": "hear", "il": "mangngeg", "cat": "Action Verbs", "story": "Can you 'hear' (mangngeg) the clopping of the horse hooves on the stones?"},
    # Aling Rosa (Souvenir Auntie)
    {"en": "speak", "il": "agsao", "cat": "Action Verbs", "story": "Don't be shy! 'Speak' (agsao) to the locals, we love visitors!"},
    {"en": "am", "il": "ket", "cat": "Linking Verbs", "story": "I am a proud Ilocana. The word for 'am' or 'is' is (ket)."},
    {"en": "is", "il": "isu ti", "cat": "Linking Verbs", "story": "Calle Crisologo 'is' (isu ti) a UNESCO World Heritage site."},
    {"en": "are", "il": "da", "cat": "Linking Verbs", "story": "The houses 'are' (da) preserved beautifully."},
    # Lola Nida (Elderly Resident)
    {"en": "was", "il": "ket idi", "cat": "Linking Verbs", "story": "This street 'was' (ket idi) a bustling trade center centuries ago."},
    {"en": "were", "il": "ket idi", "cat": "Linking Verbs", "story": "My grandparents 'were' (ket idi) merchants here."},
    {"en": "become", "il": "agbalin", "cat": "Linking Verbs", "story": "It has 'become' (agbalin) the most famous street in the North."},
    {"en": "seem", "il": "kasla", "cat": "Linking Verbs", "story": "Walking here, you 'seem' (kasla) to travel back in time."},
    # Neneng (Student)
    {"en": "remain", "il": "agtalinaed", "cat": "Linking Verbs", "story": "We hope our culture will 'remain' (agtalinaed) strong forever."},
    {"en": "stay", "il": "agyan", "cat": "Linking Verbs", "story": "I hope you 'stay' (agyan) in Vigan a bit longer!"},
    {"en": "feel", "il": "marikna", "cat": "Linking Verbs", "story": "You can truly 'feel' (marikna) the history in the air."},
    {"en": "I", "il": "siak", "cat": "Pronouns", "story": "If someone asks who loves Vigan, say 'I' (siak) do!"},
    # Aling Riza (Restaurant Owner)
    {"en": "you", "il": "sika", "cat": "Pronouns", "story": "I will prepare a special Ilocano feast just for 'you' (sika)."},
    {"en": "he", "il": "isuna", "cat": "Pronouns", "story": "Did 'he' (isuna) try the pinakbet yet?"},
    {"en": "she", "il": "isuna", "cat": "Pronouns", "story": "Make sure 'she' (isuna) tries the sinanglao, too!"},
    {"en": "we", "il": "dakkami", "cat": "Pronouns", "story": "In our family, 'we' (dakkami) cook everything traditionally."},
    # Pedro (Musician)
    {"en": "they", "il": "isuda", "cat": "Pronouns", "story": "The tourists love the music. 'They' (isuda) are dancing!"},
    {"en": "me", "il": "siak", "cat": "Pronouns", "story": "Please, sing a folk song with 'me' (siak)."},
    {"en": "us", "il": "dakkami", "cat": "Pronouns", "story": "The heritage of this town belongs to 'us' (dakkami) all."},
    {"en": "them", "il": "isuda", "cat": "Pronouns", "story": "Share the joy with 'them' (isuda)."},
    # Lola Bebang
    {"en": "what", "il": "ania", "cat": "Interrogatives", "story": "Your journey is almost complete. 'What' (ania) was your favorite memory here?"},
    {"en": "who", "il": "asino", "cat": "Interrogatives", "story": "'Who' (asino) will you tell about Calle Crisologo?"},
    {"en": "where", "il": "sadino", "cat": "Interrogatives", "story": "'Where' (sadino) will your travels take you next?"},
    {"en": "when", "il": "kaano", "cat": "Interrogatives", "story": "'When' (kaano) will you visit us again?"},
    {"en": "why", "il": "apay", "cat": "Interrogatives", "story": "'Why' (apay) don't you buy one last souvenir before you go?"},
    {"en": "how", "il": "kasano", "cat": "Interrogatives", "story": "'How' (kasano) much have you learned?"},
    {"en": "how many", "il": "mano", "cat": "Interrogatives", "story": "Finally, 'how many' (mano) Ilocano words do you now carry in your heart?"}
]

npcs_list = [
    {"key": "Kalaw", "name": "Kalaw", "fetch": "Find Fruit"},
    {"key": "Kyros", "name": "Vendor Kyros", "fetch": ""},
    {"key": "Irah", "name": "Vendor Irah", "fetch": ""},
    {"key": "Jom", "name": "Vendor Jom", "fetch": ""},
    {"key": "Ronnie", "name": "Ronnie", "fetch": ""},
    {"key": "Sally", "name": "Sally", "fetch": ""},
    {"key": "Lito", "name": "Lito", "fetch": ""},
    {"key": "ApoLakay", "name": "Apo Lakay", "fetch": ""},
    {"key": "Tomas", "name": "Tomas", "fetch": ""},
    {"key": "Klara", "name": "Klara", "fetch": ""},
    {"key": "Tala", "name": "Tala", "fetch": ""},
    {"key": "MangLance", "name": "Mang Lance", "fetch": "Find Wheel Pin"},
    {"key": "Rayo", "name": "Rayo", "fetch": ""},
    {"key": "AlingRosa", "name": "Aling Rosa", "fetch": "Find Thread"},
    {"key": "LolaNida", "name": "Lola Nida", "fetch": ""},
    {"key": "Neneng", "name": "Neneng", "fetch": ""},
    {"key": "AlingRiza", "name": "Aling Riza", "fetch": ""},
    {"key": "Pedro", "name": "Pedro", "fetch": ""},
    {"key": "LolaBebang", "name": "Lola Bebang", "fetch": ""}
]

# Distribute 81 words across ALL 19 NPCs perfectly. 
# 81 / 19 = 4.26. Some get 4, some get 5.
for i in range(len(curriculum)):
    npc_idx = min(i // 4, 18) # 19 total NPCs, max index 18
    if "words" not in npcs_list[npc_idx]:
        npcs_list[npc_idx]["words"] = []
    npcs_list[npc_idx]["words"].append(curriculum[i])

all_nodes = []

# Generate Dialogue Trees
for i, npc in enumerate(npcs_list):
    key = npc["key"]
    speaker = npc["name"]
    words = npc.get("words", [])
    fetch = npc["fetch"]
    
    # Next NPC formatting for SetupSTTFlow.cs
    base_next_npc = npcs_list[i+1]["key"] if i < 18 else "Calle Crisologo Restored"
    if base_next_npc == "MangLance": base_next_npc = "Mang Lance"
    if base_next_npc == "AlingRosa": base_next_npc = "Aling Rosa"
    if base_next_npc == "AlingRiza": base_next_npc = "Aling Riza"
    if base_next_npc == "LolaNida": base_next_npc = "Lola Nida"
    if base_next_npc == "LolaBebang": base_next_npc = "Lola Bebang"
    if base_next_npc == "ApoLakay": base_next_npc = "Apo Lakay"

    end_obj = f"SetObjective_Calle Crisologo Restored" if i == 18 else f"SetObjective_Talk to {base_next_npc}"

    start_node_guid = ""
    current_node = None

    if fetch != "":
        if key == "Kalaw":
            intro = DialogueNode(f"{key}_01_Start", speaker, f"I am {speaker}, your guide. I am weak... Please '{fetch}' so I can teach you the basics.", "", "", f"SetObjective_{fetch}")
        else:
            intro = DialogueNode(f"{key}_01_Start", speaker, f"I need your help before we speak! Please '{fetch}'.", "", "", f"SetObjective_{fetch}")
        all_nodes.append(intro)
        
        current_node = DialogueNode(f"{key}_02_Found", speaker, f"Thank you! Now, let us practice.")
        all_nodes.append(current_node)
    else:
        current_node = DialogueNode(f"{key}_01_Start", speaker, f"Greetings traveler. Let us converse.")
        all_nodes.append(current_node)

    # Chain the STT words
    for w_idx, word in enumerate(words):
        en = word["en"]
        il = word["il"]
        cat = word["cat"]
        story = word["story"]
        
        q_node = DialogueNode(f"{key}_Word_{w_idx}_Question", speaker, story)
        all_nodes.append(q_node)
        
        current_node.add_choice("Continue", q_node.guid, False, "")
        
        is_last_word = (w_idx == len(words) - 1)
        
        if is_last_word:
            s_node = DialogueNode(f"{key}_Word_{w_idx}_Success", speaker, f"Excellent! Now go talk to {base_next_npc}.", "", "", end_obj)
            f_node = DialogueNode(f"{key}_Word_{w_idx}_Fail", speaker, f"Not quite, but you will learn in time. Go talk to {base_next_npc}.", "", "", end_obj)
        else:
            s_node = DialogueNode(f"{key}_Word_{w_idx}_Success", speaker, f"Perfect!")
            f_node = DialogueNode(f"{key}_Word_{w_idx}_Fail", speaker, f"Close enough.")
            
        all_nodes.extend([s_node, f_node])
        
        q_node.add_choice(il, s_node.guid, False, "")
        q_node.add_choice(f"Say {il}", f_node.guid, False, "")
        
        current_node = s_node 
        
        if not is_last_word:
            f_node.add_choice("Continue", "", False, "") 

    fail_nodes = [n for n in all_nodes if n.name.startswith(f"{key}_Word_") and n.name.endswith("_Fail")]
    question_nodes = [n for n in all_nodes if n.name.startswith(f"{key}_Word_") and n.name.endswith("_Question")]
    
    for f_idx, f_node in enumerate(fail_nodes):
        if f_idx < len(question_nodes) - 1:
            f_node.choices[0]['next_guid'] = question_nodes[f_idx + 1].guid

out_dir = "Assets/Dialogues/CalleCrisologo"
os.makedirs(out_dir, exist_ok=True)
for node in all_nodes:
    node.write_yaml(out_dir)

print(f"Generated {len(all_nodes)} narrative dialogue assets covering the 81-word curriculum in {out_dir}")
