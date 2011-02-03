using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Gibbed.Visceral.FileFormats;

namespace Gibbed.Visceral.ArchiveViewer
{
    public partial class Viewer : Form
    {
        public Viewer()
        {
            this.InitializeComponent();
        }

        private Setup.Manager Manager;

        private void OnLoad(object sender, EventArgs e)
        {
            this.LoadProject();
        }

        private void LoadProject()
        {
            try
            {
                this.Manager = Setup.Manager.Load();
                this.projectComboBox.Items.AddRange(this.Manager.ToArray());
                this.SetProject(this.Manager.ActiveProject);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "There was an error while loading project data." +
                    Environment.NewLine + Environment.NewLine +
                    e.ToString() +
                    Environment.NewLine + Environment.NewLine +
                    "(You can press Ctrl+C to copy the contents of this dialog)",
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void SetProject(Setup.Project project)
        {
            if (project != null)
            {
                try
                {
                    project.Load();
                    this.openDialog.InitialDirectory = project.InstallPath;
                    this.saveKnownFileListDialog.InitialDirectory = project.ListsPath;
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        "There was an error while loading project data." +
                        Environment.NewLine + Environment.NewLine +
                        e.ToString() +
                        Environment.NewLine + Environment.NewLine +
                        "(You can press Ctrl+C to copy the contents of this dialog)",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    project = null;
                }
            }

            if (project != this.Manager.ActiveProject)
            {
                this.Manager.ActiveProject = project;
            }

            this.projectComboBox.SelectedItem = project;
        }

        private BigFile Archive;
        private void BuildFileTree()
        {
            this.fileList.Nodes.Clear();
            this.fileList.BeginUpdate();

            if (this.Archive != null)
            {
                Dictionary<string, TreeNode> dirNodes = new Dictionary<string, TreeNode>();

                TreeNode baseNode = new TreeNode(Path.GetFileName(this.openDialog.FileName), 0, 0);
                TreeNode knownNode = new TreeNode("Known", 1, 1);
                TreeNode unknownNode = new TreeNode("Unknown", 1, 1);

                Dictionary<uint, string> lookup = 
                    this.Manager.ActiveProject == null ? null : this.Manager.ActiveProject.FileHashLookup;

                foreach (var entry in this.Archive.Entries
                    .OrderBy(k => k.Name, new FileNameHashComparer(lookup)))
                {
                    TreeNode node = null;

                    if (lookup != null && lookup.ContainsKey(entry.Name) == true)
                    {
                        string fileName = lookup[entry.Name];
                        string pathName = Path.GetDirectoryName(fileName);
                        TreeNodeCollection parentNodes = knownNode.Nodes;

                        if (pathName.Length > 0)
                        {
                            string[] dirs = pathName.Split(new char[] { '\\' });

                            foreach (string dir in dirs)
                            {
                                if (parentNodes.ContainsKey(dir))
                                {
                                    parentNodes = parentNodes[dir].Nodes;
                                }
                                else
                                {
                                    TreeNode parentNode = parentNodes.Add(dir, dir, 2, 2);
                                    parentNodes = parentNode.Nodes;
                                }
                            }
                        }

                        node = parentNodes.Add(null, Path.GetFileName(fileName) /*+ " [" + entry.Name.ToString("X8") + "]"*/, 3, 3);
                    }
                    else
                    {
                        node = unknownNode.Nodes.Add(null, entry.Name.ToString("X8"), 3, 3);
                    }

                    node.Tag = entry;

                    if (entry.Duplicate == true)
                    {
                        node.Text += " (duplicate)";
                        node.ForeColor = System.Drawing.Color.Blue;
                    }
                }

                if (knownNode.Nodes.Count > 0)
                {
                    baseNode.Nodes.Add(knownNode);
                }

                if (unknownNode.Nodes.Count > 0)
                {
                    baseNode.Nodes.Add(unknownNode);
                    unknownNode.Text = "Unknown (" + unknownNode.Nodes.Count.ToString() + ")";
                }

                if (knownNode.Nodes.Count > 0)
                {
                    knownNode.Expand();
                }
                else if (unknownNode.Nodes.Count > 0)
                {
                    //unknownNode.Expand();
                }

                baseNode.Expand();
                this.fileList.Nodes.Add(baseNode);
            }

            //this.fileList.Sort();
            this.fileList.EndUpdate();
        }

        private void OnOpen(object sender, EventArgs e)
        {
            if (this.openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (this.openDialog.InitialDirectory != null)
            {
                this.openDialog.InitialDirectory = null;
            }

            BigFile archive;
            using (var input = this.openDialog.OpenFile())
            {
                archive = new BigFile();
                archive.Deserialize(input);
            }
            this.Archive = archive;

            /*
            TextWriter writer = new StreamWriter("all_file_hashes.txt");
            foreach (var hash in table.Keys.OrderBy(k => k))
            {
                writer.WriteLine(hash.ToString("X8"));
            }
            writer.Close();
            */

            this.BuildFileTree();
        }

        private void OnSave(object sender, EventArgs e)
        {
            if (this.fileList.SelectedNode == null)
            {
                return;
            }

            string basePath;
            Dictionary<uint, string> lookup;
            List<BigFile.Entry> saving;

            SaveProgress.SaveAllSettings settings;
            settings.SaveFilesWithDuplicateNames = this.saveDuplicateNamesMenuItem.Checked;
            settings.SaveOnlyKnownFiles = false;
            settings.DontOverwriteFiles = this.dontOverwriteFilesMenuItem.Checked;
            settings.DontSaveAudioFiles = false; //this.dontSaveAudioFilesMenuItem.Checked;

            var root = this.fileList.SelectedNode;
            if (root.Nodes.Count == 0)
            {
                this.saveFileDialog.FileName = root.Text;

                if (this.saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var entry = (BigFile.Entry)root.Tag; 

                saving = new List<BigFile.Entry>();
                saving.Add(entry);

                lookup = new Dictionary<uint, string>();
                lookup.Add(entry.Name, Path.GetFileName(this.saveFileDialog.FileName));
                basePath = Path.GetDirectoryName(this.saveFileDialog.FileName);

                settings.DontOverwriteFiles = false;
            }
            else
            {
                if (this.saveFilesDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                saving = new List<BigFile.Entry>();
                
                List<TreeNode> nodes = new List<TreeNode>();
                nodes.Add(root);

                while (nodes.Count > 0)
                {
                    var node = nodes[0];
                    nodes.RemoveAt(0);

                    if (node.Nodes.Count > 0)
                    {
                        foreach (TreeNode child in node.Nodes)
                        {
                            if (child.Nodes.Count > 0)
                            {
                                nodes.Add(child);
                            }
                            else
                            {
                                saving.Add((BigFile.Entry)child.Tag);
                            }
                        }
                    }
                }

                lookup = this.Manager.ActiveProject == null ? null : this.Manager.ActiveProject.FileHashLookup;
                basePath = this.saveFilesDialog.SelectedPath;
            }

            Stream input = File.OpenRead(this.openDialog.FileName);

            if (input == null)
            {
                return;
            }

            SaveProgress progress = new SaveProgress();
            progress.ShowSaveProgress(
                this,
                input,
                this.Archive,
                saving,
                lookup,
                basePath,
                settings);

            input.Close();
        }

        private void OnSaveAll(object sender, EventArgs e)
        {
            if (this.saveFilesDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Stream input = File.OpenRead(this.openDialog.FileName);

            if (input == null)
            {
                return;
            }

            string basePath = this.saveFilesDialog.SelectedPath;

            Dictionary<uint, string> lookup =
                this.Manager.ActiveProject == null ? null : this.Manager.ActiveProject.FileHashLookup;

            SaveProgress.SaveAllSettings settings;
            settings.SaveFilesWithDuplicateNames = this.saveDuplicateNamesMenuItem.Checked;
            settings.SaveOnlyKnownFiles = this.saveOnlyKnownFilesMenuItem.Checked;
            settings.DontOverwriteFiles = this.dontOverwriteFilesMenuItem.Checked;
            settings.DontSaveAudioFiles = this.dontSaveAudioFilesMenuItem.Checked;

            SaveProgress progress = new SaveProgress();
            progress.ShowSaveProgress(
                this,
                input,
                this.Archive,
                null, lookup,
                basePath,
                settings);

            input.Close();
        }

        private void OnReloadLists(object sender, EventArgs e)
        {
            if (this.Manager.ActiveProject != null)
            {
                this.Manager.ActiveProject.Reload();
            }

            this.BuildFileTree();
        }

        private void OnProjectSelected(object sender, EventArgs e)
        {
            this.projectComboBox.Invalidate();
            var project = this.projectComboBox.SelectedItem as Setup.Project;
            if (project == null)
            {
                this.projectComboBox.Items.Remove(this.projectComboBox.SelectedItem);
            }
            this.SetProject(project);
            this.BuildFileTree();
        }

        private void OnSaveKnownFileList(object sender, EventArgs e)
        {
            if (this.saveKnownFileListDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            List<string> names = new List<string>();

            if (this.Archive != null &&
                this.Manager.ActiveProject != null)
            {
                foreach (var entry in this.Archive.Entries)
                {
                    if (this.Manager.ActiveProject.FileHashLookup.ContainsKey(entry.Name) == true)
                    {
                        var name = this.Manager.ActiveProject.FileHashLookup[entry.Name];
                        if (names.Contains(name) == false)
                        {
                            names.Add(name);
                        }
                    }
                }
            }

            names.Sort();

            TextWriter output = new StreamWriter(this.saveKnownFileListDialog.OpenFile());
            foreach (string name in names)
            {
                output.WriteLine(name);
            }
            output.Close();
        }
    }
}
