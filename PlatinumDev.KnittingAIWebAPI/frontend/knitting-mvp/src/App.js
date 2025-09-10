import React, { useState } from "react";

function App() {
  const [file, setFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState({ pngUrl: "", svgData: "" });

  // Obsługa zmiany pliku przez input
  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    if (selectedFile) setFile(selectedFile);
  };

  // Obsługa przeciągania pliku
  const handleDrop = (e) => {
    e.preventDefault();
    const droppedFile = e.dataTransfer.files[0];
    if (droppedFile) setFile(droppedFile);
  };

  const handleDragOver = (e) => e.preventDefault();

  const handleUpload = async () => {
    if (!file) return;
    setLoading(true);

    const formData = new FormData();
    formData.append("file", file);

    try {
      const response = await fetch("/projects", {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        throw new Error(`Błąd żądania: ${response.status}`);
      }

      const data = await response.json();
      console.log("Odpowiedź z backendu:", data);

      setResult({
        pngUrl: data.imageUrl.startsWith("http")
          ? data.imageUrl
          : `http://localhost:8080${data.imageUrl}`,
        svgData: "w fazie rozwoju",
      });
    } catch (err) {
      console.error(err);
      alert("Błąd podczas przetwarzania pliku");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ padding: "20px", maxWidth: "800px", margin: "0 auto" }}>
      <h2>Knitting AI MVP</h2>

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
          <div>
            <strong>Wynik SVG:</strong>
            <div
              dangerouslySetInnerHTML={{ __html: result.svgData }}
              style={{ border: "1px solid #ccc", padding: "10px" }}
            />
          </div>
        </div>
      )}
    </div>
  );
}

export default App;
