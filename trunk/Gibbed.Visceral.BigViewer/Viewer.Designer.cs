namespace Gibbed.Visceral.BigViewer
{
    partial class Viewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Viewer));
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.projectComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.saveAllButton = new System.Windows.Forms.ToolStripButton();
            this.reloadListsButton = new System.Windows.Forms.ToolStripSplitButton();
            this.saveKnownListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.saveOnlyKnownFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDuplicateNamesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dontOverwriteFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dontSaveAudioFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileList = new System.Windows.Forms.TreeView();
            this.fileMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFilesDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveKnownFileListDialog = new System.Windows.Forms.SaveFileDialog();
            this.mainToolStrip.SuspendLayout();
            this.fileMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectComboBox,
            this.openButton,
            this.saveAllButton,
            this.reloadListsButton,
            this.settingsButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(640, 25);
            this.mainToolStrip.TabIndex = 0;
            this.mainToolStrip.Text = "toolStrip1";
            // 
            // projectComboBox
            // 
            this.projectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.projectComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.projectComboBox.Name = "projectComboBox";
            this.projectComboBox.Size = new System.Drawing.Size(121, 25);
            this.projectComboBox.Sorted = true;
            this.projectComboBox.SelectedIndexChanged += new System.EventHandler(this.OnProjectSelected);
            // 
            // openButton
            // 
            this.openButton.Image = global::Gibbed.Visceral.BigViewer.Properties.Resources.OpenArchive;
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(56, 22);
            this.openButton.Text = "&Open";
            this.openButton.Click += new System.EventHandler(this.OnOpen);
            // 
            // saveAllButton
            // 
            this.saveAllButton.Image = global::Gibbed.Visceral.BigViewer.Properties.Resources.SaveAllFiles;
            this.saveAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAllButton.Name = "saveAllButton";
            this.saveAllButton.Size = new System.Drawing.Size(68, 22);
            this.saveAllButton.Text = "Save &All";
            this.saveAllButton.Click += new System.EventHandler(this.OnSaveAll);
            // 
            // reloadListsButton
            // 
            this.reloadListsButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveKnownListToolStripMenuItem});
            this.reloadListsButton.Image = global::Gibbed.Visceral.BigViewer.Properties.Resources.ReloadLists;
            this.reloadListsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadListsButton.Name = "reloadListsButton";
            this.reloadListsButton.Size = new System.Drawing.Size(101, 22);
            this.reloadListsButton.Text = "&Reload Lists";
            this.reloadListsButton.ButtonClick += new System.EventHandler(this.OnReloadLists);
            // 
            // saveKnownListToolStripMenuItem
            // 
            this.saveKnownListToolStripMenuItem.Name = "saveKnownListToolStripMenuItem";
            this.saveKnownListToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.saveKnownListToolStripMenuItem.Text = "Save Known &List";
            this.saveKnownListToolStripMenuItem.Click += new System.EventHandler(this.OnSaveKnownFileList);
            // 
            // settingsButton
            // 
            this.settingsButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveOnlyKnownFilesMenuItem,
            this.saveDuplicateNamesMenuItem,
            this.dontOverwriteFilesMenuItem,
            this.dontSaveAudioFilesMenuItem});
            this.settingsButton.Image = global::Gibbed.Visceral.BigViewer.Properties.Resources.Settings;
            this.settingsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(78, 22);
            this.settingsButton.Text = "Settings";
            // 
            // saveOnlyKnownFilesMenuItem
            // 
            this.saveOnlyKnownFilesMenuItem.CheckOnClick = true;
            this.saveOnlyKnownFilesMenuItem.Name = "saveOnlyKnownFilesMenuItem";
            this.saveOnlyKnownFilesMenuItem.Size = new System.Drawing.Size(238, 22);
            this.saveOnlyKnownFilesMenuItem.Text = "Save only &known files";
            // 
            // saveDuplicateNamesMenuItem
            // 
            this.saveDuplicateNamesMenuItem.CheckOnClick = true;
            this.saveDuplicateNamesMenuItem.Name = "saveDuplicateNamesMenuItem";
            this.saveDuplicateNamesMenuItem.Size = new System.Drawing.Size(238, 22);
            this.saveDuplicateNamesMenuItem.Text = "Save files with &duplicate names";
            // 
            // dontOverwriteFilesMenuItem
            // 
            this.dontOverwriteFilesMenuItem.CheckOnClick = true;
            this.dontOverwriteFilesMenuItem.Name = "dontOverwriteFilesMenuItem";
            this.dontOverwriteFilesMenuItem.Size = new System.Drawing.Size(238, 22);
            this.dontOverwriteFilesMenuItem.Text = "Don\'t &overwrite files";
            // 
            // dontSaveAudioFilesMenuItem
            // 
            this.dontSaveAudioFilesMenuItem.CheckOnClick = true;
            this.dontSaveAudioFilesMenuItem.Name = "dontSaveAudioFilesMenuItem";
            this.dontSaveAudioFilesMenuItem.Size = new System.Drawing.Size(238, 22);
            this.dontSaveAudioFilesMenuItem.Text = "Don\'t save &audio files";
            // 
            // fileList
            // 
            this.fileList.ContextMenuStrip = this.fileMenuStrip;
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.Location = new System.Drawing.Point(0, 25);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(640, 295);
            this.fileList.TabIndex = 1;
            // 
            // fileMenuStrip
            // 
            this.fileMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileMenuStrip.Name = "fileMenuStrip";
            this.fileMenuStrip.Size = new System.Drawing.Size(99, 26);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.OnSave);
            // 
            // openDialog
            // 
            this.openDialog.Filter = "Visceral Archives (*.dat; *.viv)|*.dat;*.viv|All Files (*.*)|*.*";
            // 
            // saveFilesDialog
            // 
            this.saveFilesDialog.Description = "Select a directory to save all files from the archive to.";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "All Files (*.*)|*.*";
            // 
            // saveKnownFileListDialog
            // 
            this.saveKnownFileListDialog.DefaultExt = "filelist";
            this.saveKnownFileListDialog.Filter = "File List (*.filelist)|*.filelist";
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 320);
            this.Controls.Add(this.fileList);
            this.Controls.Add(this.mainToolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Viewer";
            this.Text = "Gibbed\'s Visceral Big Viewer";
            this.Load += new System.EventHandler(this.OnLoad);
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.fileMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.TreeView fileList;
        private System.Windows.Forms.OpenFileDialog openDialog;
        private System.Windows.Forms.FolderBrowserDialog saveFilesDialog;
        private System.Windows.Forms.ToolStripComboBox projectComboBox;
        private System.Windows.Forms.ContextMenuStrip fileMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripButton saveAllButton;
        private System.Windows.Forms.ToolStripDropDownButton settingsButton;
        private System.Windows.Forms.ToolStripMenuItem saveOnlyKnownFilesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dontOverwriteFilesMenuItem;
        private System.Windows.Forms.ToolStripSplitButton reloadListsButton;
        private System.Windows.Forms.ToolStripMenuItem saveKnownListToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveKnownFileListDialog;
        private System.Windows.Forms.ToolStripMenuItem saveDuplicateNamesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dontSaveAudioFilesMenuItem;
    }
}

