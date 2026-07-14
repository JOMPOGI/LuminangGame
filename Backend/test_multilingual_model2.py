from huggingface_hub import InferenceClient

HF_API_TOKEN = "your_token_here"
MODEL_NAME = "sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2"

client = InferenceClient(token=HF_API_TOKEN)
scores = client.sentence_similarity(
    sentence="naimbag a bigat",
    other_sentences=[
        "naimbag a bigat",
        "good morning",
        "maayong buntag",
        "hello",
        "naimbag a rabii"
    ],
    model=MODEL_NAME
)
print("Scores:")
print(scores)
