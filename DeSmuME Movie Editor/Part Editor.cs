using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DeSmuMe_Movie_Editor
{
    public partial class Part_Editor : Form
    {
        public Part_Editor()
        {
            InitializeComponent();
        }

        public MovieEditor mov;

        // Zones
        List<MovieZone> zones = new List<MovieZone>();

        // Loading
        private void Part_Editor_Load(object sender, EventArgs e)
        {
        }
        private void Part_Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Delete auto-saved files
            for (int i = 0; i < zoneSaveIDs.Count; i++)
                File.Delete(GetAutoSavePath() + zoneSaveIDs[i] + ".pmf");
        }

        // Get a zone from current movie
        private void btnSetZone_Click(object sender, EventArgs e)
        {
            MovieZone z = new MovieZone();
            z.start = (int)numSetStart.Value;
            z.length = (int)(numSetEnd.Value - numSetStart.Value);
            z.bytes = mov.GetPSave(z.start, z.start + z.length);
            z.name = "Zone " + zones.Count;

            zones.Add(z);
            listZones.Items.Add(z.name + " - length: " + z.length);

            AutoSaveZone(z);
        }

        // Select a zone
        private MovieZone cZone
        { get { return zones[listZones.SelectedIndex]; } }
        private void listZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AutoChange)
                return;
            if (listZones.SelectedIndex == -1)
            { txtNameZone.Text = ""; txtNameZone.Enabled = false; return; }
            AutoChange = true;
            txtNameZone.Text = cZone.name;
            txtNameZone.Enabled = true;
            AutoChange = false;
        }
        // Change a name
        bool AutoChange = false;
        private void txtNameZone_TextChanged(object sender, EventArgs e)
        {
            if (AutoChange || listZones.SelectedIndex == -1)
                return;
            AutoChange = true;
            cZone.name = txtNameZone.Text;
            listZones.Items[listZones.SelectedIndex] = cZone.getStr();
            AutoChange = false;
        }


        List<ZoneAction> actions = new List<ZoneAction>();
        private ZoneAction cAction
        { get { return actions[listActions.SelectedIndex]; } }
        // Place a zone
        private void btnAddZone_Click(object sender, EventArgs e)
        {
            if (listZones.SelectedIndex == -1)
            { MessageBox.Show("Select a zone to place!"); return; }

            ZoneAction a = new ZoneAction();
            a.zone = cZone;
            a.placeAt = (int)numPlaceStart.Value;
            a.replace = chkReplace.Checked;

            actions.Add(a);
            listActions.Items.Add(a.getStr());
        }
        // Change action
        private void numPlaceStart_ValueChanged(object sender, EventArgs e)
        {
            if (listActions.SelectedIndex == -1)
                return;
            cAction.placeAt = (int)numPlaceStart.Value;
            listActions.Items[listActions.SelectedIndex] = cAction.getStr();
        }
        private void chkReplace_CheckedChanged(object sender, EventArgs e)
        {
            if (listActions.SelectedIndex == -1)
                return;
            cAction.replace = chkReplace.Checked;
            listActions.Items[listActions.SelectedIndex] = cAction.getStr();
        }

        // Save/load a zone
        string partDir = Properties.Settings.Default.partDir;
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (listZones.SelectedIndex == -1)
            { MessageBox.Show("Select a zone to save!"); return; }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = partDir;
            dialog.DefaultExt = ".pmf";
            dialog.Filter = "Partial Movie File|*.pmf";
            dialog.AddExtension = true;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                System.IO.File.WriteAllBytes(dialog.FileName, cZone.bytes);
            else
            { MessageBox.Show("Canceled."); return; }

            partDir = Directory.GetParent(dialog.FileName).FullName;
            Properties.Settings.Default.partDir = partDir;
            Properties.Settings.Default.Save();
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = partDir;
            dialog.DefaultExt = ".pmf";
            dialog.Filter = "Partial Movie File|*.pmf";
            dialog.AddExtension = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            if (!System.IO.File.Exists(dialog.FileName))
            { MessageBox.Show("File does not exist."); return; }

            LoadZone(dialog.FileName);

            partDir = Directory.GetParent(dialog.FileName).FullName;
            Properties.Settings.Default.partDir = partDir;
            Properties.Settings.Default.Save();
        }
        private void LoadZone(string path)
        {
            MovieZone z = new MovieZone();
            z.name = new FileInfo(path).Name;
            z.name = z.name.Substring(0, z.name.LastIndexOf('.'));
            z.start = 0;
            z.bytes = System.IO.File.ReadAllBytes(path);
            z.length = BitConverter.ToInt32(z.bytes, 0);

            zones.Add(z);
            listZones.Items.Add(z.getStr());
        }
        // Auto save
        static int autoSaveID = 0;
        List<int> zoneSaveIDs = new List<int>();
        private string GetAutoSavePath()
        {
            DirectoryInfo saveDirectory = new DirectoryInfo(Application.ExecutablePath).Parent;
            saveDirectory = saveDirectory.CreateSubdirectory("autosaves");
            return saveDirectory.FullName + "\\";
        }
        private void AutoSaveZone(MovieZone zone)
        {
            string path = GetAutoSavePath() + autoSaveID + ".pmf";
            zoneSaveIDs.Add(autoSaveID);
            autoSaveID++;
            System.IO.File.WriteAllBytes(path, zone.bytes);
        }
        public void LoadAutoSavedZones()
        {
            DirectoryInfo autoSaveDirectory = new DirectoryInfo(GetAutoSavePath());
            FileInfo[] files = autoSaveDirectory.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.Contains(".pmf"))
                {
                    LoadZone(files[i].FullName);
                    File.Move(files[i].FullName, GetAutoSavePath() + autoSaveID + ".pmf");
                    zoneSaveIDs.Add(autoSaveID);
                    autoSaveID++;
                }
            }
        }

        // Move action up/down list
        private void btnUp_Click(object sender, EventArgs e)
        {
            if (listActions.SelectedIndex < 1)
                return;
            ZoneAction a = cAction;
            actions[listActions.SelectedIndex] = actions[listActions.SelectedIndex - 1];
            actions[listActions.SelectedIndex - 1] = a;
            // Update display
            listActions.Items[listActions.SelectedIndex] = cAction.getStr();
            listActions.Items[listActions.SelectedIndex - 1] = a.getStr();
            listActions.SelectedIndex -= 1;
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (listActions.SelectedIndex > listActions.Items.Count - 2)
                return;
            ZoneAction a = cAction;
            actions[listActions.SelectedIndex] = actions[listActions.SelectedIndex + 1];
            actions[listActions.SelectedIndex + 1] = a;
            // Update display
            listActions.Items[listActions.SelectedIndex] = cAction.getStr();
            listActions.Items[listActions.SelectedIndex + 1] = a.getStr();
            listActions.SelectedIndex += 1;
        }


        // Take action!
        private void btnFinalize_Click(object sender, EventArgs e)
        {
            if (actions.Count == 0)
            { MessageBox.Show("No actions were listed, silly."); return; }
            byte[] undo = mov.GetPSave(0, mov.MovieLength);
            for (int i = 0; i < actions.Count; i++)
            {
                int result = mov.UsePSave(actions[i].zone.bytes, actions[i].placeAt, actions[i].replace);
                if (result == 1)
                {
                    if (MessageBox.Show("Could not complete action #" + i + "\nDo you wish to undo completed actions?", "Undo?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        mov.UsePSave(undo, 0, true);
                    return;
                }
            }
            MessageBox.Show("Actions taken.");
            listActions.Items.Clear();
            actions.Clear();
        }

        private void listZones_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && listZones.SelectedIndex >= 0)
            {
                // Delete auto-saved file
                File.Delete(GetAutoSavePath() + zoneSaveIDs[listZones.SelectedIndex] + ".pmf");
                zoneSaveIDs.RemoveAt(listZones.SelectedIndex);

                // Delete from list
                zones.RemoveAt(listZones.SelectedIndex);
                listZones.Items.RemoveAt(listZones.SelectedIndex);
            }
        }

        // Hacky
        private void Part_Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Shift)
            {
                if (e.KeyCode == Keys.X)
                {
                    for (int i = (int)numSetStart.Value; i < numSetEnd.Value; i++)
                    {
                        mov.MirrorFrame(i);
                    }
                    MessageBox.Show("Mirrored.");
                }
            }
        }

    }

    public class MovieZone
    {
        public string name;

        public int start;
        public int length;
        public byte[] bytes;

        public string getStr()
        {
            return name + " - length: " + length;
        }
    }
    class ZoneAction
    {
        public MovieZone zone;
        public int placeAt;
        public bool replace;

        public string getStr()
        {
            string str = "insert at";
            if (replace) str = "replace at";
            str = zone.name + " (" + zone.length + "): " + str + placeAt;

            return str;
        }
    }
}
