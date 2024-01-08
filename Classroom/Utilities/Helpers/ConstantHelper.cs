using System.Text;

namespace Classroom.Utilities.Helpers;

/// <summary>
/// ConstantHelper
/// </summary>
public class ConstantHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    public static string ToHexString(string str)
    {
        var sb = new StringBuilder();

        var bytes = Encoding.Unicode.GetBytes(str);
        foreach (var t in bytes)
        {
            sb.Append(t.ToString("X2"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    public static string FromHexString(string hexString)
    {
        var bytes = new byte[hexString.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
        }

        return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
    }


    public static string hostMail = "smtp.gmail.com";
    public static int portEmail = 587;
    public static string email = "huynh.it.24@gmail.com";
    public static string pass = "efjfvqsuufgaumhe";
}