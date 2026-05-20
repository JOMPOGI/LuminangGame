import requests

HF_API_TOKEN = "your_token_here"

url = "https://api-inference.huggingface.co/models/intfloat/multilingual-e5-small"
headers = {
    "Authorization": f"Bearer {HF_API_TOKEN}"
}
payload = {
    "inputs": {
        "source_sentence": "query: hello",
        "sentences": ["query: hi"]
    }
}
response = requests.post(url, headers=headers, json=payload)
print(f"Status Code: {response.status_code}")
print("Response Headers:")
for k, v in response.headers.items():
    print(f"  {k}: {v}")
