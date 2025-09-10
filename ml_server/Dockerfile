FROM python:3.6

# Instalujemy zależności potrzebne do działania Pythona i narzędzi
RUN apt-get update && apt-get install -y \
    libglib2.0-0 libsm6 libxrender1 libxext6 imagemagick dos2unix \
 && rm -rf /var/lib/apt/lists/*

# Katalog roboczy wewnątrz kontenera
WORKDIR /app

# Kopiujemy plik z zależnościami
COPY requirements-infer.txt /app/

# pip <22 — ostatnia wersja zgodna z Pythonem 3.6
RUN python -m pip install --upgrade "pip<22" \
 && pip install --no-cache-dir -r requirements-infer.txt

# Kopiujemy projekt do kontenera
COPY . /app

# Naprawiamy format i uprawnienia dla skryptów
RUN find /app -type f -name "*.sh" -exec dos2unix {} \; \
 && find /app -type f -name "*.sh" -exec chmod +x {} \;

# Otwieramy port dla FastAPI
EXPOSE 8000

# Zmienne środowiskowe (wyłączamy CUDA, używamy CPU)
ENV CUDA_VISIBLE_DEVICES=""
ENV MPLBACKEND="Agg"

# Automatycznie uruchamiamy serwer przy starcie kontenera
CMD ["uvicorn", "server:app", "--host", "0.0.0.0", "--port", "8000"]
