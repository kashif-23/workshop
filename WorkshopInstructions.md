# BSides Prague 2026 Workshop: Autonomous Malware Writing & Analysis

## ⚠️ Important  Notice

**This workshop is for educational purposes only.** Run this code only on systems you own and control, and only if you are authorized to do so. Use the skills learned here exclusively for ethical purposes in offensive or defensive security domains. Unauthorized use may violate laws and ethical standards.

---

## 🎯 Workshop Overview

This hands-on workshop teaches autonomous malware analysis through 4 progressive steps. Each step builds on the previous, introducing Win32 API calls, system checks, file operations, and culminating in PowerShell integration for advanced stealth techniques.

**Prerequisites:**
- Windows 11 (x86_64 architecture)
- .NET 10.0 SDK installed ([download](https://dotnet.microsoft.com/download))
- Folder structure: `C:\Users\[YourUsername]\Documents\BSidesPRG\`
- Files: `FinancialDetails.txt` in the BSidesPRG folder
- Subfolders: `BackupFiles\Step1\` through `BackupFiles\Step4\` in BSidesPRG

**Dependencies to Verify:**
- .NET SDK: Run `dotnet --version` (should show 10.0.x)
- PowerShell: Run `$PSVersionTable.PSVersion` (should show version 5.1+)
- Win32 APIs: Available on all Windows systems
- Network access: For connectivity tests
- Administrative privileges: May be needed for some service checks

---

## 📁 Setup Instructions

1. **Create folder structure:**
   ```
   C:\Users\[YourUsername]\Documents\BSidesPRG\
   ├── BackupFiles\
   │   ├── Step1\
   │   ├── Step2\
   │   ├── Step3\
   │   └── Step4\
   └── FinancialDetails.txt
   ```

2. **Add sample content to FinancialDetails.txt:**
   ```
   Sample financial data for testing purposes.
   This file will be encrypted during the workshop.
   ```

3. **Verify .NET installation:**
   ```powershell
   dotnet --version
   # Should output: 10.0.xxx
   ```

---

## 🚀 Step-by-Step Workshop

### Step 1: Basic Win32 API - Hello World
**Goal:** Learn basic Win32 API calls for hostname retrieval.

**Files:** `HostnameChecker.cs` in `BackupFiles\Step1\`

**What it does:**
- Uses `GetComputerName` Win32 API to get hostname
- Prints "Hello World" if hostname matches "goltralabs"

**Compile & Run:**
```powershell
cd "C:\Users\[YourUsername]\Documents\BSidesPRG\BackupFiles\Step1"
dotnet build HostnameChecker.csproj
dotnet run --project HostnameChecker.csproj
```

**Expected Output:**
```
Hello World
```

---

### Step 2: Conditional Execution & File Creation
**Goal:** Add system checks and create README.txt file.

**Files:** `HostnameChecker.cs` in `BackupFiles\Step2\`

**What it does:**
- Checks hostname match
- Verifies network connectivity (ping/TCP to 1.1.1.1:53)
- Checks Windows Defender status
- Creates README.txt with test results

**New Features:**
- Network connectivity validation
- Service status checking
- Dynamic file creation

**Compile & Run:**
```powershell
cd "C:\Users\[YourUsername]\Documents\BSidesPRG\BackupFiles\Step2"
dotnet build HostnameChecker.csproj
dotnet run --project HostnameChecker.csproj
```

**Expected Output:**
```
Hostname: [YourHostname]
Hostname matches target: True/False
Ping or TCP port 53 reachable on 1.1.1.1: True/False
Windows Defender service running: True/False
Created backup file at [Path]\README.txt
```

---

### Step 3: User Activity & File Encryption
**Goal:** Detect user activity and encrypt sensitive files.

**Files:** `HostnameChecker.cs` in `BackupFiles\Step3\`

**What it does:**
- All Step 2 checks plus user activity detection
- Checks mouse/keyboard input in last 10 seconds
- Encrypts FinancialDetails.txt using XOR with hostname as key
- Calculates and displays MD5 hash
- Creates encrypted backup

**New Features:**
- `GetLastInputInfo` Win32 API for activity detection
- XOR encryption algorithm
- MD5 hashing for integrity
- Dynamic username resolution (no hardcoded paths)

**Compile & Run:**
```powershell
cd "C:\Users\[YourUsername]\Documents\BSidesPRG\BackupFiles\Step3"
dotnet build HostnameChecker.csproj
dotnet run --project HostnameChecker.csproj
```

**Expected Output:**
```
[DEBUG] Current Username: [YourUsername]
Computer in use (last 10 seconds): True/False
MD5 hash of FinancialDetails.txt: [hash]
Created encrypted backup at [Path]\FinancialDetails.enc
```

---

### Step 4: Smart Dormant Execution with PowerShell Integration
**Goal:** Implement intelligent execution logic with PowerShell-based encryption for advanced stealth operations.

**Files:** `HostnameChecker.cs` in `BackupFiles\Step4\`

**What it does:**
- All previous checks (hostname, network, defender, user activity)
- **Smart logic:** Only executes encryption if ALL conditions met:
  - Defender service OFF
  - Computer idle (no input for 10+ seconds)
  - Network reachable
  - **NEW:** Lateral host 192.168.1.140 connectivity check
- **PowerShell Integration:** Instead of direct C# encryption, creates and executes a PowerShell script for:
  - MD5 hash calculation
  - XOR encryption with hostname as key
  - File backup creation
- If conditions not met, stays dormant and suggests lateral movement

**New Features:**
- Conditional execution based on security posture
- PowerShell script generation and execution
- Separation of encryption logic from main program
- Enhanced stealth through external script execution
- Dynamic username resolution via Win32 API
- Dormant behavior simulation with lateral movement suggestion

**Compile & Run:**
```powershell
cd "C:\Users\[YourUsername]\Documents\BSidesPRG\BackupFiles\Step4"
dotnet build HostnameChecker.csproj
dotnet run --project HostnameChecker.csproj
```

**Expected Output (Active):**
```
[DEBUG] Current Username: [YourUsername]
[DEBUG] User Profile Path: C:\Users\[YourUsername]
Hostname: [YourHostname]
Hostname matches target: True
Ping or TCP port 53 reachable on 1.1.1.1: True
Windows Defender service running: False
Computer in use (last 10 seconds): False
MD5 hash of FinancialDetails.txt: [hash]
Created encrypted backup at [Path]\FinancialDetails.enc
PowerShell script created at [Path]\EncryptFile.ps1
Created backup file at [Path]\README.txt
```

**Expected Output (Dormant):**
```
Hostname: [YourHostname]
Hostname matches target: True
Ping or TCP port 53 reachable on 1.1.1.1: True
Windows Defender service running: True
Computer in use (last 10 seconds): True

Conditions not appropriate for execution, continue to stay dormant and copy to the next host 192.168.1.140 over SMB to attempt there.
```

---

## 🔧 Troubleshooting

**Build Errors:**
- Ensure .NET 10.0 SDK is installed
- Check file paths match your username
- Verify all using statements are present

**Runtime Errors:**
- Run as administrator if service checks fail
- Ensure FinancialDetails.txt exists
- Check network connectivity for tests
- **PowerShell execution fails:** Ensure PowerShell execution policy allows script running (`Set-ExecutionPolicy RemoteSigned`)
- **PowerShell script not found:** Check that the script was created in the BackupFiles directory

**Win32 API Issues:**
- Only works on Windows systems
- May require elevated privileges for some calls

---

## 🎓 Learning Objectives

By the end of this workshop, you'll understand:
- Win32 API integration in C# for system reconnaissance
- PowerShell script generation and execution from C#
- System reconnaissance techniques (network, services, user activity)
- File encryption algorithms (XOR) and integrity checking (MD5)
- Conditional execution logic for stealth operations
- Dynamic path resolution without hardcoded usernames
- Separation of concerns between main program and utility scripts

**Remember:** These techniques are powerful tools for security research. Use them responsibly! 🔒