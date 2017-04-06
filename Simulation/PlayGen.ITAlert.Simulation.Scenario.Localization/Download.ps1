$url="https://docs.google.com/spreadsheets/d/1cr5ELg381XCVyiOF9RORv7N5rKxSonPhfkBN0Bc0kqw/export?format=xlsx&id=1cr5ELg381XCVyiOF9RORv7N5rKxSonPhfkBN0Bc0kqw&gid=976752819"
$xlsx = "ScenarioLocalization.xlsx" 
$xlsxPath = $xlsx
$jsonPath = "ScenarioLocalization.json" 
     
if(!(Split-Path -parent $xlsxPath) -or !(Test-Path -pathType Container (Split-Path -parent $xlsxPath))) { 
	$xlsxPath = Join-Path $pwd (Split-Path -leaf $xlsxPath) 
}      
Write-Output $xlsxPath

$ie = new-object -com "InternetExplorer.Application"
$ie.visible = $false
$ie.navigate($url)

while ($ie.Busy -eq $true)
{
	Start-Sleep -Milliseconds 1000;
}

$wshell = new-object -com wscript.shell
$wshell.appactivate("Save As")
$wshell.sendkeys("{RIGHT}{RIGHT}{DOWN}a")
$wshell.sendkeys("$xlsxPath")
$wshell.sendkeys("{ENTER}")
$wshell.sendkeys("y")

while ($ie.Busy -eq $true)
{
	Start-Sleep -Milliseconds 1000;
}

$wshell.sendkeys("%l")
$wshell.sendkeys("%c")

while (!(Test-Path $xlsxPath))
{
	Start-Sleep -Milliseconds 1000;
}

& ..\..\tools\exceltojsonconverter\exceltojsonconverter.exe $xlsx $jsonPath