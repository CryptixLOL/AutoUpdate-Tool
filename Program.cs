// Was made by Cryptix originally for my executor for roblox
// Dont remove these credits!

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

class Program
{
    static string GetUserDesktopPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    }

    static string localVersionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version.txt");
    static string serverVersionUrl = "http://yourdomain/version.txt";
    static string zipDownloadUrl = "http://yourdomain/Your%20File.zip";
    static string desktopPath = GetUserDesktopPath();
    static string zipPath = Path.Combine(desktopPath, "Your File.zip");
    static string extractPath = Path.Combine(desktopPath, "Your File");

    [STAThread]
    static void Main()
    {
        try
        {
            if (!File.Exists(localVersionFile))
            {
                MessageBox.Show("version.txt not found with Updater.exe.", "❌File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string localVersion = File.ReadAllText(localVersionFile).Trim();

            string serverVersion;
            using (WebClient client = new WebClient())
            {
                serverVersion = client.DownloadString(serverVersionUrl).Trim();
            }

            if (localVersion == serverVersion)
            {
                // no update found so it runs executor like normal  and say that msg no update
                MessageBox.Show("Your already using the latest version!", "✅Up to Date", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string executorPath = Path.Combine(extractPath, "Cryptix Executor.exe");
                if (File.Exists(executorPath))
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = executorPath;
                    psi.UseShellExecute = true;
                    psi.Verb = "runas";
                    Process.Start(psi);
                }
                return;
            }

            foreach (var proc in Process.GetProcessesByName("Cryptix Executor"))
            {
                try
                {
                    proc.Kill();
                    proc.WaitForExit();
                }
                catch {}
            }

            // download the new zip from server
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(zipDownloadUrl, zipPath);
            }

            // Delete old foldr if exist
            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);

            // extract my cryptix executor to desktop folder
            ZipFile.ExtractToDirectory(zipPath, extractPath);
            File.Delete(zipPath);

            MessageBox.Show("Update complete!\n\nPlease run the new executor from your Desktop folder.", "Update Complete", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Update failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
