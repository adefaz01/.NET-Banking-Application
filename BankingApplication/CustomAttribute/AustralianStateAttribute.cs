using System.ComponentModel.DataAnnotations;

namespace BankingApplication.CustomAttribute;
          

public class AustralianStateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is string state)
        {
            string[] validStates = ["ACT", "NSW", "NT", "QLD", "SA", "TAS", "VIC", "WA"];

            return validStates.Any(x => x.Equals(state, StringComparison.OrdinalIgnoreCase));

        }

        return true;

    }


}
