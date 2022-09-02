using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Core.Domain.Enums;

public enum DocumentType : int
{
    ID,
    Passport,
    SSN,
    WorkPermit,
    DrivingLicence
}
