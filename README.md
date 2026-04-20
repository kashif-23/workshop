# Hostname Checker - Win32 API Edition

A C# console application that retrieves the computer hostname using Win32 API calls and verifies if it matches the target system name "goltralabs".

## Project Description

This program:
1. Uses Win32 API (`GetComputerName`) via P/Invoke to retrieve the hostname
2. Checks if the hostname equals "goltralabs" (case-insensitive)
3. Prints "Hello World" if it matches
4. Prints "This is not the Target System and I Should not Run" if it doesn't match

## Compilation Instructions for Windows 11

### Prerequisites
- Windows 11
- .NET 6.0 SDK or later installed

### Installation of .NET SDK
1. Download from: https://dotnet.microsoft.com/download
2. Run the installer
3. Verify installation:
   ```powershell
   dotnet --version
   ```

### Compilation Steps

#### Method 1: Using PowerShell (Recommended)
1. Open PowerShell
2. Navigate to the project directory:
   ```powershell
   cd "c:\Users\kashi\Documents\BSidesPRG\labAutMal"
   ```
3. Compile the project:
   ```powershell
   dotnet build
   ```
4. Or compile for Release mode (optimized):
   ```powershell
   dotnet build -c Release
   ```

#### Method 2: Using Visual Studio Code
1. Open the folder in VS Code
2. Open integrated terminal (Ctrl + `)
3. Run:
   ```powershell
   dotnet build
   ```

#### Method 3: Using Visual Studio
1. Open Visual Studio
2. File → Open → Folder → Select project folder
3. Right-click on project → Build

### Running the Application

#### After Compilation (Quick Run)
```powershell
dotnet run
```

#### Run Compiled Executable
Navigate to the output directory and run:
```powershell
.\bin\Debug\net6.0\HostnameChecker.exe
```

Or for Release build:
```powershell
.\bin\Release\net6.0\HostnameChecker.exe
```

## Output Examples

**If hostname is "goltralabs":**
```
Hello World
```

**If hostname is something else (e.g., "DESKTOP-12345"):**
```
This is not the Target System and I Should not Run
```

## Changing the Target Hostname

To target a different hostname, edit `HostnameChecker.cs` line 20:
```csharp
if (hostname.Equals("YOUR_HOSTNAME_HERE", StringComparison.OrdinalIgnoreCase))
```

## Technical Details

- **Win32 API Used**: `GetComputerName` from `kernel32.dll`
- **P/Invoke Declaration**: Marshals between managed C# and unmanaged Windows API
- **Max Computer Name Length**: 15 characters (NetBIOS limit)
- **Target Framework**: .NET 6.0 (cross-platform runtime available, though P/Invoke Win32 calls are Windows-specific)

## Troubleshooting

**"dotnet: The term 'dotnet' is not recognized"**
- Install .NET SDK or add it to PATH

**Build fails with errors**
- Ensure all project files are in the same directory
- Check that `.csproj` file is named correctly

**Permission Denied when running .exe**
- Right-click → Properties → General tab → Check "Unblock" checkbox at the bottom
- Or bypass with: `powershell -ExecutionPolicy Bypass -File ".\bin\Debug\net6.0\HostnameChecker.exe"`
