from huggingface_hub import InferenceClient

HF_API_TOKEN = "your_token_here"
MODEL_NAME = "intfloat/multilingual-e5-small"

client = InferenceClient(token=HF_API_TOKEN)
scores1 = client.sentence_similarity(
    sentence="query: naimbag a bigat",
    other_sentences=[
        "passage: naimbag a bigat",
        "passage: good morning",
        "passage: maayong buntag",
        "passage: hello",
        "passage: naimbag a rabii"
    ],
    model=MODEL_NAME
)
print("Using query-passage formatting:")
print(scores1)

scores2 = client.sentence_similarity(
    sentence="query: Tinagan ko kay Jerome.",
    other_sentences=[
        "passage: naimbag a rabii",
        "passage: pasayloa ko",
        "passage: pwede nimo ko tabangan",
        "passage: ti nagan ko ket ___",
        "passage: ako si ___"
    ],
    model=MODEL_NAME
)
print("\nTinagan ko kay Jerome query-passage formatting:")
print(scores2)
