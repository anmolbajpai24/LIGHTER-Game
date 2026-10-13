param(
    [ValidateSet('Windows', 'Android', 'WindowsQA', 'Validate')]
    [string]$Target = 'Windows',
    [string]$UnityPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.9f1\Editor\Unity.exe',
    [switch]$UseShortJavaSocketPath
)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
if (-not (Test-Path -LiteralPath $UnityPath)) { throw 'Pass -UnityPath pointing to the Unity 6000.3.9f1 editor.' }
$logRoot = Join-Path $projectRoot 'Logs'
New-Item -ItemType Directory -Path $logRoot -Force | Out-Null
$logPath = Join-Path $logRoot ("build-{0}-{1}.log" -f $Target, (Get-Date -Format 'yyyyMMdd-HHmmss'))
$info = [System.Diagnostics.ProcessStartInfo]::new()
$info.FileName = $UnityPath
$info.UseShellExecute = $false
$info.CreateNoWindow = $true
$info.Arguments = '-batchmode -nographics -quit -projectPath "' + $projectRoot + '" -executeMethod ReleaseBuild.' + $Target + ' -logFile "' + $logPath + '"'
if ($UseShortJavaSocketPath) {
    $socketRoot = Join-Path $projectRoot '.build-temp'
    if ($socketRoot.Length -gt 75) { throw 'Use a shorter project path for the Java socket workaround (project plus .build-temp must be <= 75 characters).' }
    New-Item -ItemType Directory -Path $socketRoot -Force | Out-Null
    $option = ' "-Djdk.net.unixdomain.tmpdir=' + $socketRoot + '"'
    # Assign the child environment explicitly; do not change user or system settings.
    $info.EnvironmentVariables['JAVA_TOOL_OPTIONS'] = ($env:JAVA_TOOL_OPTIONS + $option).Trim()
    $info.EnvironmentVariables['_JAVA_OPTIONS'] = ($env:_JAVA_OPTIONS + $option).Trim()
}
Write-Output ("Started {0:o}: {1}. Log: {2}" -f (Get-Date), $Target, $logPath)
$process = [System.Diagnostics.Process]::Start($info)
$process.WaitForExit()
if ($process.ExitCode -ne 0) { throw "Unity exited $($process.ExitCode). Read $logPath" }
Write-Output ("Succeeded {0:o}: {1}" -f (Get-Date), $Target)
