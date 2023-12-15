namespace JwtAuth
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        //why byte array
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set;}
        public string Token { get; set; }
        //why not just add refresh token?
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
