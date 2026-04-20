/*
 * ==========================================
 * Author:  Kashif Amanat
 * Event:   BSides Prague 2026
 * Contact: kashif.amanat1@gmail.com
 * ==========================================
 */

using System;
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

    // Dynamically resolved path (set in Main)
    private static string BackupFilePath = "";

    // Win32 API declarations
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GetComputerName(StringBuilder lpBuffer, ref uint nSize);

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
            // Initialize dynamic path based on current username
            string userProfile = GetUserProfilePath();
            BackupFilePath = Path.Combine(userProfile, "Documents", "BSidesPRG", "BackupFiles", "README.txt");

            Console.WriteLine($"[DEBUG] Current Username: {GetCurrentUsername()}");
            Console.WriteLine($"[DEBUG] User Profile Path: {userProfile}");
            Console.WriteLine($"[DEBUG] Backup Location: {BackupFilePath}\n");

            string hostname = GetHostname();
            bool hostnameMatches = hostname.Equals(TargetHostname, StringComparison.OrdinalIgnoreCase);
            bool networkCheck = CheckIpReachable(TargetIp, TargetPort, NetworkTimeoutMs);
            bool defenderRunning = CheckWindowsDefenderRunning();

            Console.WriteLine($"Hostname: {hostname}");
            Console.WriteLine($"Hostname matches target: {hostnameMatches}");
            Console.WriteLine($"Ping or TCP port {TargetPort} reachable on {TargetIp}: {networkCheck}");
            Console.WriteLine($"Windows Defender service running: {defenderRunning}");

            if (hostnameMatches)
            {
                CreateBackupReadme();
                Console.WriteLine($"Created backup file at {BackupFilePath}");
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

    private static void CreateBackupReadme()
    {
        string directoryPath = Path.GetDirectoryName(BackupFilePath) ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(BackupFilePath, "The test executed successfully.\r\nHostname matched target system and checks completed.");
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
