namespace JwtAuth
{
    public class RefreshToken
    {
        //public int Id { get; set; } not being used now
        public string Token { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; }
    }
}
