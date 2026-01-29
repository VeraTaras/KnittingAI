# KnittingAI

**KnittingAI** to system do analizy obrazów wyrobów dzianych i automatycznego generowania schematów wzorów.  
Projekt składa się z **Frontendu (React)**, **Backendu (ASP.NET Core Web API)** oraz **ML Server (Python, FastAPI)**.  
Komponenty komunikują się ze sobą za pomocą **Docker Compose**.

![Interface preview](KnittingAI/interface.png)

---

## Architektura

- **Frontend** — aplikacja React do ładowania obrazów i wyświetlania wyników.  
- **Backend** — ASP.NET Core Web API:
  - Odbiera obrazy od klienta.
  - Wysyła je na serwer ML.
  - Zapisuje projekt i zwraca wynik w formacie PNG.  
- **ML Server** — model uczenia maszynowego (Python), który analizuje obraz i zapisuje wynik w katalogu `output/`.  
- Pliki z `ml_server/output` są montowane w kontenerze backendu pod `/shared_output`.

---

## Wymagania
### Docker

### Git

### Opcjonalne
- Visual Studio Code (zalecane do edycji kodu)
- Rozszerzenia VS Code: C#, Python, Docker
- Postman lub inny klient HTTP (do testowania API)

---

## Uruchomienie projektu

### 1. Klonowanie repozytorium
```bash
git clone https://github.com/VeraTaras/KnittingAI.git
cd KnittingAI
```

---

### 2. Uruchomienie za pomocą Docker Compose
Przed wykonaniem polecenia otworzyć Docker Desktop.
```bash
docker compose up --build
```

Po uruchomieniu:

- **Frontend**: http://localhost:3000

- **Backend API**: http://localhost:8080

- **ML Server**: http://localhost:8000

---

### 3. Przygotowanie skryptów

Przed uruchomieniem należy upewnić się, że plik `infer.sh` ma poprawny format i prawa do wykonywania.  

```bash
docker-compose exec mlserver /bin/bash 
dos2unix infer.sh
chmod +x infer.sh
```
---

### 4. Użycie
1. Otwórz stronę: http://localhost:3000.

2. Prześlij obraz fragmentu wzoru dziewiarskiego o wymiarach 160 x 160 pikseli (.png lub .jpg). *Obrazy testowe znajdują się w folderze `ml_server/images`*.

3. Kliknij przycisk "Załaduj".

4. Poczekaj około 20 sekund, aż obraz zostanie przetworzony:
   - Backend prześle plik do serwera ML,
   - Serwer ML przetworzy obraz i zapisze wynik w `ml_server/output`,
   - Backend skopiuje plik PNG do `wwwroot/results/{projectId}/`,
   - Frontend wyświetli zarówno obraz wejściowy, jak i wynik.

5. Wynik można również znaleźć w folderze `ml_server/output`.




