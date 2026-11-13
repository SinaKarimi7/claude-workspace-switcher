using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Win32;

class Switcher : Form {
    const string Key = @"Software\Claude Workspace Switcher";
    XElement profiles;
    readonly FlowLayoutPanel list = new FlowLayoutPanel();
    static XElement Defaults() {
        return new XElement("profiles", new XAttribute("main", "default"),
            Entry("default", "Work workspace"), Entry("Claude-Work", "Original workspace"));
    }
    static XElement Entry(string id, string name) {
        return new XElement("profile", new XAttribute("id", id), new XAttribute("name", name));
    }
    static ProcessStartInfo Workspace(string id) {
        dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));
        dynamic app = shell.NameSpace("shell:AppsFolder").ParseName("Claude_pzs8sxrjxfjjc!Claude");
        string root = app.ExtendedProperty("System.AppUserModel.PackageInstallPath");
        string exe = Path.Combine(root, @"app\Claude.exe");
        if (!File.Exists(exe)) throw new FileNotFoundException("Could not locate Claude Desktop.", exe);
        if (id == "default") return new ProcessStartInfo(exe) { UseShellExecute = false };
        string folder = id == "Claude-Work" ? id : Path.Combine("ClaudeWorkspaceSwitcher", "Profiles", Guid.ParseExact(id,"N").ToString("N"));
        folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), folder);
        return new ProcessStartInfo(exe, "--user-data-dir=\"" + folder + "\"") { UseShellExecute = false };
    }
    Switcher() {
        string saved = Registry.GetValue(@"HKEY_CURRENT_USER\" + Key, "Profiles", null) as string;
        profiles = saved == null ? Defaults() : XElement.Parse(saved);
        Text = "Claude Workspace Switcher";
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        Font = new Font("Segoe UI", 10);
        AutoScaleDimensions = new SizeF(96,96);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(360,230);
        StartPosition = FormStartPosition.CenterScreen;
        list.Dock = DockStyle.Fill;
        list.AutoScroll = true;
        list.Padding = new Padding(8);
        list.FlowDirection = FlowDirection.TopDown;
        list.WrapContents = false;
        Controls.Add(list);
        Button add = new Button { Text = "Add profile", Dock = DockStyle.Bottom, Height = 36 };
        add.Click += delegate { Edit(null); };
        Controls.Add(add);
        Render();
    }
    void Save(XElement next) {
        try {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(Key))
                key.SetValue("Profiles", next.ToString(SaveOptions.DisableFormatting));
            profiles = next;
            Render();
        } catch (Exception ex) { MessageBox.Show(this, ex.Message, Text); }
    }
    void Render() {
        while(list.Controls.Count > 0) list.Controls[0].Dispose();
        foreach (var entry in profiles.Elements().OrderByDescending(e => (string)e.Attribute("id") == (string)profiles.Attribute("main"))) {
            string id = (string)entry.Attribute("id");
            bool main = id == (string)profiles.Attribute("main");
            var row = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
            var open = new Button { Width = 245, Height = 38, AutoEllipsis = true, Text = (string)entry.Attribute("name") + (main ? "  (main)" : "") };
            if (main) AcceptButton = open;
            open.Click += delegate { try { Process.Start(Workspace(id)); } catch(Exception ex) { MessageBox.Show(this, ex.Message, Text); } };
            var edit = new Button { Text = "Edit", Height = 38, Width = 58 };
            edit.Click += delegate { Edit(id); };
            row.Controls.Add(open);
            row.Controls.Add(edit);
            list.Controls.Add(row);
        }
    }
    void Edit(string id) {
        var next = new XElement(profiles);
        var entry = next.Elements().FirstOrDefault(e => (string)e.Attribute("id") == id);
        using (var dialog = new Form { Text = entry == null ? "Add profile" : "Edit profile", Font = Font, ClientSize = new Size(320,150), FormBorderStyle = FormBorderStyle.FixedDialog, MinimizeBox = false, MaximizeBox = false, StartPosition = FormStartPosition.CenterParent }) {
            var label = new Label { Text = "Profile name", Left = 12, Top = 8, AutoSize = true };
            var name = new TextBox { Left = 12, Top = 32, Width = 294, MaxLength = 80, Text = entry == null ? "" : (string)entry.Attribute("name") };
            bool isMain = id == (string)next.Attribute("main");
            var main = new CheckBox { Text = "Main profile", Left = 12, Top = 65, Checked = isMain, Enabled = !isMain, AutoSize = true };
            var save = new Button { Text = "Save", Left = 228, Top = 105 };
            var remove = new Button { Text = "Remove", Left = 12, Top = 105, Visible = entry != null, Enabled = next.Elements().Count() > 1 };
            save.Click += delegate {
                string value = name.Text.Trim();
                if (value.Length == 0 || next.Elements().Any(e => e != entry && String.Equals((string)e.Attribute("name"),value,StringComparison.OrdinalIgnoreCase))) {
                    MessageBox.Show(dialog,"Enter a unique profile name."); return;
                }
                if (entry == null) { id = Guid.NewGuid().ToString("N"); next.Add(Entry(id,value)); }
                else entry.SetAttributeValue("name", value);
                if (main.Checked) next.SetAttributeValue("main", id);
                dialog.DialogResult = DialogResult.OK;
            };
            remove.Click += delegate {
                if (MessageBox.Show(dialog,"Remove this launcher entry? Its Claude data stays on disk.","Remove profile",MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                entry.Remove();
                if (isMain) next.SetAttributeValue("main",(string)next.Elements().First().Attribute("id"));
                dialog.DialogResult = DialogResult.OK;
            };
            dialog.Controls.AddRange(new Control[]{label,name,main,save,remove});
            dialog.AcceptButton = save;
            dialog.ActiveControl = name;
            if (dialog.ShowDialog(this) == DialogResult.OK) Save(next);
        }
    }
    [STAThread] static int Main(string[] args) {
        if (args.Length == 1 && args[0] == "--check")
            return Workspace("default").Arguments == "" && Workspace("Claude-Work").Arguments.EndsWith("Claude-Work\"") && Workspace(Guid.NewGuid().ToString("N")).Arguments.Contains("Profiles") ? 0 : 1;
        bool first;
        using (var guard = new Mutex(true, @"Local\ClaudeWorkspaceSwitcher-" + System.Security.Principal.WindowsIdentity.GetCurrent().User.Value, out first)) {
            if (!first) return 0;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Switcher());
        }
        return 0;
    }
}
