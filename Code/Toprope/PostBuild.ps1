param([string]$base = '')

$cacheManifest = $base + '\Content\Misc\cache.manifest'
$timestamp = Get-Date
$rev = 'rev ' + $timestamp.Ticks.ToString()

(Get-Content $cacheManifest) | 
Foreach-Object {$_ -replace 'rev\s+([0-9]+)', $rev} | 
Set-Content $cacheManifest