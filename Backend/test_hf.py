import requests

MODEL_NAME = "intfloat/multilingual-e5-small"
HF_API_TOKEN = "your_token_here"

url = f"https://api-inference.huggingface.co/models/{MODEL_NAME}"
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
print(f"Status Code: {response.status_code}")
print(f"Response: {response.text}")
