import os
import logging
from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import JSONResponse, FileResponse
import subprocess
import tempfile
from pathlib import Path

app = FastAPI()
logging.basicConfig(level=logging.DEBUG)

logging.debug(f"Aktualny katalog roboczy: {os.getcwd()}")
logging.debug(f"Zawartość /app: {os.listdir('/app')}")
logging.debug(f"CKPT_DIR: {os.environ.get('CKPT_DIR')}")

@app.get("/health")
def health():
    return {"status": "ok"}

@app.post("/infer")
async def infer(file: UploadFile = File(...)):
    # tworzymy plik tymczasowy dla uploadu
    suffix = os.path.splitext(file.filename or "")[1] or ".jpg"
    with tempfile.NamedTemporaryFile(delete=False, suffix=suffix) as tmp:
        tmp.write(await file.read())
        tmp_path = tmp.name

    try:
        # katalog wyjściowy
        out_dir = "/app/output"
        os.makedirs(out_dir, exist_ok=True)

        # ścieżka do checkpointu modelu
        ckpt = os.environ.get(
            "CKPT_DIR", 
            "/app/experiment-real-milce/experiment-real-milce/_lr-0.0005_batch-2"
        )

        # --- uruchomienie inferencji ---
        cmd = ["bash", "/app/infer.sh", "-g", "", "-c", ckpt, "-o", out_dir, tmp_path]
        proc = subprocess.Popen(cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, universal_newlines=True)
        logs, _ = proc.communicate()
        rc = proc.returncode

        base = os.path.splitext(os.path.basename(tmp_path))[0]
        candidate = os.path.join(out_dir, base + ".png")

        print("=== LOGI INFERENCJI ===")
        print(logs)
        print("=======================")

        if rc != 0 or not os.path.exists(candidate):
            raise HTTPException(status_code=500, detail="Inferencja nie powiodła się")

        # --- uruchomienie wizualizacji bezpośrednio w pliku candidate ---
        vis_cmd = ["python", "/app/test/visualize.py", candidate, candidate]
        proc_vis = subprocess.Popen(vis_cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, universal_newlines=True)
        vis_logs, _ = proc_vis.communicate()
        if proc_vis.returncode != 0 or not os.path.exists(candidate):
            print("=== LOGI WIZUALIZACJI ===")
            print(vis_logs)
            print("=========================")
            raise HTTPException(status_code=500, detail="Wizualizacja nie powiodła się")

        tail = "\n".join(logs.splitlines()[-60:])

        # zwracamy ścieżkę do PNG (już po wizualizacji)
        return JSONResponse({
            "outputPath": candidate,
            "confidence": None,
            "extra": {"logs_tail": tail}
        })

    finally:
        try:
            os.remove(tmp_path)
        except Exception:
            pass

@app.get("/file")
def get_file(path: str):
    if not os.path.exists(path):
        raise HTTPException(status_code=404, detail="Nie znaleziono")
    return FileResponse(path, media_type="image/png")
