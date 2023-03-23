namespace Classroom.Utilities.Constants
{
    public static class SystemVariable
    {
        public static string GetRanDomClassID(int length)
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

            IEnumerable<string> string_Enumerable = Enumerable.Repeat(chars, length);
            char[] arr = string_Enumerable.Select(s => s[random.Next(s.Length)]).ToArray();

            return new string(arr);
        }
    }
}