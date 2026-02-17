namespace SimpleAPI.Helpers;

public static class Helper
{
        public static string? ValidateSkipAndTake(int skip, int take)
        {
            if(skip < 0)
                return "Skip parameter cannot be negative number.";

            if (take <= 0 )
                return "Take parameter cannot be negative number or zero.";

            if(take > 1000)
                return "Take parameter can be at most 1000.";

            return null;
        }
        
        
}