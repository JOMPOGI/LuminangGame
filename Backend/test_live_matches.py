import requests

url = "https://luminang-nlp-service.onrender.com/find_all_matches"
data = {
    "region": "BossBattle",
    "transcribed_text": "Tinagan ko kay Jerome."
}
response = requests.post(url, data=data)
print(f"Status Code: {response.status_code}")
try:
    print("Response JSON:")
    import json
    print(json.dumps(response.json(), indent=2))
except Exception as e:
    print(f"Error parsing response: {e}")
    print(response.text)
