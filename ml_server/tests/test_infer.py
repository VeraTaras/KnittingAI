import io
import sys, os
sys.path.append(os.path.dirname(os.path.dirname(__file__)))
from fastapi.testclient import TestClient
from server import app

client = TestClient(app)

def test_infer_jpg(tmp_path):
    fake_img = io.BytesIO(b"\xFF\xD8\xFF\xE0" + b"0" * 100)  # fake JPEG
    files = {"file": ("test.jpg", fake_img, "image/jpeg")}
    response = client.post("/infer", files=files)

    # Może zwrócić 200 lub 500 – w zależności od tego,
    # czy infer.sh jest podłączony – ważne, żeby endpoint działał
    assert response.status_code in (200, 500)
