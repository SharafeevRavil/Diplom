namespace ScaApi.OssIndexClient;

static class Guard
{
    public static void AgainstNull(object value, string argumentName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstNullOrEmpty(string value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstEmpty(Guid value, string argumentName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentNullException(argumentName);
        }
    }
}