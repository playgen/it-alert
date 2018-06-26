$xlsx = "ScenarioLocalization.xlsx" 
$xlsxPath = $xlsx
$jsonPath = "ScenarioLocalization.json" 
     
if(!(Split-Path -parent $xlsxPath) -or !(Test-Path -pathType Container (Split-Path -parent $xlsxPath))) { 
	$xlsxPath = Join-Path $pwd (Split-Path -leaf $xlsxPath) 
}      

& ..\tools\exceltojsonconverter\exceltojsonconverter.exe $xlsx $jsonPath