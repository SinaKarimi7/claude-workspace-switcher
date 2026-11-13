using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
class Switcher : Form {
    static ProcessStartInfo Workspace(bool work) {
        dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));
        dynamic app = shell.NameSpace("shell:AppsFolder").ParseName("Claude_pzs8sxrjxfjjc!Claude");
        string root = app.ExtendedProperty("System.AppUserModel.PackageInstallPath");
        string exe = Path.Combine(root, @"app\Claude.exe");
        if (!File.Exists(exe)) throw new FileNotFoundException("Could not locate Claude Desktop.", exe);
        string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Claude-Work");
        return new ProcessStartInfo(exe, work ? "--user-data-dir=\"" + folder + "\"" : "") { UseShellExecute = false };
    }
    Switcher() {
        Text = "Claude Workspace Switcher";
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(306, 130);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 11);
        AddWorkspace("Work workspace", false, 14);
        AddWorkspace("Original workspace", true, 71);
        ResumeLayout(true);
    }
    void AddWorkspace(string label, bool work, int top) {
        Button button = new Button { Text = label, Left = 14, Top = top, Width = 278, Height = 44 };
        button.Click += delegate {
            try { Process.Start(Workspace(work)); }
            catch (Exception e) { MessageBox.Show(this, e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        };
        Controls.Add(button);
    }
    [STAThread] static int Main(string[] args) {
        if (args.Length == 1 && args[0] == "--check") {
            var original = Workspace(false);
            var work = Workspace(true);
            return original.Arguments == "" && work.Arguments.EndsWith("Claude-Work\"") && original.FileName == work.FileName ? 0 : 1;
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Switcher());
        return 0;
    }
}


