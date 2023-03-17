

namespace OnlineBanking.Application.Enums;

public enum ErrorCode
{
    NotFound = 404,
    ServerError = 500,

    //Validation errors should be in the range 100 - 199
    ValidationError = 101,

    //Infrastructure errors should be in the range 200-299
    IdentityCreationFailed = 202,
    //Application errors should be in the range 300 - 399
    CashTransactionUpdateNotPossible = 300,
    CashTransactionDeleteNotPossible = 301,
    CreateCashTransactionNotAuthorized = 302,
    IdentityUserAlreadyExists = 303,
    IdentityUserDoesNotExist = 304,
    IncorrectPassword = 305,
    UnauthorizedAccountRemoval = 306,
    InSufficintFunds = 307,

    UnAuthorizedOperation = 308,
    UnknownError = 999
}