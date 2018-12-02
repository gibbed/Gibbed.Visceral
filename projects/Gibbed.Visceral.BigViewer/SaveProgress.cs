/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Gibbed.Visceral.FileFormats;

namespace Gibbed.Visceral.BigViewer
{
	public partial class SaveProgress : Form
	{
		public SaveProgress()
		{
			this.InitializeComponent();
		}

		delegate void SetStatusDelegate(string status, int percent);
		private void SetStatus(string status, int percent)
		{
			if (this.progressBar.InvokeRequired || this.statusLabel.InvokeRequired)
			{
				SetStatusDelegate callback = new SetStatusDelegate(SetStatus);
				this.Invoke(callback, new object[] { status, percent });
				return;
			}

			this.statusLabel.Text = status;
			this.progressBar.Value = percent;
		}

		delegate void SaveDoneDelegate();
		private void SaveDone()
		{
			if (this.InvokeRequired)
			{
				SaveDoneDelegate callback = new SaveDoneDelegate(SaveDone);
				this.Invoke(callback);
				return;
			}

			this.Close();
		}

		public void SaveAll(object oinfo)
		{
			SaveAllInformation info = (SaveAllInformation)oinfo;
            IEnumerable<BigFile.Entry> saving;

            if (info.Saving == null)
            {
                saving = info.Archive.Entries;
            }
            else
            {
                saving = info.Saving;
            }

            this.SetStatus("", 0);

            int total = saving.Count();
            int current = 0;

            byte[] buffer = new byte[0x4000];
			foreach (var entry in saving)
			{
                current++;

                if (entry.Duplicate == true &&
                    info.Settings.SaveFilesWithDuplicateNames == false)
                {
                    this.SetStatus("Skipping...", (int)(((float)current / (float)total) * 100.0f));
                    continue;
                }

				string fileName = null;

                if (info.FileNames.ContainsKey(entry.Name) == true)
				{
                    fileName = info.FileNames[entry.Name];
                    
                    if (entry.Duplicate == true)
                    {
                        string newFileName;
                        
                        newFileName = Path.GetFileNameWithoutExtension(fileName);
                        newFileName = string.Format("{0} (duplicate #{1})", newFileName, current);
                        newFileName = Path.ChangeExtension(newFileName, Path.GetExtension(fileName));
                        fileName = Path.Combine(Path.GetDirectoryName(fileName), newFileName);
                    }
				}
				else
				{
					if (info.Settings.SaveOnlyKnownFiles)
					{
                        this.SetStatus("Skipping...", (int)(((float)current / (float)total) * 100.0f));
						continue;
					}

                    fileName = entry.Name.ToString("X8");

                    if (true)
                    {
                        info.Stream.Seek(entry.Offset, SeekOrigin.Begin);
                        byte[] guess = new byte[16];
                        int read = info.Stream.Read(guess, 0, guess.Length);

                        if (
                            read >= 4 &&
                            guess[0] == 0x28 &&
                            guess[1] == 0x7B &&
                            guess[2] == 0xBA &&
                            guess[3] == 0x9C)
                        {
                            fileName = Path.ChangeExtension(fileName, ".toc");
                            fileName = Path.Combine("archive", fileName);
                        }
                        else if (
                            read >= 4 &&
                            guess[0] == 0x9C &&
                            guess[1] == 0xBA &&
                            guess[2] == 0x7B &&
                            guess[3] == 0x28)
                        {
                            fileName = Path.ChangeExtension(fileName, ".toc");
                            fileName = Path.Combine("archive", fileName);
                        }
                        else if (
                            read >= 4 &&
                            guess[0] == '3' && // 3
                            guess[1] == 's' && // s
                            guess[2] == 'l' && // l
                            guess[3] == 'o')   // o
                        {
                            fileName = Path.ChangeExtension(fileName, ".str");
                            fileName = Path.Combine("archive", fileName);
                        }
                        else if (
                            read >= 4 &&
                            guess[0] == 'o' && // o
                            guess[1] == 'l' && // l
                            guess[2] == 's' && // s
                            guess[3] == '3')   // 3
                        {
                            fileName = Path.ChangeExtension(fileName, ".str");
                            fileName = Path.Combine("archive", fileName);
                        }
                        else if (
                            read >= 4 &&
                            guess[0] == 'M' &&
                            guess[1] == 'V' &&
                            guess[2] == 'h' &&
                            guess[3] == 'd')
                        {
                            fileName = Path.ChangeExtension(fileName, ".vp6");
                            fileName = Path.Combine("video", fileName);
                        }
                        else if (
                            read >= 4 &&
                            guess[0] == 'S' &&
                            guess[1] == 'C' &&
                            guess[2] == 'H' &&
                            guess[3] == 'l')
                        {
                            fileName = Path.ChangeExtension(fileName, ".vp6");
                            fileName = Path.Combine("audio", fileName);
                        }
                        else if (
                            read >= 3 &&
                            guess[0] == 0x03 &&
                            guess[1] == 0x00 &&
                            guess[2] == 0x00)
                        {
                            fileName = Path.ChangeExtension(fileName, ".exa.snu");
                            fileName = Path.Combine("audio", fileName);
                        }
                        else
                        {
                            //fileName = Path.Combine(Path.Combine("unknown", fileName.Substring(0, 1)), fileName);
                            fileName = Path.Combine("unknown", fileName);
                        }
                    }

                    fileName = Path.Combine("__UNKNOWN", fileName);

                    if (entry.Duplicate == true)
                    {
                        string newFileName;

                        newFileName = Path.GetFileNameWithoutExtension(fileName);
                        newFileName = string.Format("{0} (duplicate #{1})", newFileName, current);
                        newFileName = Path.ChangeExtension(newFileName, Path.GetExtension(fileName));
                        fileName = Path.Combine(Path.GetDirectoryName(fileName), newFileName);
                    }
				}

				string path = Path.Combine(info.BasePath, fileName);
                if (File.Exists(path) == true &&
                    info.Settings.DontOverwriteFiles == true)
                {
                    this.SetStatus("Skipping...", (int)(((float)current / (float)total) * 100.0f));
                    continue;
                }

                if (info.Settings.DontSaveAudioFiles == true &&
                    Path.GetExtension(path) == ".snu")
                {
                    this.SetStatus("Skipping...", (int)(((float)current / (float)total) * 100.0f));
                    continue;
                }

                this.SetStatus(fileName, (int)(((float)current / (float)total) * 100.0f));

                Directory.CreateDirectory(Path.Combine(info.BasePath, Path.GetDirectoryName(fileName)));

                info.Stream.Seek(entry.Offset, SeekOrigin.Begin);

                using (var output = File.Create(path))
                {
                    int left = (int)entry.Size;
                    while (left > 0)
                    {
                        int read = info.Stream.Read(buffer, 0, Math.Min(left, buffer.Length));
                        if (read == 0)
                        {
                            break;
                        }
                        output.Write(buffer, 0, read);
                        left -= read;
                    }
                }
			}

			this.SaveDone();
		}

        public struct SaveAllSettings
        {
            public bool SaveOnlyKnownFiles;
            public bool SaveFilesWithDuplicateNames;
            public bool DontOverwriteFiles;
            public bool DontSaveAudioFiles;
        }

		private struct SaveAllInformation
		{
			public string BasePath;
			public Stream Stream;
			public BigFile Archive;
            public IEnumerable<BigFile.Entry> Saving;
			public Dictionary<uint, string> FileNames;
            public SaveAllSettings Settings;
		}

		private Thread SaveThread;
		public void ShowSaveProgress(
            IWin32Window owner,
            Stream stream,
            BigFile archive,
            IEnumerable<BigFile.Entry> saving,
            Dictionary<uint, string> fileNames,
            string basePath,
            SaveAllSettings settings)
		{
			SaveAllInformation info;
			info.BasePath = basePath;
			info.Stream = stream;
			info.Archive = archive;
            info.Saving = saving;
			info.FileNames = fileNames;
            info.Settings = settings;

			this.progressBar.Value = 0;
			this.progressBar.Maximum = 100;

			this.SaveThread = new Thread(new ParameterizedThreadStart(SaveAll));
			this.SaveThread.Start(info);
			this.ShowDialog(owner);
		}

		private void OnCancel(object sender, EventArgs e)
		{
			if (this.SaveThread != null)
			{
				this.SaveThread.Abort();
			}

			this.Close();
		}
	}
}
