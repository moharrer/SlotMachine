using System.Text.RegularExpressions;

namespace SlotMachine.Validations
{
    public static class EmailValidator
    {
        public static bool ValidateEmailAddress(string emailAddress)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            bool isMatch = Regex.IsMatch(emailAddress, pattern);

            return isMatch;
        }
    }
}
