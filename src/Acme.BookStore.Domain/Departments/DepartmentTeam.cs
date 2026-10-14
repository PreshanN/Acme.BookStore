using Acme.BookStore.Shared.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Acme.BookStore.Departments
{
    public class DepartmentTeam : Entity<Guid>
    {
        public string Name { get; internal set; }
        private DepartmentTeam() { }
        internal DepartmentTeam(Guid id, string name) : base(id)
        { Name = name; }

        private readonly List<DepartmentTeam> _teams = new();
        public IReadOnlyCollection<DepartmentTeam> Teams => _teams.AsReadOnly();

        public DepartmentTeam AddTeam(Guid teamId, string teamName) { if (_teams.Any(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase))) { throw new BusinessException("Hr:DuplicateTeamName").WithData("TeamName", teamName); } if (_teams.Count >= DepartmentConsts.MaxTeamCount) { throw new BusinessException("Hr:TooManyTeams"); } var team = new DepartmentTeam(teamId, teamName); _teams.Add(team); return team; }
        public void RemoveTeam(Guid teamId) { _teams.RemoveAll(t => t.Id == teamId); }
    }
}
