$installMySql = Read-Host "Should MySql be installed? (y/n)"
Write-Host $installMySql
if ($installMySql) {
    <# Action to perform if the condition is true #>
}


function Say-Hello {
    param (
        [string]$name
    )
    Write-Host "Hello, $name!"   
}

Say-Hello -name "John Doe"

