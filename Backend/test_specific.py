import difflib
from huggingface_hub import InferenceClient

def get_lexical_similarity(text1, text2):
    return difflib.SequenceMatcher(None, text1.lower(), text2.lower()).ratio()

HF_API_TOKEN = "your_token_here"
MODEL_NAME = "intfloat/multilingual-e5-small"
client = InferenceClient(token=HF_API_TOKEN)

t1 = "Anyatinagan mo."
targets = ["ania ti nagan mo", "agtalinaed / agyan"]

print(f"Transcript: '{t1}'")
for t2 in targets:
    lex = get_lexical_similarity(t1, t2)
    scores = client.sentence_similarity(
        sentence=f"query: {t1}",
        other_sentences=[f"passage: {t2}"],
        model=MODEL_NAME
    )
    sim_score = scores[0]
    final = sim_score
    if lex < 0.35:
        final -= 0.15
    print(f"Target: '{t2}' | E5: {sim_score:.4f} | Lexical: {lex:.4f} | Final: {final:.4f}")
