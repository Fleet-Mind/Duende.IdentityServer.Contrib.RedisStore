param (
    [string]$buildNumber
)

$projFile = "Duende.IdentityServer.Contrib.RedisStore\Duende.IdentityServer.Contrib.RedisStore.csproj"
$newProjFile = ($projFile + ".new")

$version = (Select-String "<Version" $projFile).ToString().Replace("<Version>", "").Replace("</Version>", "")
$asmblReleaseVersion = (($version.Substring($version.LastIndexOf(":") + 1).Trim()) + "." + $buildNumber)

New-Item version.txt
Add-Content -Path version.txt -Value $asmblReleaseVersion

New-Item $newProjFile

foreach ($line in (Get-Content $projFile)) {
	if ($line.Contains("<AssemblyVersion>")) {
		$ident = $line.Substring(0, $line.IndexOf("<"))
		
		Add-Content -Path $newProjFile -Value "$ident<AssemblyVersion>$asmblReleaseVersion</AssemblyVersion>"
		
		continue;
	}
	
	if ($line.Contains("<Version>")) {
		$ident = $line.Substring(0, $line.IndexOf("<"))
		
		Add-Content -Path $newProjFile -Value "$ident<Version>$asmblReleaseVersion</Version>"
		
		continue;
	}
	
	Add-Content -Path $newProjFile -Value $line
}

Remove-Item $projFile
Move-Item $newProjFile $projFile