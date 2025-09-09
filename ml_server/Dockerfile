FROM python:3.6

# Устанавливаем зависимости для работы Python и утилит
RUN apt-get update && apt-get install -y \
    libglib2.0-0 libsm6 libxrender1 libxext6 imagemagick dos2unix \
 && rm -rf /var/lib/apt/lists/*

# Рабочая директория внутри контейнера
WORKDIR /app

# Копируем зависимости
COPY requirements-infer.txt /app/

# pip <22 — последняя версия, совместимая с Python 3.6
RUN python -m pip install --upgrade "pip<22" \
 && pip install --no-cache-dir -r requirements-infer.txt

# Копируем проект в контейнер
COPY . /app

# Чиним формат и права для скрипта
RUN find /app -type f -name "*.sh" -exec dos2unix {} \; \
 && find /app -type f -name "*.sh" -exec chmod +x {} \;

# Открываем порт для FastAPI
EXPOSE 8000

# Переменные окружения (отключаем CUDA, используем CPU)
ENV CUDA_VISIBLE_DEVICES=""
ENV MPLBACKEND="Agg"

# Запускаем сервер автоматически при старте контейнера
CMD ["uvicorn", "server:app", "--host", "0.0.0.0", "--port", "8000"]
