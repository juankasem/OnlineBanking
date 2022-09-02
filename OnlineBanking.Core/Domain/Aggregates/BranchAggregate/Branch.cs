using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Core.Domain.Common;

namespace OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
    public class Branch : BaseDomainEntity
    {
        public new int Id { get; set; }
        public string Name { get; set; }
        public Address Address  { get; set; }
    }