namespace Classroom.Utilities.Constants
{
    /// <summary>
    /// SystemVariable
    /// </summary>
    /// <author>huynhdev24</author>
    public static class SystemVariable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
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