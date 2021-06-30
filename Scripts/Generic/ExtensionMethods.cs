public static class ExtensionMethods
{
    public static string FirstLetterToUpperCase(this string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        var a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }
}