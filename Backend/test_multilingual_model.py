from huggingface_hub import InferenceClient

HF_API_TOKEN = "your_token_here"
MODEL_NAME = "sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2"

client = InferenceClient(token=HF_API_TOKEN)
scores = client.sentence_similarity(
    sentence="Tinagan ko kay Jerome.",
    other_sentences=[
        "naimbag a rabii",
        "pasayloa ko",
        "pwede nimo ko tabangan",
        "ti nagan ko ket ___",
        "ako si ___"
    ],
    model=MODEL_NAME
)
print("Scores:")
print(scores)
