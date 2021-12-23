using System.Collections.Generic;

namespace BugTracker.Models.Database
{
    public class ProjectsModel
    {
        public IEnumerable<Project> Projects { get; set; }
        public Project NewProject { get; set; }
    }
}
