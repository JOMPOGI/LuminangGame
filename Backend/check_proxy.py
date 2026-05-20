import urllib.request
import os

print("Proxies detected by urllib:")
print(urllib.request.getproxies())

print("\nEnvironment proxy variables:")
for key in ["HTTP_PROXY", "HTTPS_PROXY", "ALL_PROXY", "NO_PROXY", "http_proxy", "https_proxy"]:
    if key in os.environ:
        print(f"  {key}: {os.environ[key]}")
    else:
        print(f"  {key}: Not Set")
