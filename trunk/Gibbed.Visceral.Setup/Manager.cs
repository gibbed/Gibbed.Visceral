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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gibbed.Visceral.Setup
{
    public class Manager : IEnumerable<Project>
    {
        private Manager()
        {
        }

        private string ProjectPath;

        private List<Project> Projects = new List<Project>();
        private Project _ActiveProject;
        public Project ActiveProject
        {
            get
            {
                return this._ActiveProject;
            }

            set
            {
                if (value == null)
                {
                    File.Delete(Path.Combine(this.ProjectPath, "current.txt"));
                }
                else
                {
                    Stream output = File.Open(Path.Combine(this.ProjectPath, "current.txt"), FileMode.Create, FileAccess.Write, FileShare.Read);
                    TextWriter writer = new StreamWriter(output);
                    writer.WriteLine(value.Name);
                    writer.Close();
                    output.Close();
                }

                this._ActiveProject = value;
            }
        }

        public Project this[string name]
        {
            get
            {
                var project = this.Projects.SingleOrDefault(
                    p => p.Name.ToLowerInvariant() == name.ToLowerInvariant());
                if (project != null)
                {
                    project.Load();
                }
                return project;
            }
        }

        public static Manager Load()
        {
            string projectPath;

            Manager manager = new Manager();

            projectPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            projectPath = Path.Combine(projectPath, "projects");

            manager.ProjectPath = projectPath;

            foreach (string xmlPath in Directory.GetFiles(projectPath, "*.xml", SearchOption.TopDirectoryOnly))
            {
                manager.Projects.Add(Project.Create(xmlPath, manager));
            }

            string currentPath = Path.Combine(projectPath, "current.txt");
            if (File.Exists(currentPath) == false)
            {
                manager._ActiveProject = null;
            }
            else
            {
                Stream input = File.Open(currentPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                TextReader reader = new StreamReader(input);
                string name = reader.ReadLine().Trim();
                reader.Close();
                input.Close();

                if (manager[name] != null)
                {
                    manager._ActiveProject = manager[name];
                }
            }

            if (manager.ActiveProject != null)
            {
                manager.ActiveProject.Load();
            }

            return manager;
        }

        public IEnumerator<Project> GetEnumerator()
        {
            return this.Projects.Where(
                    p =>
                        p.Hidden == false &&
                        p.InstallPath != null
                ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Projects.Where(
                    p =>
                        p.Hidden == false &&
                        p.InstallPath != null
                ).GetEnumerator();
        }
    }
}
