using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using Gibbed.Visceral.FileFormats;
using Microsoft.Win32;

namespace Gibbed.Visceral.Setup
{
    public class Project
    {
        public string Name { get; private set; }
        public bool Hidden { get; private set; }
        public string InstallPath { get; private set; }
        public string ListsPath { get; private set; }
        public List<string> Dependencies { get; private set; }
        private bool Loaded;

        public Dictionary<uint, string> NameHashLookup { get; private set; }
        public Dictionary<uint, string> FileHashLookup { get; private set; }
        
        internal Manager Manager;

        private Project()
        {
            this.Dependencies = new List<string>();
            this.NameHashLookup = new Dictionary<uint, string>();
            this.FileHashLookup = new Dictionary<uint, string>();
            this.Loaded = false;
        }

        public void Load()
        {
            if (this.Loaded == false)
            {
                this.Reload();
            }
        }

        public void Reload()
        {
            this.Loaded = true;
            this.LoadFileLists();
            this.LoadNameLists();
        }

        private void LoadFileLists()
        {
            this.FileHashLookup.Clear();

            foreach (var name in this.Dependencies)
            {
                var dependency = this.Manager[name];
                if (dependency != null)
                {
                    this.LoadFileListsFrom(dependency.ListsPath);
                }
            }

            this.LoadFileListsFrom(this.ListsPath);
        }

        private void LoadFileListsFrom(string basePath)
        {
            if (Directory.Exists(basePath) == false)
            {
                return;
            }

            foreach (string listPath in Directory.GetFiles(basePath, "*.filelist", SearchOption.AllDirectories))
            {
                Stream input = File.Open(listPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                TextReader reader = new StreamReader(input);

                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    if (line.Length <= 0)
                    {
                        continue;
                    }

                    line = line.Replace('/', '\\');
                    line = line.ToLowerInvariant();

                    //uint hash = Path.GetFileName(line).HashFileName();
                    uint hash = line.HashFileName();

                    if (this.FileHashLookup.ContainsKey(hash) == true &&
                        this.FileHashLookup[hash] != line)
                    {
                        string otherLine = this.FileHashLookup[hash];
                        throw new InvalidOperationException(
                            string.Format(
                                "duplicate hash ('{0}' vs '{1}')",
                                line,
                                otherLine));
                    }

                    this.FileHashLookup[hash] = line; // .Add(hash, line);
                }

                reader.Close();
                input.Close();
            }
        }

        private void LoadNameLists()
        {
            this.NameHashLookup.Clear();

            foreach (var name in this.Dependencies)
            {
                var dependency = this.Manager[name];
                if (dependency != null)
                {
                    this.LoadNameListsFrom(dependency.ListsPath);
                }
            }

            this.LoadNameListsFrom(this.ListsPath);
        }

        private void LoadNameListsFrom(string basePath)
        {
            if (Directory.Exists(basePath) == false)
            {
                return;
            }

            foreach (string listPath in Directory.GetFiles(basePath, "*.namelist", SearchOption.AllDirectories))
            {
                Stream input = File.Open(listPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                TextReader reader = new StreamReader(input);

                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    line = line.Trim();
                    if (line.Length <= 0)
                    {
                        continue;
                    }

                    //uint hash = Path.GetFileName(line).HashFileName();
                    uint hash = line.HashFileName();

                    if (this.NameHashLookup.ContainsKey(hash) &&
                        this.NameHashLookup[hash] != line)
                    {
                        string otherLine = this.NameHashLookup[hash];
                        throw new InvalidOperationException("duplicate hash");
                    }

                    this.NameHashLookup[hash] = line; // .Add(hash, line);
                }

                reader.Close();
                input.Close();
            }
        }

        internal static Project Create(string path, Manager manager)
        {
            var project = new Project();
            project.Manager = manager;

            var doc = new XPathDocument(path);
            var nav = doc.CreateNavigator();

            project.Name = nav.SelectSingleNode("/project/name").Value;
            project.ListsPath = nav.SelectSingleNode("/project/list_location").Value;

            project.Hidden = nav.SelectSingleNode("/project/hidden") != null;

            if (Path.IsPathRooted(project.ListsPath) == false)
            {
                project.ListsPath = Path.Combine(Path.GetDirectoryName(path), project.ListsPath);
            }

            project.Dependencies = new List<string>();
            var dependencies = nav.Select("/project/dependencies/dependency");
            while (dependencies.MoveNext() == true)
            {
                project.Dependencies.Add(dependencies.Current.Value);
            }

            project.InstallPath = null;
            var locations = nav.Select("/project/install_locations/install_location");
            while (locations.MoveNext() == true)
            {
                bool failed = true;

                var actions = locations.Current.Select("action");
                string locationPath = null;
                while (actions.MoveNext() == true)
                {
                    string type = actions.Current.GetAttribute("type", "");

                    switch (type)
                    {
                        case "registry":
                        {
                            string key = actions.Current.GetAttribute("key", "");
                            string value = actions.Current.GetAttribute("value", "");
                            
                            path = (string)Registry.GetValue(key, value, null);

                            if (path != null) // && Directory.Exists(path) == true)
                            {
                                locationPath = path;
                                failed = false;
                            }

                            break;
                        }

                        case "path":
                        {
                            locationPath = actions.Current.Value;

                            if (Directory.Exists(locationPath) == true)
                            {
                                failed = false;
                            }

                            break;
                        }

                        case "combine":
                        {
                            locationPath = Path.Combine(locationPath, actions.Current.Value);

                            if (Directory.Exists(locationPath) == true)
                            {
                                failed = false;
                            }

                            break;
                        }

                        case "directory_name":
                        {
                            locationPath = Path.GetDirectoryName(locationPath);

                            if (Directory.Exists(locationPath) == true)
                            {
                                failed = false;
                            }

                            break;
                        }

                        case "fix":
                        {
                            locationPath = locationPath.Replace('/', '\\');
                            failed = false;
                            break;
                        }

                        default:
                        {
                            throw new InvalidOperationException("unhandled install location action type");
                        }
                    }

                    if (failed == true)
                    {
                        break;
                    }
                }

                if (failed == false && Directory.Exists(locationPath) == true)
                {
                    project.InstallPath = locationPath;
                    break;
                }
            }

            return project;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
