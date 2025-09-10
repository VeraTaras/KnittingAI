# KnittingAI

**KnittingAI** to system do analizy obrazów wyrobów dzianych i automatycznego generowania schematów wzorów.  
Projekt składa się z **Frontendu (React)**, **Backendu (ASP.NET Core Web API)** oraz **ML Server (Python, FastAPI)**.  
Komponenty komunikują się ze sobą za pomocą **Docker Compose**.

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
1. Otworzenie http://localhost:3000.

2. Przesłanie zdjęcia fragmentu wzoru dziewiarskiego o wymiarach 160 x 160 (.png lub .jpg). Można wybrać spośród zdjęć w folderze ml_server\images

3. Po wciżnięciu "Załadować" Backend  prześle plik do serwera ML.

4. Serwer ML przetworzy obraz i zapisze wynik w ml_server/output.

5. Backend skopiuje PNG do wwwroot/results/{projectId}/.

6. Frontend wyświetli oryginalny obraz i wynik. Wynik też można będzie znaleźć w folderze ml_server\output


