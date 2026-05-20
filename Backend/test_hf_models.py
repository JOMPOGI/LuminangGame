import requests

HF_API_TOKEN = "your_token_here"

def test_model(model_name):
    url = f"https://api-inference.huggingface.co/models/{model_name}"
    headers = {
        "Authorization": f"Bearer {HF_API_TOKEN}"
    }
    payload = {
        "inputs": {
            "source_sentence": "query: hello",
            "sentences": ["query: hi", "query: bye"]
        }
    }
    response = requests.post(url, headers=headers, json=payload)
    print(f"Model: {model_name}")
    print(f"Status Code: {response.status_code}")
    print(f"Response: {response.text[:200]}")
    print("-" * 50)

# Test E5 and MiniLM
test_model("intfloat/multilingual-e5-small")
test_model("sentence-transformers/all-MiniLM-L6-v2")
