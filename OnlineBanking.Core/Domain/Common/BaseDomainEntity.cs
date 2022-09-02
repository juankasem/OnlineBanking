using System;

namespace OnlineBanking.Core.Domain.Common
{
    public abstract class BaseDomainEntity
    {
        public Guid Id { get;  set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}