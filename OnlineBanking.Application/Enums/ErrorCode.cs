

namespace OnlineBanking.Application.Enums;

public enum ErrorCode
{
    BadRequest = 400,
    NotFound = 404,
    InternalServerError = 500,

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
    CustomerAlreadyExists = 308,

    UnAuthorizedOperation = 401,
    UnknownError = 999
}