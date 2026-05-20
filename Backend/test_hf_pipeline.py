import requests

HF_API_TOKEN = "your_token_here"

def test_pipeline_endpoint(model_name):
    # Testing the feature-extraction pipeline endpoint
    url = f"https://api-inference.huggingface.co/pipeline/feature-extraction/{model_name}"
    headers = {
        "Authorization": f"Bearer {HF_API_TOKEN}"
    }
    payload = {
        "inputs": ["query: hello", "query: hi"]
    }
    response = requests.post(url, headers=headers, json=payload)
    print(f"Model: {model_name} (Pipeline Endpoint)")
    print(f"Status Code: {response.status_code}")
    print(f"Response: {response.text[:200]}")
    print("-" * 50)

test_pipeline_endpoint("intfloat/multilingual-e5-small")
test_pipeline_endpoint("sentence-transformers/all-MiniLM-L6-v2")
