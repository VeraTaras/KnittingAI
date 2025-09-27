import React, { useState, useEffect } from "react";

function App() {
  const [file, setFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState({ pngUrl: "", svgData: "" });
  const [projects, setProjects] = useState([]); // список всех проектов
  const [selectedProject, setSelectedProject] = useState(null); // проект по ID

  // ==========================
  // Obsługa plików
  // ==========================
  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    if (selectedFile) setFile(selectedFile);
  };

  const handleDrop = (e) => {
    e.preventDefault();
    const droppedFile = e.dataTransfer.files[0];
    if (droppedFile) setFile(droppedFile);
  };

  const handleDragOver = (e) => e.preventDefault();

  // ==========================
  // POST /projects (tworzenie nowego projektu)
  // ==========================
  const handleUpload = async () => {
    if (!file) return;
    setLoading(true);

    const formData = new FormData();
    formData.append("file", file);

    try {
      const response = await fetch("http://localhost:8080/projects", {
        method: "POST",
        body: formData,
      });

      if (!response.ok) throw new Error(`Błąd żądania: ${response.status}`);

      const data = await response.json();
      console.log("✅ Nowy projekt:", data);

      setResult({
        pngUrl: data.imageUrl.startsWith("http")
          ? data.imageUrl
          : `http://localhost:8080${data.imageUrl}`,
        svgData: "w fazie rozwoju",
      });

      // automatyczne odświeżenie listy projektów
      fetchProjects();
    } catch (err) {
      console.error(err);
      alert("Błąd podczas przetwarzania pliku");
    } finally {
      setLoading(false);
    }
  };

  // ==========================
  // GET /projects (lista projektów)
  // ==========================
  const fetchProjects = async () => {
    try {
      const res = await fetch("http://localhost:8080/projects");
      if (!res.ok) throw new Error("Błąd pobierania projektów");

      const data = await res.json();
      setProjects(data);
    } catch (err) {
      console.error("❌", err);
    }
  };

  // ==========================
  // GET /projects/{id} (pojedynczy projekt)
  // ==========================
  const fetchProjectById = async (id) => {
    try {
      const res = await fetch(`http://localhost:8080/projects/${id}`);
      if (res.status === 404) {
        alert("⚠️ Projekt nie znaleziony");
        return;
      }
      if (!res.ok) throw new Error("Błąd pobierania projektu");

      const data = await res.json();
      console.log("📌 Wybrany projekt:", data);
      setSelectedProject(data);
    } catch (err) {
      console.error("❌", err);
    }
  };

  // automatyczne pobranie listy projektów przy starcie
  useEffect(() => {
    fetchProjects();
  }, []);

  return (
    <div style={{ padding: "20px", maxWidth: "1000px", margin: "0 auto" }}>
      <h2>Knitting AI MVP</h2>

      {/* Sekcja upload */}
      <div
        onDrop={handleDrop}
        onDragOver={handleDragOver}
        style={{
          border: "2px dashed #ccc",
          padding: "20px",
          marginBottom: "10px",
          textAlign: "center",
        }}
      >
        {file ? file.name : "Przeciągnij plik tutaj"}
      </div>

      <label
        htmlFor="fileInput"
        style={{
          display: "inline-block",
          padding: "10px 20px",
          backgroundColor: "#007bff",
          color: "white",
          borderRadius: "4px",
          cursor: "pointer",
          marginRight: "10px",
        }}
      >
        Wybierz plik
      </label>
      <input
        id="fileInput"
        type="file"
        onChange={handleFileChange}
        style={{ display: "none" }}
      />

      <button
        onClick={handleUpload}
        disabled={!file || loading}
        style={{ padding: "10px 20px", cursor: "pointer" }}
      >
        Załaduj
      </button>

      {loading && <div style={{ marginTop: "10px" }}>⏳ Przetwarzanie...</div>}

      {/* Sekcja wyników */}
      {result.pngUrl && (
        <div style={{ marginTop: "20px" }}>
          <h3>Wyniki</h3>
          <div>
            <strong>Obraz źródłowy:</strong>
            <br />
            <img
              src={URL.createObjectURL(file)}
              alt="Oryginał"
              style={{ maxWidth: "100%" }}
            />
          </div>
          <div>
            <strong>PNG z backendu:</strong>
            <br />
            <img
              src={result.pngUrl}
              alt="Wynik PNG"
              style={{ maxWidth: "100%" }}
            />
          </div>
        </div>
      )}

      {/* Sekcja lista projektów */}
      <div style={{ marginTop: "30px" }}>
        <h3>📂 Wszystkie projekty</h3>
        <ul>
          {projects.map((p) => (
            <li key={p.id}>
              <button
                onClick={() => fetchProjectById(p.id)}
                style={{ cursor: "pointer" }}
              >
                {p.name || p.id}
              </button>
            </li>
          ))}
        </ul>
      </div>

      {/* Sekcja wybranego projektu */}
      {selectedProject && (
        <div style={{ marginTop: "20px" }}>
          <h3>📌 Wybrany projekt</h3>
          <pre>{JSON.stringify(selectedProject, null, 2)}</pre>
        </div>
      )}
    </div>
  );
}

export default App;
