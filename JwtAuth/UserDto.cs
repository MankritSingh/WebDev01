namespace JwtAuth
{
    //dto data tranfer object
    public class UserDto
    {
        //why we use string.empty
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class UserTokenDto
    {
        //why we use string.empty
        public string Token { get; set; } = string.Empty;
    }

}
