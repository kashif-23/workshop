
param(
    [string]$SourceFile = 'C:\Users\kashi\Documents\BSidesPRG\FinancialDetails.txt',
    [string]$DestFile = 'C:\Users\kashi\Documents\BSidesPRG\BackupFiles\FinancialDetails.enc',
    [string]$Key = 'GOLTRALABS'
)

# Calculate MD5 hash
$md5 = New-Object -TypeName System.Security.Cryptography.MD5CryptoServiceProvider
$hash = [System.BitConverter]::ToString($md5.ComputeHash([System.IO.File]::ReadAllBytes($SourceFile))).Replace('-', '').ToLower()

# Read file bytes
$fileBytes = [System.IO.File]::ReadAllBytes($SourceFile)
$keyBytes = [System.Text.Encoding]::UTF8.GetBytes($Key)
$encryptedBytes = New-Object byte[] $fileBytes.Length

# XOR encryption
for ($i = 0; $i -lt $fileBytes.Length; $i++) {
    $encryptedBytes[$i] = $fileBytes[$i] -bxor $keyBytes[$i % $keyBytes.Length]
}

# Save encrypted file
[System.IO.File]::WriteAllBytes($DestFile, $encryptedBytes)

# Output hash
Write-Output $hash
