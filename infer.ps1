param (
    [string]$CheckpointDir,
    [string]$Image
)

if (-not (Test-Path $CheckpointDir)) {
    Write-Error "Checkpoint directory not found: $CheckpointDir"
    exit 1
}

if (-not (Test-Path $Image)) {
    Write-Error "Input image not found: $Image"
    exit 1
}

$inputName = [System.IO.Path]::GetFileNameWithoutExtension($Image)
$grayImage = "input-gray.jpg"
$instrImage = "instruction.png"

# 🟡 Python-скрипт создаёт входные изображения (вместо magick)
python prepare_input.py $Image $grayImage $instrImage
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to generate input files"
    exit 1
}

# 🟡 Запуск модели
python main.py --checkpoint_dir $CheckpointDir --notraining --batch_size=1 --dataset test --real_eval --real_img $grayImage --real_instruction $instrImage

# 🟡 Копирование результата
$output = "$CheckpointDir/eval/input-$inputName.png"
if (-Not (Test-Path $output)) {
    Write-Error "Output file not generated: $output"
    exit 1
}

$outputName = "$inputName.png"
Copy-Item $output $outputName
Write-Host "✅ Saved: $outputName"
