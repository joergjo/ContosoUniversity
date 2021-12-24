namespace ContosoUniversity;

public static class Extensions
{
    #if SQLSERVER
    public static string GetLastChars(byte[] bytes) => bytes[^7..].ToString();
#else
    public static string GetLastChars(this Guid guid) => guid.ToString()[^3..];
#endif
}