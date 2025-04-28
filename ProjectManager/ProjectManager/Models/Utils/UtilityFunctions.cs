namespace ExpenseTracker.Models.Utils
{
    public class UtilityFunctions
    {
        public static string CapitalizeFirstLetter(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
    }
}
