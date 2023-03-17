namespace OnlineBanking.API;

public static class ApiRoutes
{
    public const string BaseRoute = "api/v{version:apiVersion}/[controller]";

    public static class BankAccounts
    {
        public const string All = "all";
        public const string IdRoute = "{id:guid}";
        public const string GetByCustomerNo = "{customerNo}";
        public const string GetByIBAN = "{iban}";
        public const string GetByAccountNo = "{accountNo}";
        public const string AccountTransactions = "{iban}/account-transactions";
        public const string Activate = "activate/{iban}";
        public const string Deactivate = "deactivate/{iban}";
    }

    public static class CashTransactions
    {
        public const string All = "all";
        public const string IdRoute = "{id:guid}";
        public const string GetByIBAN = "{iban:string}";
        public const string GetByAccountNo = "{accountNo:string}";
    }

    public static class Branches
    {
        public const string All = "all";
        public const string IdRoute = "{id:int}";
        public const string GetByCountry = "{countryId}";
    }

    public static class Customers
    {
        public const string All = "all";
        public const string IdRoute = "{id:guid}";
        public const string GetByCustomerNo = "{customerNo}";
        public const string BankAccounts = "{id}/bank-accounts";
    }

    
    public static class CreditCards
    {
        public const string All = "all";
        public const string IdRoute = "{id:guid}";
        public const string GetByIBAN = "{iban:string}";
        public const string GetByAccountNo = "{accountNo:string}";
        public const string Activate = "activate/{creditCardNo:string}";
        public const string Deactivate = "deactivate/{creditCardNo:string}";
    }

    
    public static class FastTransactions
    {
        public const string All = "all";
        public const string IdRoute = "{id:guid}";
        public const string GetByIBAN = "iban/{iban:string}";
        public const string GetByAccountNo = "{accountNo:string}";
        public const string DeleteById = "account/{bankAccountId:guid}/fast-transactions/{id:guid}";
    }

    public static class AppUsers
    {
        public const string All = "all";
        public const string IdRoute = "{id}";
        public const string Login = "login";
        public const string Signup = "signup";
        public const string RefreshToken = "refresh-token";
        public const string Revoke = "revoke/{username:string}";
        public const string CurrentUser = "current-user";
        public const string EmailExists = "email-exists";
        public const string Address = "address";
        public const string Phone = "phone";
    }
}
