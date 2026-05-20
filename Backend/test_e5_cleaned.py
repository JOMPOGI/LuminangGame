from huggingface_hub import InferenceClient

HF_API_TOKEN = "your_token_here"
MODEL_NAME = "intfloat/multilingual-e5-small"

client = InferenceClient(token=HF_API_TOKEN)
scores = client.sentence_similarity(
    sentence="query: tinagan ko kay jerome",
    other_sentences=[
        "passage: ti nagan ko ket",
        "passage: ako si"
    ],
    model=MODEL_NAME
)
print("Scores with cleaned targets:")
print(scores)
