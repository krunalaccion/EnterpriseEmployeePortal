namespace EmployeePortal.Business.Validation;

public interface IInputValidationService
{
    bool ValidateEmployeeCodeRegexUnsafe(string inputCode);
    (bool IsValid, string ErrorMessage) ValidateLeaveDates(DateTime startDate, DateTime endDate);
}

