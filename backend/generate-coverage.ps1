# Clean previous results
if (Test-Path "./InventoryApp.Tests/TestResults") {
    Remove-Item "./InventoryApp.Tests/TestResults" -Recurse -Force
}

# Run the tests and collect coverage data
dotnet test --no-restore --verbosity normal

# Install the report generator tool if not already installed
dotnet tool install -g dotnet-reportgenerator-globaltool 2>&1 | Out-Null

# Generate the HTML report
reportgenerator `
    "-reports:./InventoryApp.Tests/TestResults/coverage.cobertura.xml" `
    "-targetdir:./InventoryApp.Tests/TestResults/CoverageReport" `
    "-reporttypes:Html;Badges"

# Open the report in the default browser
$reportPath = "./InventoryApp.Tests/TestResults/CoverageReport/index.html"
if (Test-Path $reportPath) {
    Write-Host "`nCoverage report generated successfully!"
    Write-Host "Opening report in default browser..."
    Start-Process $reportPath
} else {
    Write-Host "Error: Coverage report was not generated."
} 