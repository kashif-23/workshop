/*
 * ==========================================
 * Author:  Kashif Amanat
 * Event:   BSides Prague 2026
 * Contact: kashif.amanat1@gmail.com
 * ==========================================
 */

using System;
using System.Runtime.InteropServices;
using System.Text;

class HostnameChecker
{
    // Win32 API declaration to get the computer name
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GetComputerName(StringBuilder lpBuffer, ref uint nSize);

    static void Main()
    {
        try
        {
            // Get the hostname using Win32 API
            string hostname = GetHostname();
            
            // Check if the hostname matches the target system
            if (hostname.Equals("goltralabs", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Hello World");
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
