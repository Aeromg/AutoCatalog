namespace AutoCatalogLib.Exchange
{
    public enum GeneralizedType : byte
    {
        Unknown = 0,
        String = 1,
        Boolean = 2,
        Integer = 3,
        Float = 4,
        DateTime = 5,
        ArrayOfUnknown = 16,
        ArrayOfString = ArrayOfUnknown + String,
        ArrayOfBoolean = ArrayOfUnknown + Boolean,
        ArrayOfInteger = ArrayOfUnknown + Integer,
        ArrayOfFloat = ArrayOfUnknown + Float,
        ArrayOfDateTime = ArrayOfUnknown + DateTime
    }
}