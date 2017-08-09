namespace DeSmuMe_Movie_Editor
{
    partial class Part_Editor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listZones = new System.Windows.Forms.ListBox();
            this.btnSetZone = new System.Windows.Forms.Button();
            this.numSetStart = new System.Windows.Forms.NumericUpDown();
            this.btnAddZone = new System.Windows.Forms.Button();
            this.numSetEnd = new System.Windows.Forms.NumericUpDown();
            this.numPlaceStart = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listActions = new System.Windows.Forms.ListBox();
            this.chkReplace = new System.Windows.Forms.CheckBox();
            this.btnFinalize = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtNameZone = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numSetStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlaceStart)).BeginInit();
            this.SuspendLayout();
            // 
            // listZones
            // 
            this.listZones.FormattingEnabled = true;
            this.listZones.Location = new System.Drawing.Point(12, 51);
            this.listZones.Name = "listZones";
            this.listZones.Size = new System.Drawing.Size(276, 108);
            this.listZones.TabIndex = 0;
            this.listZones.SelectedIndexChanged += new System.EventHandler(this.listZones_SelectedIndexChanged);
            this.listZones.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listZones_KeyDown);
            // 
            // btnSetZone
            // 
            this.btnSetZone.Location = new System.Drawing.Point(153, 22);
            this.btnSetZone.Name = "btnSetZone";
            this.btnSetZone.Size = new System.Drawing.Size(75, 23);
            this.btnSetZone.TabIndex = 1;
            this.btnSetZone.Text = "Set Zone";
            this.btnSetZone.UseVisualStyleBackColor = true;
            this.btnSetZone.Click += new System.EventHandler(this.btnSetZone_Click);
            // 
            // numSetStart
            // 
            this.numSetStart.Location = new System.Drawing.Point(12, 25);
            this.numSetStart.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numSetStart.Name = "numSetStart";
            this.numSetStart.Size = new System.Drawing.Size(65, 20);
            this.numSetStart.TabIndex = 2;
            // 
            // btnAddZone
            // 
            this.btnAddZone.Location = new System.Drawing.Point(12, 190);
            this.btnAddZone.Name = "btnAddZone";
            this.btnAddZone.Size = new System.Drawing.Size(75, 23);
            this.btnAddZone.TabIndex = 1;
            this.btnAddZone.Text = "Place Zone";
            this.btnAddZone.UseVisualStyleBackColor = true;
            this.btnAddZone.Click += new System.EventHandler(this.btnAddZone_Click);
            // 
            // numSetEnd
            // 
            this.numSetEnd.Location = new System.Drawing.Point(82, 25);
            this.numSetEnd.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numSetEnd.Name = "numSetEnd";
            this.numSetEnd.Size = new System.Drawing.Size(65, 20);
            this.numSetEnd.TabIndex = 2;
            // 
            // numPlaceStart
            // 
            this.numPlaceStart.Location = new System.Drawing.Point(150, 193);
            this.numPlaceStart.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numPlaceStart.Name = "numPlaceStart";
            this.numPlaceStart.Size = new System.Drawing.Size(65, 20);
            this.numPlaceStart.TabIndex = 2;
            this.numPlaceStart.ValueChanged += new System.EventHandler(this.numPlaceStart_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "zone start        zone end";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(98, 195);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "zone start";
            // 
            // listActions
            // 
            this.listActions.FormattingEnabled = true;
            this.listActions.Location = new System.Drawing.Point(12, 243);
            this.listActions.Name = "listActions";
            this.listActions.Size = new System.Drawing.Size(276, 95);
            this.listActions.TabIndex = 0;
            // 
            // chkReplace
            // 
            this.chkReplace.AutoSize = true;
            this.chkReplace.Location = new System.Drawing.Point(221, 194);
            this.chkReplace.Name = "chkReplace";
            this.chkReplace.Size = new System.Drawing.Size(67, 17);
            this.chkReplace.TabIndex = 6;
            this.chkReplace.Text = "replace?";
            this.chkReplace.UseVisualStyleBackColor = true;
            this.chkReplace.CheckedChanged += new System.EventHandler(this.chkReplace_CheckedChanged);
            // 
            // btnFinalize
            // 
            this.btnFinalize.Location = new System.Drawing.Point(213, 344);
            this.btnFinalize.Name = "btnFinalize";
            this.btnFinalize.Size = new System.Drawing.Size(75, 23);
            this.btnFinalize.TabIndex = 7;
            this.btnFinalize.Text = "Finalize";
            this.btnFinalize.UseVisualStyleBackColor = true;
            this.btnFinalize.Click += new System.EventHandler(this.btnFinalize_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 163);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save Zone";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(93, 163);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 8;
            this.btnLoad.Text = "Load Zone";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtNameZone
            // 
            this.txtNameZone.Enabled = false;
            this.txtNameZone.Location = new System.Drawing.Point(188, 165);
            this.txtNameZone.Name = "txtNameZone";
            this.txtNameZone.Size = new System.Drawing.Size(100, 20);
            this.txtNameZone.TabIndex = 9;
            this.txtNameZone.Text = "Zone 0";
            this.txtNameZone.TextChanged += new System.EventHandler(this.txtNameZone_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 229);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Actions:";
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(188, 219);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(56, 22);
            this.btnUp.TabIndex = 12;
            this.btnUp.Text = "move up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(246, 219);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(42, 22);
            this.btnDown.TabIndex = 12;
            this.btnDown.Text = "down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // Part_Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 373);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtNameZone);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnFinalize);
            this.Controls.Add(this.chkReplace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numPlaceStart);
            this.Controls.Add(this.numSetEnd);
            this.Controls.Add(this.btnAddZone);
            this.Controls.Add(this.numSetStart);
            this.Controls.Add(this.btnSetZone);
            this.Controls.Add(this.listActions);
            this.Controls.Add(this.listZones);
            this.Controls.Add(this.label3);
            this.KeyPreview = true;
            this.Name = "Part_Editor";
            this.Text = "Part Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Part_Editor_FormClosing);
            this.Load += new System.EventHandler(this.Part_Editor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Part_Editor_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.numSetStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlaceStart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listZones;
        private System.Windows.Forms.Button btnSetZone;
        private System.Windows.Forms.NumericUpDown numSetStart;
        private System.Windows.Forms.Button btnAddZone;
        private System.Windows.Forms.NumericUpDown numSetEnd;
        private System.Windows.Forms.NumericUpDown numPlaceStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listActions;
        private System.Windows.Forms.CheckBox chkReplace;
        private System.Windows.Forms.Button btnFinalize;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox txtNameZone;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;

    }
}