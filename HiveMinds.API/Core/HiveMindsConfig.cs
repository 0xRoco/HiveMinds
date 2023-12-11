namespace HiveMinds.API.Core;

public class HiveMindsConfig
{
    public int MaxThoughtsLength { get; set; }
    public int MaxImageSize { get; set; }
    public int TokenExpirationHours { get; set; }
    public string DefaultProfileImage { get; set; } = "";
    public string BaseUrl { get; set; } = "";
}