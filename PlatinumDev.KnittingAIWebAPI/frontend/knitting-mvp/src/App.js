import React, { useState } from "react";

function App() {
  const [file, setFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState({ pngUrl: "", svgData: "" });

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

  const handleUpload = async () => {
    if (!file) return;
    setLoading(true);

    const formData = new FormData();
    formData.append("file", file);

    try {
      // Используем относительный путь — прокси из package.json
      const response = await fetch("/projects", {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        throw new Error(`Ошибка запроса: ${response.status}`);
      }

      const data = await response.json();
      console.log("Ответ от backend:", data);

      // Формируем полный URL PNG
      const pngUrl = data.imageUrl.startsWith("http")
        ? data.imageUrl
        : `${window.location.origin}${data.imageUrl}`;

      setResult({
      pngUrl: data.imageUrl.startsWith("http")
        ? data.imageUrl
        : `http://localhost:8080${data.imageUrl}`, // теперь URL ведет на backend
      svgData: "w fazie rozwoju",
    });
    } catch (err) {
      console.error(err);
      alert("Ошибка при обработке файла");
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
        {file ? file.name : "Перетащите файл сюда или выберите через кнопку"}
      </div>

      <input type="file" onChange={handleFileChange} />
      <button
        onClick={handleUpload}
        disabled={!file || loading}
        style={{ marginLeft: "10px" }}
      >
        Загрузить
      </button>

      {loading && <div style={{ marginTop: "10px" }}>⏳ Обработка...</div>}

      {result.pngUrl && (
        <div style={{ marginTop: "20px" }}>
          <h3>Результаты</h3>
          <div>
            <strong>Исходное изображение:</strong>
            <br />
            <img
              src={URL.createObjectURL(file)}
              alt="Original"
              style={{ maxWidth: "100%" }}
            />
          </div>
          <div>
            <strong>PNG из backend:</strong>
            <br />
            <img
              src={result.pngUrl}
              alt="PNG Result"
              style={{ maxWidth: "100%" }}
            />
          </div>
          <div>
            <strong>SVG результат:</strong>
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
