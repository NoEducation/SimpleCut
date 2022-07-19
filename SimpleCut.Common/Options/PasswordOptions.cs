namespace SimpleCut.Common.Options
{
    public class TokenOptions
    {
        public static string Position => "Token";

        public string? Salt { get; set; }
        public string? Secrete { get; set; }
        public int AccessTokenTimeValid { get; set; }
        public int RefreshTokenTimeValid { get; set; }
    }
}
