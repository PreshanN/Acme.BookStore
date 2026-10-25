using Acme.BookStore.Shared.Departments;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Acme.BookStore.Departments
{
    [Audited]
    public class Department : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; private set; } // owner of this row
        public string Name { get; private set; }
        public string? Description { get; private set; }
        private Department() { }
        public Department(Guid id, [NotNull] string name, string? description = null, Guid? tenantId = null) : base(id) { TenantId = tenantId; SetName(name); SetDescription(description); }
        public void SetName([NotNull] string name) { Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: DepartmentConsts.MaxNameLength); }
        public void SetDescription(string? description) { Description = Check.Length(description, nameof(description), maxLength: DepartmentConsts.MaxDescriptionLength); }
        public ContactInfo? ContactInfo { get; private set; }
        public void SetContactInfo(ContactInfo? contactInfo) { ContactInfo = contactInfo; }

        private readonly List<DepartmentTeam> _teams = new();
        public IReadOnlyCollection<DepartmentTeam> Teams => _teams.AsReadOnly();
        public DepartmentTeam AddTeam(Guid teamId, string teamName)
        {
            if (_teams.Any(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BusinessException("BookStore:DuplicateTeamName").WithData("TeamName", teamName);
            }
            if (_teams.Count >= DepartmentConsts.MaxTeamCount)
            {
                throw new BusinessException("BookStore:TooManyTeams");
            }

            var team = new DepartmentTeam(teamId, teamName);
            _teams.Add(team);
            return team;
        }
        public void RemoveTeam(Guid teamId) { _teams.RemoveAll(t => t.Id == teamId); }
    }
}
