using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

// Portable launcher for the OpenClaude DCS translator.
// Starts dashboard/server.mjs with the bundled Node.js runtime if the
// dashboard is not already listening, then opens the DCS tab in a browser.
internal static class Program
{
    private const int Port = 3000;

    [STAThread]
    private static int Main()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
        string node = Path.Combine(baseDir, "engine", "node-win-x64", "node.exe");
        string server = Path.Combine(baseDir, "dashboard", "server.mjs");

        if (!File.Exists(node))
        {
            MessageBox.Show("Node.js introuvable :\n" + node + "\n\nLancez START.bat une premiere fois pour installer l'environnement.",
                "OpenClaude DCS Traducteur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }
        if (!File.Exists(server))
        {
            MessageBox.Show("Serveur du dashboard introuvable :\n" + server,
                "OpenClaude DCS Traducteur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }

        string translatorJar = Path.Combine(baseDir, "tools", "miz-translator", "miz-translator", "lib", "miz-translator-fat.jar");
        if (!File.Exists(translatorJar))
        {
            string installer = Path.Combine(baseDir, "tools", "install_miz_translator.bat");
            DialogResult answer = MessageBox.Show(
                "Le moteur de traduction DCS (miz-translator) n'est pas installe.\n\n" +
                "Le telecharger maintenant (~150 Mo) depuis GitHub ?\n\n" +
                "Fichier attendu :\n" + translatorJar,
                "OpenClaude DCS Traducteur", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer == DialogResult.Yes)
            {
                if (File.Exists(installer))
                {
                    ProcessStartInfo psiInstaller = new ProcessStartInfo(installer);
                    psiInstaller.UseShellExecute = true;
                    Process.Start(psiInstaller);
                }
                else
                {
                    MessageBox.Show("Installateur introuvable :\n" + installer,
                        "OpenClaude DCS Traducteur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return 0;
        }

        if (!PortOpen(3000))
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(node, "\"" + server + "\"");
                psi.WorkingDirectory = baseDir;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psi);

                for (int i = 0; i < 60 && !PortOpen(Port); i++) Thread.Sleep(250);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible de demarrer le dashboard :\n" + ex.Message,
                    "OpenClaude DCS Traducteur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
        }

        try
        {
            ProcessStartInfo open = new ProcessStartInfo("http://localhost:" + Port + "/#dcs");
            open.UseShellExecute = true;
            Process.Start(open);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Impossible d'ouvrir le navigateur :\n" + ex.Message,
                "OpenClaude DCS Traducteur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }
        return 0;
    }

    private static bool PortOpen(int port)
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                System.Threading.Tasks.Task task = client.ConnectAsync("127.0.0.1", port);
                return task.Wait(300) && client.Connected;
            }
        }
        catch
        {
            return false;
        }
    }
}
