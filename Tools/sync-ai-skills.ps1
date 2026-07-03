# Kayano monorepo — 同步 Cursor / Claude AI Skills
# 用法: .\Tools\sync-ai-skills.ps1

$ErrorActionPreference = 'Stop'
$Root = Split-Path $PSScriptRoot -Parent
$Client = Join-Path $Root 'KayanoClient'
$Server = Join-Path $Root 'KayanoServer'
$FantasySkillSource = 'E:\Kayano Project\Fantasy-2026.0.1023\Skills\fantasy-net'

function Ensure-Junction {
    param([string]$Link, [string]$Target)
    if (-not (Test-Path $Target)) { throw "Target not found: $Target" }
    $parent = Split-Path $Link -Parent
    if (-not (Test-Path $parent)) { New-Item -ItemType Directory -Path $parent -Force | Out-Null }
    if (Test-Path $Link) { Remove-Item -LiteralPath $Link -Recurse -Force }
    New-Item -ItemType Junction -Path $Link -Target $Target | Out-Null
    Write-Host "  OK: $Link -> $Target"
}

Write-Host 'KayanoClient .cursor/skills (from .claude/skills)...'
$clientSkills = @('tengine-dev', 'luban-dev', 'html-to-ugui', 'wiki-synchelper')
foreach ($s in $clientSkills) {
    Ensure-Junction (Join-Path $Client ".cursor\skills\$s") (Join-Path $Client ".claude\skills\$s")
}
Ensure-Junction (Join-Path $Client '.cursor\skills\fantasy-net') $FantasySkillSource

Write-Host 'KayanoServer fantasy-net...'
Ensure-Junction (Join-Path $Server '.cursor\skills\fantasy-net') $FantasySkillSource
Ensure-Junction (Join-Path $Server '.claude\skills\fantasy-net') $FantasySkillSource

Write-Host 'Done.'
