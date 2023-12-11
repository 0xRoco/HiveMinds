namespace HiveMinds.Core;

public class HiveMindsSettings 
{
    public int MaxThoughtLength { get; init; } // Free = 250, Premium = 500, Admin = 0 (unlimited)
    public int MaxImageSize { get; init; } // Free = 2.5MB, Premium = 5MB, Admin = 0 (unlimited)
    public string DefaultProfilePicture { get; init; } = "https://i.imgur.com/V0GPiKR.png";
}