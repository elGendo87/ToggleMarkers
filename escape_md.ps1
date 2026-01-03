param(
  [string]$InputFile,
  [string]$OutputFile
)

$content = Get-Content $InputFile -Raw
$escaped = [System.Security.SecurityElement]::Escape($content)
Set-Content $OutputFile $escaped -Encoding UTF8