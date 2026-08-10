param([string]$path)
(get-content "$($path)web.config") -replace ' -argFile IISExeLauncherArgs.txt', '' | out-file "$($path)web.config" -Encoding UTF8
