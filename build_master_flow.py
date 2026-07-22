import os
import uuid
import re

def get_guid(dir_path, name):
    asset_path = os.path.join(dir_path, f"{name}.asset")
    meta_path = asset_path + ".meta"
    if os.path.exists(meta_path):
        with open(meta_path, 'r') as f:
            for line in f:
                if line.startswith("guid:"):
                    return line.split(":")[1].strip()
    return uuid.uuid4().hex

class DialogueNode:
    def __init__(self, name, speaker, text, translated="", trigger_event="", end_event=""):
        self.name = name
        self.speaker = speaker
        self.text = text
        self.translated = translated
        self.trigger_event = trigger_event
        self.end_event = end_event
        self.choices = []
        self.guid = ""

    def add_choice(self, text, target_node, is_wrong=False, event=""):
        self.choices.append({
            'text': text,
            'target_node': target_node,
            'is_wrong': is_wrong,
            'event': event
        })

    def write_yaml(self, dir_path):
        asset_path = os.path.join(dir_path, f"{self.name}.asset")

        clean_text = self.text.replace('"', '\\"')
        clean_trans = self.translated.replace('"', '\\"')
        
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
  speakerName: "{self.speaker}"
  speakerPortrait: {{fileID: 0}}
  dialogueText: "{clean_text}"
  translatedText: "{clean_trans}"
  animationTrigger: 
  triggerEventName: "{self.trigger_event}"
  endEventName: "{self.end_event}"
  choices:
"""
        for choice in self.choices:
            next_node = choice['target_node']
            next_guid = next_node.guid if next_node else ""
            next_str = f"{{fileID: 11400000, guid: {next_guid}, type: 2}}" if next_guid else "{fileID: 0}"
            clean_choice = choice['text'].replace('"', '\\"')
            asset_content += f"""  - choiceText: "{clean_choice}"
    nextNode: {next_str}
    isWrong: {1 if choice['is_wrong'] else 0}
    choiceEvent: "{choice['event']}"
"""
        with open(asset_path, "w") as f:
            f.write(asset_content)

        meta_path = asset_path + ".meta"
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
        with open(meta_path, "w") as f:
            f.write(meta_content)

def split_text_to_nodes(base_name, speaker, long_text, all_nodes_list):
    """
    Splits long text blocks into smaller 1-2 sentence segments (max ~130 chars each)
    chained with empty choice text so the built-in NEXT button drives progression smoothly.
    """
    if len(long_text) <= 140:
        n = DialogueNode(base_name, speaker, long_text)
        all_nodes_list.append(n)
        return n, n

    sentences = re.split(r'(?<=[.!?])\s+', long_text.strip())
    chunks = []
    curr = ""
    for s in sentences:
        if len(curr) + len(s) > 130 and curr:
            chunks.append(curr.strip())
            curr = s
        else:
            curr += (" " + s) if curr else s
    if curr:
        chunks.append(curr.strip())

    if len(chunks) <= 1:
        n = DialogueNode(base_name, speaker, long_text)
        all_nodes_list.append(n)
        return n, n

    first_node = None
    prev_node = None
    for idx, chunk in enumerate(chunks):
        name = f"{base_name}_part_{idx+1}" if idx > 0 else base_name
        node = DialogueNode(name, speaker, chunk)
        all_nodes_list.append(node)
        if prev_node:
            prev_node.add_choice("", node, False, "")
        else:
            first_node = node
        prev_node = node

    return first_node, prev_node

# Complete master dataset of Calle Crisologo NPCs & Dialogues
npcs_data = [
    {
        "key": "Kalaw", "name": "Kalaw", "stages": [
            {
                "id": "Start",
                "fetch_intro": "Squawk! Oh... is that a fruit in your hand? May I have some? Flying under this hot Vigan sun has completely worn me out...",
                "fetch_complete": "Mmm, sweet and juicy! Thank you so much, traveler! Ah, where are my manners—I am Kalaw, your companion and guide through these lands! Wait a second... look at that ancient anting-anting pendant resting on your chest! Feel that faint hum? The Ilocos Language Crystal inside it is sleeping. To charge it up and awaken its power, you've got to explore Calle Crisologo and speak with the locals in their native tongue! As you practice and learn how Ilocanos talk in their daily lives, your pendant will charge until it shines in full brilliance! We've got 81 native words to discover across this town! Let's start with the absolute basics—breaking the ice with the townsfolk!",
                "next_obj": "SetObjective_Talk to Ronnie",
                "words": [
                    {
                        "en": "hello", "il": "kumusta",
                        "model": "You can’t just stand there staring at the locals like a cobblestone! What’s the friendly word Ilocanos use to say hello when meeting someone? It's 'kumusta'!",
                        "react": "Kumusta! Nailed it! Look, your pendant just gave its very first warm spark of light!"
                    },
                    {
                        "en": "how are you?", "il": "kumusta ka?",
                        "model": "Don't stop at hello! Show some real warmth—how do you ask a neighbor how they're doing today? Say 'kumusta ka?'!",
                        "react": "Kumusta ka! Ah, see that smile? You're making friends already!"
                    },
                    {
                        "en": "I'm fine", "il": "nasayaat ak",
                        "model": "When they ask how your trip to Vigan is going, tell them you're doing great with 'nasayaat ak'!",
                        "react": "Nasayaat ak! That’s the spirit! Keep that energy up!"
                    },
                    {
                        "en": "good morning", "il": "naimbag a bigat",
                        "model": "The sun is shining over the tiled roofs! Greet the vendors starting their day by saying 'naimbag a bigat'!",
                        "react": "Naimbag a bigat! You've got the morning down! Go meet Ronnie down the street for the next challenges!"
                    }
                ]
            }
        ]
    },
    {
        "key": "Ronnie", "name": "Ronnie", "stages": [
            {
                "id": "Start",
                "intro": "Hey there! Kalaw said you're charging up that glowing pendant!",
                "next_obj": "SetObjective_Talk to Ronnie again",
                "words": [
                    {
                        "en": "I understand", "il": "maawatan ko",
                        "model": "Hey there! Kalaw said you're charging up that glowing pendant! Did you know the orange color of the empanada crust comes from achuete seeds? Say 'maawatan ko' if you get it!",
                        "react": "Maawatan ko! You're sharp!"
                    },
                    {
                        "en": "I don't understand", "il": "diak maawatan",
                        "model": "If an elder starts talking ultra-fast about Spanish-era architecture, naturally admit 'I don't understand' with 'diak maawatan'!",
                        "react": "Diak maawatan... Talk to me again for the Identity trials!"
                    }
                ]
            },
            {
                "id": "Identity",
                "intro": "Let's learn how to share our names!",
                "next_obj": "SetObjective_Talk to Sally",
                "words": [
                    {
                        "en": "what is your name", "il": "ania ti nagan mo",
                        "model": "We've been walking down Calle Crisologo together! Ask a fellow traveler 'what is your name' with 'ania ti nagan mo'!",
                        "react": "Ania ti nagan mo! Ti nagan ko ket Ronnie!"
                    },
                    {
                        "en": "my name is ___", "il": "ti nagan ko ket ___",
                        "model": "Now introduce yourself! Tell people 'my name is ___' using 'ti nagan ko ket ___'!",
                        "react": "Awesome to meet you! Go meet Sally near the brick arch to complete Level I!"
                    }
                ]
            }
        ]
    },
    {
        "key": "Sally", "name": "Sally", "stages": [
            {
                "id": "Start",
                "intro": "Welcome to my neighborhood!",
                "next_obj": "SetObjective_Talk to Sally for Requests",
                "words": [
                    {
                        "en": "where are you from", "il": "taga sadino ka",
                        "model": "Welcome to my neighborhood! When meeting someone new on these cobblestones, ask 'where are you from' with 'taga sadino ka'!",
                        "react": "Taga sadino ka! Taga Vigan ak—born right in these ancestral houses!"
                    },
                    {
                        "en": "I am from ___", "il": "taga ___ ak",
                        "model": "Now tell me where your adventure started! Say 'taga ___ ak'!",
                        "react": "Your pendant is blazing with light! Stick with me for the Request trials!"
                    }
                ]
            },
            {
                "id": "Requests",
                "intro": "Let's learn how to ask for help!",
                "next_obj": "SetObjective_Talk to Lito",
                "words": [
                    {
                        "en": "help me", "il": "tulunganak",
                        "model": "If your bag drops or you're stuck in a pinch, speak the direct call for help: 'tulunganak'!",
                        "react": "Tulunganak! We've always got your back!"
                    },
                    {
                        "en": "can you help me", "il": "mabalin kadi a tulunganak",
                        "model": "How about asking politely? Ask 'can you help me' with 'mabalin kadi a tulunganak'!",
                        "react": "Mabalin kadi a tulunganak! Perfect! Find Tour Guide Lito for the rest of these requests!"
                    }
                ]
            }
        ]
    },
    {
        "key": "Lito", "name": "Lito", "stages": [
            {
                "id": "Start",
                "intro": "Welcome to the streets!",
                "next_obj": "SetObjective_Talk to Lito for Directions",
                "words": [
                    {
                        "en": "please wait", "il": "urayennak",
                        "model": "Hold up! A horse carriage is clip-clopping past! Tell the tour group to 'please wait for me' with 'urayennak'!",
                        "react": "Urayennak! Safety first on these narrow streets!"
                    },
                    {
                        "en": "give me ___", "il": "ikanmo man ___",
                        "model": "Ask someone to hand over a handy item, like 'give me the map', using 'ikanmo man'!",
                        "react": "Ikanmo man! Here is your map of Vigan!"
                    },
                    {
                        "en": "can I ask", "il": "mabalin kadi agsaludsod",
                        "model": "Before interrupting a guide, politely ask 'can I ask a question' with 'mabalin kadi agsaludsod'!",
                        "react": "Mabalin kadi agsaludsod! Ask away! Talk to me again for the Direction trials!"
                    }
                ]
            },
            {
                "id": "Directions",
                "intro": "Let's learn how to navigate!",
                "next_obj": "SetObjective_Talk to Apo Lakay",
                "words": [
                    {
                        "en": "go straight", "il": "agdiretso",
                        "model": "To reach Plaza Salcedo without making any wrong turns, the command 'go straight' is 'agdiretso'!",
                        "react": "Agdiretso! Straight ahead! Go find Apo Lakay by the stone well for more pathfinding!"
                    }
                ]
            }
        ]
    },
    {
        "key": "ApoLakay", "name": "Apo Lakay", "stages": [
            {
                "id": "Start",
                "intro": "Ah, young seeker! Let me share directions with you.",
                "next_obj": "SetObjective_Talk to Tomas",
                "words": [
                    {
                        "en": "turn left", "il": "agliko iti kannigid",
                        "model": "Ah, young seeker! To find the Burnay pottery yard, turn left at the well with 'agliko iti kannigid'!",
                        "react": "Agliko iti kannigid! Turn left, the clay kilns are right there!"
                    },
                    {
                        "en": "turn right", "il": "agliko iti kannawan",
                        "model": "And if you're heading toward the grand cathedral bell tower instead, turn right with 'agliko iti kannawan'!",
                        "react": "Agliko iti kannawan! Turn right at the corner!"
                    },
                    {
                        "en": "go up", "il": "umuli iti ngato",
                        "model": "To get the best view of the whole province from the top of the tower, say 'go up' with 'umuli iti ngato'!",
                        "react": "Umuli iti ngato! Climb up to the top!"
                    },
                    {
                        "en": "go down", "il": "bumaba",
                        "model": "When you're done admiring the view, tell your companions to 'go down' with 'bumaba'!",
                        "react": "Bumaba! Mind your step! Go see Tomas at the pottery yard!"
                    }
                ]
            }
        ]
    },
    {
        "key": "Tomas", "name": "Tomas", "stages": [
            {
                "id": "Start",
                "intro": "Welcome to the clay yard!",
                "next_obj": "SetObjective_Talk to Klara",
                "words": [
                    {
                        "en": "stop here", "il": "agsardeng ditoy",
                        "model": "Welcome to the clay yard! Command your party to 'stop here' in front of my wheel with 'agsardeng ditoy'!",
                        "react": "Agsardeng ditoy! Perfect landing spot!"
                    },
                    {
                        "en": "come here", "il": "umay ditoy",
                        "model": "Watch how I shape this wet clay! Call your friends over by saying 'come here' with 'umay ditoy'!",
                        "react": "Umay ditoy! Check out this pottery action!"
                    },
                    {
                        "en": "go there", "il": "mapan idiay",
                        "model": "Whew, the dragon kiln is scorching hot! Tell everyone to 'go there' toward the shaded trees with 'mapan idiay'!",
                        "react": "Mapan idiay! Cool shade is much safer!"
                    },
                    {
                        "en": "follow me", "il": "surotennak",
                        "model": "I'm walking over to the jar drying racks. Tell visitors to 'follow me' with 'surotennak'!",
                        "react": "Surotennak! Visit Klara at the antique shop for the last direction command!"
                    }
                ]
            }
        ]
    },
    {
        "key": "Klara", "name": "Klara", "stages": [
            {
                "id": "Start",
                "intro": "Welcome to my antique shop!",
                "next_obj": "SetObjective_Talk to Klara for Counting",
                "words": [
                    {
                        "en": "wait here", "il": "uray ditoy",
                        "model": "Tell customers to 'wait here' while you fetch a rare Spanish-era relic using 'uray ditoy'!",
                        "react": "Uray ditoy! Talk to me again for the Counting trials!"
                    }
                ]
            },
            {
                "id": "Counting",
                "intro": "Let's count together!",
                "next_obj": "SetObjective_Talk to Tala",
                "words": [
                    {
                        "en": "one", "il": "maysa",
                        "model": "Let's test your counting skills! The word for 'one' in Ilocano is 'maysa'!",
                        "react": "Maysa! Just one rare chest!"
                    },
                    {
                        "en": "two", "il": "dua",
                        "model": "How about the word for 'two'? It's 'dua'!",
                        "react": "Dua matching chairs!"
                    },
                    {
                        "en": "three", "il": "tallo",
                        "model": "And the word for 'three' is 'tallo'!",
                        "react": "Tallo oil lamps! Head over to Tala the Bagnet seller for more numbers!"
                    }
                ]
            }
        ]
    },
    {
        "key": "Tala", "name": "Tala", "stages": [
            {
                "id": "Start",
                "intro": "Slicing up crispy pork!",
                "next_obj": "SetObjective_Talk to Mang Lance",
                "words": [
                    {
                        "en": "four", "il": "uppat",
                        "model": "Slicing up crispy pork! The word for 'four' is 'uppat'!",
                        "react": "Uppat crispy slabs!"
                    },
                    {
                        "en": "five", "il": "lima",
                        "model": "The word for 'five' is 'lima'!",
                        "react": "Lima kilos of delicious Bagnet!"
                    },
                    {
                        "en": "six", "il": "innem",
                        "model": "The word for 'six' is 'innem'!",
                        "react": "Innem family recipes!"
                    },
                    {
                        "en": "seven", "il": "pito",
                        "model": "The word for 'seven' is 'pito'!",
                        "react": "Pito o'clock sharp! Run to Mang Lance the Kalesa driver to finish counting!"
                    }
                ]
            }
        ]
    },
    {
        "key": "MangLance", "name": "Mang Lance", "stages": [
            {
                "id": "Start",
                "fetch_intro": "Whoa, hold on! My carriage wheel pin popped out! Please 'Find Wheel Pin' so my horse Barnaby and I can ride safely!",
                "fetch_complete": "Whew, thanks for fixing my wheel! Let me teach you: the word for 'eight' is 'walo'!",
                "next_obj": "SetObjective_Talk to Mang Lance for Verbs",
                "words": [
                    {
                        "en": "eight", "il": "walo",
                        "model": "Whew, thanks for fixing my wheel! Let me teach you: the word for 'eight' is 'walo'!",
                        "react": "Walo treats for Barnaby!"
                    },
                    {
                        "en": "nine", "il": "siam",
                        "model": "The word for 'nine' is 'siam'!",
                        "react": "Siam in the morning!"
                    },
                    {
                        "en": "ten", "il": "sangapulo",
                        "model": "The big number 'ten' is 'sangapulo'!",
                        "react": "Sangapulo! Stay with me for Action Verbs!"
                    }
                ]
            },
            {
                "id": "Verbs",
                "intro": "Let's look at action words!",
                "next_obj": "SetObjective_Talk to Rayo",
                "words": [
                    {
                        "en": "eat", "il": "mangan",
                        "model": "After all that walking, Ilocanos say 'eat' with 'mangan'!",
                        "react": "Mangan! Time for longganisa! Go see Rayo the photographer for more action verbs!"
                    }
                ]
            }
        ]
    },
    {
        "key": "Rayo", "name": "Rayo", "stages": [
            {
                "id": "Start",
                "intro": "Click! Let's take some photos!",
                "next_obj": "SetObjective_Talk to Aling Rosa",
                "words": [
                    {
                        "en": "drink", "il": "uminom",
                        "model": "Click! Great pose! It's warm today—the verb meaning to 'drink' cold sugarcane juice is 'uminom'!",
                        "react": "Uminom! Refreshing!"
                    },
                    {
                        "en": "go", "il": "mapan",
                        "model": "The verb meaning to 'go' toward a scenic photo spot is 'mapan'!",
                        "react": "Mapan! Let's head over to the arch!"
                    },
                    {
                        "en": "come", "il": "umay",
                        "model": "Tell someone to 'come' over here for a snapshot using 'umay'!",
                        "react": "Umay! Stand right by the window!"
                    },
                    {
                        "en": "sleep", "il": "maturog",
                        "model": "After exploring all day, the verb meaning to 'sleep' and recharge is 'maturog'!",
                        "react": "Maturog well! Catch up with Aling Rosa to wrap up Action Verbs!"
                    }
                ]
            }
        ]
    },
    {
        "key": "AlingRosa", "name": "Aling Rosa", "stages": [
            {
                "id": "Start",
                "fetch_intro": "Ay, my thread broke! Please 'Find Thread' so I can finish weaving these colorful souvenirs!",
                "fetch_complete": "Aww, thank you! The verb meaning to 'see' or look at my bright textiles is 'makita'!",
                "next_obj": "SetObjective_Talk to Aling Rosa for Linking Verbs",
                "words": [
                    {
                        "en": "see", "il": "makita",
                        "model": "Aww, thank you! The verb meaning to 'see' or look at my bright textiles is 'makita'!",
                        "react": "Makita! Look at these lovely patterns!"
                    },
                    {
                        "en": "hear", "il": "mangngeg",
                        "model": "The verb meaning to 'hear' horse hooves clattering down Calle Crisologo is 'mangngeg'!",
                        "react": "Mangngeg! The sound of the kalesas!"
                    },
                    {
                        "en": "speak", "il": "agsao",
                        "model": "The action verb meaning to 'speak' and practice Ilocano with the locals is 'agsao'!",
                        "react": "Agsao! Stay here for Linking Verbs!"
                    }
                ]
            },
            {
                "id": "Linking",
                "intro": "Let's connect subjects and descriptions!",
                "next_obj": "SetObjective_Talk to Neneng",
                "words": [
                    {
                        "en": "am", "il": "ket / siak ti",
                        "model": "In 'Siak ket weaver' (I am a weaver), the word that connects subject and description is 'ket' or 'siak ti'!",
                        "react": "Ket! Spot on!"
                    },
                    {
                        "en": "is", "il": "ket / isu ti",
                        "model": "In 'Calle Crisologo is the heart of Vigan', the phrase acting as 'is' or 'is the' is 'ket' or 'isu ti'!",
                        "react": "Isu ti heart of our heritage!"
                    },
                    {
                        "en": "are", "il": "ket / da",
                        "model": "When describing a plural subject like 'they are weavers', we use 'ket' or the plural marker 'da'!",
                        "react": "Da weavers! Keeping our town proud!"
                    },
                    {
                        "en": "was", "il": "ket idi / na",
                        "model": "To describe history like 'this street was a river port', the phrase connecting past states is 'ket idi' or 'na'!",
                        "react": "Ket idi! Such rich history!"
                    },
                    {
                        "en": "were", "il": "ket idi",
                        "model": "To state 'our ancestors were skilled merchants', the plural past phrase used is 'ket idi'!",
                        "react": "They ket idi merchants! Catch up with student Neneng next!"
                    }
                ]
            }
        ]
    },
    {
        "key": "Neneng", "name": "Neneng", "stages": [
            {
                "id": "Start",
                "intro": "Hello! I'm Neneng, a student here.",
                "next_obj": "SetObjective_Talk to Aling Riza",
                "words": [
                    {
                        "en": "become", "il": "agbalin",
                        "model": "Hey! The verb describing how you can 'become' fluent with practice is 'agbalin'!",
                        "react": "Agbalin fluent! Keep it up!"
                    },
                    {
                        "en": "seem", "il": "kasla / kasla ket",
                        "model": "The word expressing that walking here 'seems' like time travel is 'kasla' or 'kasla ket'!",
                        "react": "Kasla traveling back to the Spanish era!"
                    },
                    {
                        "en": "remain", "il": "agtalinaed",
                        "model": "The verb declaring that our culture will 'remain' strong forever is 'agtalinaed'!",
                        "react": "Agtalinaed! Always strong!"
                    },
                    {
                        "en": "stay", "il": "agtalinaed / agyan",
                        "model": "The verb you use to say you hope to 'stay' or reside in Vigan longer is 'agyan' or 'agtalinaed'!",
                        "react": "Agyan a few more days! Go see Aling Riza at the restaurant to finish connectors!"
                    }
                ]
            }
        ]
    },
    {
        "key": "AlingRiza", "name": "Aling Riza", "stages": [
            {
                "id": "Start",
                "intro": "Welcome to my restaurant!",
                "next_obj": "SetObjective_Talk to Aling Riza for Pronouns",
                "words": [
                    {
                        "en": "feel", "il": "marikna",
                        "model": "The verb allowing us to 'feel' or experience warm hospitality is 'marikna'!",
                        "react": "Marikna! Talk to me again for Pronouns!"
                    }
                ]
            },
            {
                "id": "Pronouns",
                "intro": "Let's learn about pronouns!",
                "next_obj": "SetObjective_Talk to Pedro",
                "words": [
                    {
                        "en": "I", "il": "siak",
                        "model": "When someone asks who loves visiting Vigan, say 'I' with 'siak'!",
                        "react": "Siak! Glad you love our town!"
                    },
                    {
                        "en": "you", "il": "sika",
                        "model": "When handing a plate of food to a friend, say 'you' directly with 'sika'!",
                        "react": "Sika! Freshly cooked for you!"
                    },
                    {
                        "en": "he", "il": "isuna",
                        "model": "Referring to the tour guide over there, say 'he' with 'isuna'!",
                        "react": "Isuna knows all the stories! Go find Pedro the musician for more pronouns!"
                    }
                ]
            }
        ]
    },
    {
        "key": "Pedro", "name": "Pedro", "stages": [
            {
                "id": "Start",
                "intro": "Strumming my guitar...",
                "next_obj": "SetObjective_Talk to Lola Bebang",
                "words": [
                    {
                        "en": "she", "il": "isuna",
                        "model": "Strumming the guitar! In Ilocano, 'isuna' covers both 'he' and 'she'!",
                        "react": "Isuna sings with pure soul!"
                    },
                    {
                        "en": "we", "il": "dakkami",
                        "model": "When referring to our musical band—'we' who play for visitors—the word meaning 'we' is 'dakkami'!",
                        "react": "Dakkami love sharing folk music!"
                    },
                    {
                        "en": "they", "il": "isuda",
                        "model": "Look at the tourists dancing near the plaza! Refer to 'they' or 'them' with 'isuda'!",
                        "react": "Isuda are having a blast!"
                    },
                    {
                        "en": "me", "il": "siak",
                        "model": "When telling someone to come play music with 'me', the pronoun to use is 'siak'!",
                        "react": "Come play with siak! Head to Lola Bebang for the final pronoun challenges!"
                    }
                ]
            }
        ]
    },
    {
        "key": "LolaBebang", "name": "Lola Bebang", "stages": [
            {
                "id": "Start",
                "intro": "Welcome, child. Let's learn more pronouns.",
                "next_obj": "SetObjective_Talk to Lola Bebang for Interrogatives",
                "words": [
                    {
                        "en": "us", "il": "dakkami",
                        "model": "This town's heritage belongs to all of us! The word for 'us' is 'dakkami'!",
                        "react": "Dakkami are proud Ilocanos!"
                    },
                    {
                        "en": "them", "il": "isuda",
                        "model": "Share your happy travel stories with other travelers—say 'them' with 'isuda'!",
                        "react": "Share the joy with isuda! Stay with me for the ultimate quest!"
                    }
                ]
            },
            {
                "id": "Interrogatives",
                "intro": "Let's explore question words!",
                "next_obj": "SetObjective_Return to Kalaw for Final Test",
                "words": [
                    {
                        "en": "what", "il": "ania",
                        "model": "To ask questions and explore, you need the W-words! Ask 'what' using 'ania'!",
                        "react": "Ania was your favorite sight today?"
                    },
                    {
                        "en": "who", "il": "asino",
                        "model": "Ask 'who' came along with you on this trip using 'asino'!",
                        "react": "Asino is exploring with you?"
                    },
                    {
                        "en": "where", "il": "sadino",
                        "model": "Ask 'where' your travels take you next using 'sadino'!",
                        "react": "Sadino are you heading next?"
                    },
                    {
                        "en": "when", "il": "kaano",
                        "model": "Ask 'when' will you visit Vigan again using 'kaano'!",
                        "react": "Kaano will you come back?"
                    },
                    {
                        "en": "why", "il": "apay",
                        "model": "Ask 'why' don't you sit and enjoy the sunny plaza longer using 'apay'!",
                        "react": "Apay not relax a bit more?"
                    },
                    {
                        "en": "how", "il": "kasano",
                        "model": "Ask 'how' much Ilocano you've practiced today using 'kasano'!",
                        "react": "Look at kasano far you've come!"
                    },
                    {
                        "en": "how many", "il": "mano",
                        "model": "The final word of our town's adventures! Ask 'how many' native words you've practiced today using 'mano'!",
                        "react": "Mano! You've practiced all 81 words across Calle Crisologo! But wait—your anting-anting pendant is pulsing with energy. Head back to Kalaw at the plaza! As your companion and guide, he must put your skills to the Final Regional Evaluation before the crystal fully ignitesits power!"
                    }
                ]
            }
        ]
    }
]

out_dirs = ["Assets/Dialogues/GDD_Flow", "Assets/Dialogues/CalleCrisologo"]
for d in out_dirs:
    os.makedirs(d, exist_ok=True)

all_nodes = []

for npc in npcs_data:
    key = npc["key"]
    speaker = npc["name"]
    stages = npc["stages"]

    for stage in stages:
        stage_id = stage["id"]
        words = stage["words"]
        next_obj = stage["next_obj"]

        # Stage node name
        stage_prefix = f"{key}_{stage_id}" if stage_id != "Start" else f"{key}"

        # 1. Start dialogue / Fetch dialogue
        if "fetch_intro" in stage:
            start_node, last_intro = split_text_to_nodes(f"{stage_prefix}_Start", speaker, stage["fetch_intro"], all_nodes)
            complete_first, complete_last = split_text_to_nodes(f"{stage_prefix}_GiveFruit" if key == "Kalaw" else f"{stage_prefix}_GivePin" if key == "MangLance" else f"{stage_prefix}_GiveThread", speaker, stage["fetch_complete"], all_nodes)
            
            # Linear chain with empty choice to complete handover
            last_intro.add_choice("", complete_first, False, "")
            current_node = complete_last
        else:
            intro_text = stage.get("intro", "Greetings traveler! Let us speak.")
            start_node, current_node = split_text_to_nodes(f"{stage_prefix}_Start", speaker, intro_text, all_nodes)

        # 2. Iterate and chain all words in this stage
        for w_idx, wdata in enumerate(words):
            model_text = wdata["model"]
            react_text = wdata["react"]

            word_prefix = f"{stage_prefix}_Word_{w_idx}"
            model_first, model_last = split_text_to_nodes(f"{word_prefix}_Model", speaker, model_text, all_nodes)
            react_first, react_last = split_text_to_nodes(f"{word_prefix}_Success", speaker, react_text, all_nodes)

            # Link current node to this word's model
            current_node.add_choice("", model_first, False, "")
            # Link model node to reaction/success
            model_last.add_choice("", react_first, False, "")

            # Set current_node to react_last for the next iteration
            current_node = react_last

        # 3. Last reaction node in stage sets the objective / end event
        current_node.end_event = next_obj

# ── Kalaw Finale Evaluation Nodes ──────────────────────────────────────────
# Trigger: Return to Kalaw for Final Test
k_final_first, k_final_last = split_text_to_nodes("Kalaw_Final_Start", "Kalaw", "Squawk! Look at you! The anting-anting pendant around your neck is practically bursting with energy from your travels down Calle Crisologo! But hold your horses—or kalesas! Before we set sail, I need to test if you've truly mastered the ways of Ilocos. Prove your voice to the pendant one last time in my Final Evaluation!", all_nodes)

eval_tests = [
    {
        "prompt": "First challenge! How do you greet shopkeepers when the sun rises over the heritage houses? Remember: 'naimbag a bigat'!",
        "react": "Spot on! A proper morning greeting!"
    },
    {
        "prompt": "Second challenge! You're riding a kalesa down the cobblestones and want to tell the driver to go straight toward the plaza without turning. What do you command? Remember: 'agdiretso'!",
        "react": "Agdiretso! Straight ahead indeed!"
    },
    {
        "prompt": "Third challenge! Walking all day works up an appetite. What verb do Ilocanos use when it's time to sit down and eat delicious longganisa? Remember: 'mangan'!",
        "react": "Mangan! Haha, now you're making me hungry!"
    },
    {
        "prompt": "Final challenge! Declare who you are! Tell the world 'my name is' so your identity echoes through the archipelago! Remember: 'ti nagan ko ket [Your Name]'!",
        "react": "SQUAWK! SANGAPULO OVER SANGAPULO! 🌟 THE ILOCOS LANGUAGE CRYSTAL INSIDE YOUR PENDANT SHINES IN FULL BRILLIANCE! 🌟 Look at how far you've come from when we first met!"
    }
]

cur_eval = k_final_last
for t_idx, test in enumerate(eval_tests):
    prompt = test["prompt"]
    react = test["react"]

    eq_first, eq_last = split_text_to_nodes(f"Kalaw_Final_Test_{t_idx}_Q", "Kalaw", prompt, all_nodes)
    es_first, es_last = split_text_to_nodes(f"Kalaw_Final_Test_{t_idx}_S", "Kalaw", react, all_nodes)

    cur_eval.add_choice("", eq_first, False, "")
    eq_last.add_choice("", es_first, False, "")
    cur_eval = es_last

o1_first, o1_last = split_text_to_nodes("Kalaw_Final_Outro_1", "Kalaw", "SQUAWK! SANGAPULO OVER SANGAPULO! 🌟 THE ILOCOS LANGUAGE CRYSTAL INSIDE YOUR PENDANT SHINES IN FULL BRILLIANCE! 🌟 Look at how far you've come from when we first met! Great job, traveler! You've learned so much about how we speak and live here in Vigan, Ilocos Sur. Remember, Calle Crisologo and the whole Ilocos region will always be here, so you can drop by anytime to practice and chat with the locals!", all_nodes)
o1_last.end_event = "SetObjective_Explore Calle Crisologo!"

cur_eval.add_choice("", o1_first, False, "")

# 4. Generate all .asset files
for dir_path in out_dirs:
    for node in all_nodes:
        node.guid = get_guid(dir_path, node.name)
    for node in all_nodes:
        node.write_yaml(dir_path)

print(f"Successfully generated {len(all_nodes)} dialogue nodes driven by NEXT button in {out_dirs[0]} and {out_dirs[1]}")
