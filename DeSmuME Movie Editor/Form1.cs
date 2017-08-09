using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace DeSmuMe_Movie_Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mov != null)
                mov.Dispose();
        }

        bool AutoChange = true;
        MovieEditor mov = null;

        private FrameInput cFrame
        { get { return mov.getInput((int)numViewFrame.Value); } }
        private int ViewFrame
        { get { return (int)numViewFrame.Value; } }

        // New interface
        CheckBox[] checks;
        private void Form1_Load(object sender, EventArgs e)
        {
            checks = new CheckBox[] { chkg, chkR, chkL, chkX, chkY, chkA, chkB, chkStart, chkSelect, chkUp, chkDown, chkLeft, chkRight };
            lblDesync.Text = "";
        }
        // Load Movie
        private void btnLoadMovie_Click(object sender, EventArgs e)
        {
            // Get the movie!
            bool firstFind = true;
            if (mov != null)
            {
                firstFind = false;
                mov.Dispose();
            }

            mov = new MovieEditor();
            if (mov.GetMovie(rdoVer9.Checked ? 9 : 432, (int)numInst.Value - 1) != 0)
                return;

            mov.DesyncDetected += DesyncDetect;
            mov.RerecordIncremented += RerecordInc;
            mov.FrameEdited += (f, c) => { shouldAutoSave = true; };
            if (firstFind)
            {
                Timer autoSaver = new Timer();
                autoSaver.Interval = 600000; // 10 mins
                autoSaver.Tick += (s, te) => { AutoSave(); };
                autoSaver.Start();
            }

            // Enable interface.
            numViewFrame.Enabled = true;
            btnCurrent.Enabled = true;
            btnSave.Enabled = true;
            btnInsert.Enabled = true;
            btnDelete.Enabled = true;
            btnSetNextTo.Enabled = true;
            btnPartialSave.Enabled = true;
            lblMovieInfo.Visible = true;

            // Reset display
            AutoChange = true;
            numViewFrame.Value = 0;
            AutoChange = false;
            UpdateDisplay();

            // Hide video find things
            lblInst.Visible = false;
            numInst.Visible = false;
            rdoVerHD.Visible = false;
            rdoVer9.Visible = false;
            btnLoadMovie.Text = "Refind";

            // Check for autosaved movie
            if (firstFind)
            {
                bool loadedMovie = false;
                if (File.Exists(GetAutoSaveMoviePath()))
                {
                    if (MessageBox.Show("The last movie was not saved. Do you wish to load the auto-saved version?", "Load auto-save?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        byte[] data = File.ReadAllBytes(GetAutoSaveMoviePath());
                        mov.reRecords = BitConverter.ToInt32(data, 0);
                        byte[] inputData = new byte[data.Length - 4];
                        Array.Copy(data, 4, inputData, 0, inputData.Length);

                        string loadingStr = "Loading autosave...";
                        lblDesync.Text = loadingStr;
                        Refresh();
                        mov.deleteFrames(0, mov.MovieLength - 1);
                        mov.UsePSave(inputData, 0, true);
                        if (lblDesync.Text == loadingStr)
                            lblDesync.Text = "";
                        loadedMovie = true;
                    }
                }

                // Are there partials to load?
                DirectoryInfo autoSaveDirectory = new DirectoryInfo(GetAutoSaveMoviePath()).Parent;
                if (autoSaveDirectory.GetFiles().Length > 1)
                {
                    if (loadedMovie || MessageBox.Show("DeSmuME movie editor did not shut down properly last time. Do you wish to load the auto-saved partials?", "Load auto-save?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Part_Editor p = new Part_Editor();
                        p.mov = mov;
                        p.LoadAutoSavedZones();
                        p.Show();
                    }
                }
            }
        }
        // Change view frame
        private void numViewFrame_ValueChanged(object sender, EventArgs e)
        {
            justClicked = false;
            if (AutoChange) return;
            AutoChange = true;

            // Max value
            if (numViewFrame.Value >= mov.MovieLength)
                numViewFrame.Value = mov.MovieLength - 1;

            UpdateDisplay();

            // lblAddress.Text = Convert.ToString(mov.memoryStart + (int)numFrame.Value * 12, 16);
            AutoChange = false;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (ActiveControl is NumericUpDown)
                return;

            if (e.KeyCode == Keys.Up)
                numViewFrame.Value++;
            else if (e.KeyCode == Keys.Down)
                numViewFrame.Value--;
        }
        // Go to the current frame
        private void btnCurrentFrame_Click(object sender, EventArgs e)
        {
            numViewFrame.Value = mov.CurrentFrame;

            //// TEMP RAPID INPUT
            //for (int i = mov.CurrentFrame; i < mov.CurrentFrame + 1000; i += 2)
            //{
            //    FrameInput setTo = mov.getInput(mov.CurrentFrame);
            //    setTo.upButton = true;
            //    mov.setInput(i, setTo);
            //}
        }
        // new updateDisplay
        private void UpdateDisplay()
        {
            lblMovieInfo.Text = "Movie Length: " + mov.MovieLength;
            lblViewFrame.Text = "Holding: ";
            FrameInput f = cFrame;
            for (int i = 12; i > -1; i--)
            {
                if (f.ButtonDown(i))
                {
                    lblViewFrame.Text += FrameInput.buttonNames[12 - i];
                    checks[i].Checked = true;
                }
                else
                {
                    lblViewFrame.Text += " ";
                    checks[i].Checked = false;
                }
            }
            if (lblViewFrame.Text == "Holding:              ") lblViewFrame.Text = "Holding: Nothing";
            numTouchX.Value = f.touchX;
            numTouchY.Value = f.touchY;
            numTouchP.Value = f.touchP;

            lblReRecords.Text = "Re-Records: " + mov.reRecords;
        }

        // Mov events
        private void DesyncDetect(int frame)
        {
            if (lblDesync.InvokeRequired)
            {
                lblDesync.Invoke(new MovieEditor.EvDel(DesyncDetect), new object[] { frame });
                return;
            }

            if (frame == -1)
                lblDesync.Text = "";
            else
                lblDesync.Text = "Desync possible at frame " + frame;
        }
        private void RerecordInc()
        {
            if (lblReRecords.InvokeRequired)
            {
                lblReRecords.Invoke(new MovieEditor.EvDel3(RerecordInc));
                return;
            }

            lblReRecords.Text = "Re-Records: " + mov.reRecords;
        }

        // change input
        private void chkAnyButton_CheckChanged(object sender, EventArgs e)
        {
            if (AutoChange)
                return;

            FrameInput f = mov.getInput((int)numViewFrame.Value);
            for (int i = 0; i < checks.Length; i++)
                f.setButton(i, checks[i].Checked);
            mov.setInput((int)numViewFrame.Value, f);
        }
        private void numTouch_ValueChanged(object sender, EventArgs e)
        {
            if (AutoChange) { return; }
            int ID = ViewFrame;

            FrameInput f = mov.getInput(ID);
            f.touchX = (byte)numTouchX.Value;
            f.touchY = (byte)numTouchY.Value;
            f.touchP = (int)numTouchP.Value;
            mov.setInput(ID, f);
        }

        // Insert/Delete
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int toDelete = 1;
            if (chk10.Checked)
                toDelete = 10;

            if (ViewFrame + toDelete >= mov.MovieLength)
            {
                MessageBox.Show("Cannot delete frames beyond the length of the movie.");
            }
            else
                mov.deleteFrames(ViewFrame, toDelete);

            UpdateDisplay();
        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
            int toInsert = 1;
            if (chk10.Checked)
                toInsert = 10;

            mov.insertFrames(ViewFrame, toInsert, chkCopy.Checked);

            UpdateDisplay();
        }

        // Multi-change
        private void lblOnlyChange_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Double-click a check box (or 'Touch Screen:') to toggle. Red means don't change.");
        }
        bool justClicked = false;
        private void chkAnyCheckBox_DoubleClick(object sender, MouseEventArgs e)
        {
            if (!justClicked)
            {
                justClicked = true;
                return;
            }

            justClicked = true;
            if (e.Clicks != 2) return;

            Control s = (Control)sender;

            if (s.ForeColor == Color.Red)
                s.ForeColor = Color.Black;
            else
                s.ForeColor = Color.Red;
        }
        private void btnSetNextTo_Click(object sender, EventArgs e)
        {
            int startAt = (int)numViewFrame.Value;
            // Get an array of the buttons to change.
            int[] change = new int[13];
            int ci = 0;
            for (int i = 0; i < 13; i++)
            {
                if (checks[i].ForeColor == Color.Black)
                {
                    change[ci] = i;
                    ci += 1;
                }
            }
            Array.Resize(ref change, ci);
            // Touch change?
            bool touch = (lblTouch.ForeColor == Color.Black);
            // Make sure number to change is not too high/low
            int lastnext = 1;
            int toChange = (int)numNextLast.Value;
            if (rdoLast.Checked)
            {
                toChange = -toChange;
                lastnext = -1;
            }
            if (startAt + toChange < 0)
                toChange -= (startAt + toChange);
            if (startAt + toChange >= mov.MovieLength)
                toChange -= (startAt + toChange - mov.MovieLength - 1);
            // Set next/last frames
            for (int i = 0; i < (int)numNextLast.Value; i++)
            {
                startAt += lastnext;
                FrameInput f = mov.getInput(startAt);
                for (int ic = 0; ic < change.Length; ic++)
                    f.setButton(change[ic], checks[change[ic]].Checked);
                if (touch)
                { f.touchP = cFrame.touchP; f.touchX = cFrame.touchX; f.touchY = cFrame.touchY; }
                mov.setInput(startAt, f);
            }
        }

        // Save!
        private string saveDir = DeSmuMe_Movie_Editor.Properties.Settings.Default.saveDir;
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = saveDir;
            dialog.DefaultExt = ".dsm";
            dialog.Filter = "Movie File |*.dsm";
            dialog.AddExtension = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            { MessageBox.Show("Save canceled."); return; }
            saveDir = Directory.GetParent(dialog.FileName).FullName;
            Properties.Settings.Default.saveDir = saveDir;
            Properties.Settings.Default.Save();

            // If the file does not already exist, get start-up info from another.
            bool ReplaceReRecords = false;
            if (!File.Exists(dialog.FileName))
            {
                MessageBox.Show("The file name you gave does not exist. Please select a file to copy start-up info from. \n (Start date, start SRAM, etc.)");
                OpenFileDialog oDialog = new OpenFileDialog();
                oDialog.InitialDirectory = dialog.FileName;
                oDialog.DefaultExt = ".dsm";
                oDialog.AddExtension = true;
                oDialog.ShowDialog();
                if (!File.Exists(oDialog.FileName))
                {
                    MessageBox.Show("Error: File does not exist. Save canceled.");
                    return;
                }
                // Copy file
                FileStream copyStream = new FileStream(oDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader copyReader = new StreamReader(copyStream);
                string fileData = copyReader.ReadToEnd();
                copyStream.Close();
                copyStream = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter copyWriter = new StreamWriter(copyStream);
                copyWriter.Write(fileData);
                copyWriter.Flush();
                copyStream.Close();
                //File.Copy(oDialog.FileName, dialog.FileName);
                
                if (MessageBox.Show("Do you want to keep (and add to) the source file's re-record count?", "Overwrite?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    ReplaceReRecords = true;
            }

            // Replace input information.
            FileStream fileStream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader fileReader = new StreamReader(fileStream);
            string[] startInfo = fileReader.ReadToEnd().Split('\n');
            fileStream.Close();

            int inputAt = 0;
            do
            {
                inputAt += 1;
                if (startInfo[inputAt].StartsWith("rerecordCount"))
                {
                    int rr = mov.reRecords;
                    if (!ReplaceReRecords)
                        rr += Convert.ToInt32(startInfo[inputAt].Substring("rerecordCount ".Length));
                    startInfo[inputAt] = "rerecordCount " + rr;
                    mov.reRecords = 0;
                }
            } while (!startInfo[inputAt].StartsWith("|"));
            Array.Resize(ref startInfo, inputAt + 1);

            startInfo[startInfo.Length - 1] = mov.GenerateSaveString();

            fileStream = new FileStream(dialog.FileName, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter fileWriter = new StreamWriter(fileStream);
            fileWriter.Write(string.Join("\n", startInfo));
            fileWriter.Flush();
            fileStream.Close();

            // Delete the autosaved movie
            File.Delete(GetAutoSaveMoviePath());

            MessageBox.Show("Saved!");
        }
        private bool shouldAutoSave = false;
        private void AutoSave()
        {
            if (shouldAutoSave)
            {
                shouldAutoSave = false;
                byte[] inputBytes = mov.GetPSave(0, mov.MovieLength);
                byte[] saveBytes = new byte[inputBytes.Length + 4];
                BitConverter.GetBytes(mov.reRecords).CopyTo(saveBytes, 0);
                inputBytes.CopyTo(saveBytes, 4);
                File.WriteAllBytes(GetAutoSaveMoviePath(), saveBytes);
            }
        }
        private string GetAutoSaveMoviePath()
        {
            DirectoryInfo saveDirectory = new DirectoryInfo(Application.ExecutablePath).Parent;
            saveDirectory = saveDirectory.CreateSubdirectory("autosaves");
            return saveDirectory.FullName + "\\movie";
        }

        // What button!?
        private void numButtonID_ValueChanged(object sender, EventArgs e)
        {
            FrameInput f = mov.getInput(mov.CurrentFrame);
            AutoChange = false;
            chkButton.Checked = f.ButtonDown((int)numButtonID.Value);
            AutoChange = true;
        }
        private void chkButton_CheckedChanged(object sender, EventArgs e)
        {
            FrameInput f = mov.getInput(mov.CurrentFrame);
            f.setButton((int)numButtonID.Value, chkButton.Checked);
            mov.setInput(mov.CurrentFrame, f);
        }

        private void btnPartialSave_Click(object sender, EventArgs e)
        {
            //btnTempLoad_Click(sender, e);
            Part_Editor pe = new Part_Editor();
            pe.mov = mov;
            pe.Show();
        }

        // Change frame
        private void btnSetCurrent_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to change the current frame?", "sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                mov.CurrentFrame = (int)numViewFrame.Value;
            }
        }

        private void chk10_CheckedChanged(object sender, EventArgs e)
        {
            if (chk10.Checked)
                chk10.ForeColor = Color.Red;
            else
                chk10.ForeColor = Color.Black;
        }

        // Help?
        private void helpBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DeSmuMe Movie Editor, by Suuper.\nVersion C (9 Aug, 2017)\n\n" +
             "Features to be added later:" +
             "\n-LID and mic are not currently supported." +
             "\n-Rapid-fire input is not currently supported." +
             "\n\nKnown bugs/problems:" +
             "\nDesync detection cannot detect loading a desynced savestate. (This is probably the easiest way to get one, so be careful.)" +
             "\nChanging a frame and then changing it back before playing still counts as a rerecord." +
             "\n\nIf you have any questions, problems or suggestions, let me know at SuuperW@gmail.com");

            MessageBox.Show("This program's main purpose is to aid in creating tool-assisted speedruns. To use this program:" +
                "\nOpen DeSmuMe and play a movie as 'Read Only'." +
                "\nSelect your DeSmuMe version. If you have multiple open, set 'Instance' to 1 for the 1st one opened, 2 for the 2nd, etc. Then click 'Get Movie'." +
                "\nYou can now select any frame of the movie and change it with the check boxes (for buttons) and number boxes (for touch screen)." +
                "\nUse savestates, pause/unpause, and frame advance in DeSmuMe to watch your edited movie. No other input in DeSmuMe is required." +
                "\n\nThe 'Partials' button will open a new window which allows you to save/load portions of a movie, and place them in the currently playing movie." +
                "\nSet the starting point (inclusive) and ending point (exclusive) of a 'Zone' to get the input between those frames." +
                "\nOnce created, the 'zones' in the list do NOT change. Editing the input between the start/end points you specified will not change the input in the created zone." +
                "\nThese zones do not provide desync warnings, so be careful!");
        }

        private void Control_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (ActiveControl is CheckBox || ActiveControl is Button)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    e.IsInputKey = true;
                }
            }
        }


    }
}
