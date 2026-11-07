using System.Text.RegularExpressions;

namespace EmployeePortal.Business.Validation;

public class InputValidationService : IInputValidationService
{
    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-VAL-02 | CWE-1333 / CWE-20: Inefficient Regular Expression Complexity (ReDoS)
    // WARNING: INTENTIONALLY VULNERABLE REGEX WITHOUT TIMEOUT FOR SECURESCANAI BENCHMARKING ONLY.
    // Nested quantifiers ^([a-zA-Z0-9]+\s?)+$ without RegexOptions.NonBacktracking or MatchTimeout.
    // =========================================================================
    private static readonly Regex UnsafeBacktrackingRegex = new(@"^([a-zA-Z0-9\-]+\s?)+$");

    public bool ValidateEmployeeCodeRegexUnsafe(string inputCode)
    {
        if (string.IsNullOrEmpty(inputCode))
        {
            return false;
        }

        return UnsafeBacktrackingRegex.IsMatch(inputCode);
    }

    public (bool IsValid, string ErrorMessage) ValidateLeaveDates(DateTime startDate, DateTime endDate)
    {
        if (endDate.Date < startDate.Date)
        {
            return (false, "End date cannot be earlier than start date.");
        }

        var spanDays = (endDate.Date - startDate.Date).TotalDays + 1;
        if (spanDays > 60)
        {
            return (false, "Single leave request cannot exceed 60 calendar days.");
        }

        return (true, string.Empty);
    }
}

