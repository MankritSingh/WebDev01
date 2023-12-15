using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserValidation;

namespace JwtAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static SymmetricSecurityKey? SigningKey;
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private string connectionStringUserInformation = @"Data Source = DESKTOP-8JBEJ91\SQLEXPRESS; Initial Catalog=UserInformation; Integrated Security=true";

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            SigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
        }     

        [HttpPost("register")]
        //The Parameter objects fields are taken as input fields in swagger/ASp?
        public async Task<ActionResult<User>> Register(UserDto request) {      
            //string connectionString1 = @"Data Source = DESKTOP-8JBEJ91\SQLEXPRESS; Initial Catalog=SAMPLE_DB; Integrated Security=true";

            Helpers.DBInteraction dBInteraction = new(connectionStringUserInformation);         
            if (dBInteraction.UserExists("UserInfo",request.Username)) {
                return BadRequest("Username Already Exists");
            }

            CreatePasswordHash(request.Password,out byte[] passwordHash, out byte[] passwordSalt);
            user.Username = request.Username;
            user.PasswordHash=passwordHash;
            user.PasswordSalt = passwordSalt;
            
         
            dBInteraction.WriteToUserInfoDb(user.Username,
                Convert.ToBase64String(user.PasswordHash) ,Convert.ToBase64String(user.PasswordSalt), "NA","NA");
            return Ok(user);    
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request) {
            
            Helpers.DBInteraction dBInteraction = new Helpers.DBInteraction(connectionStringUserInformation);
            if (!dBInteraction.UserExists("UserInfo", request.Username)) {
                return BadRequest("User not found");
            }
            var userInfoQuery = $"SELECT * FROM UserInfo WHERE UserName='{request.Username}'";
            var userInfo = dBInteraction.DbFetchEntries(userInfoQuery).FirstOrDefault();

            var userPasswordHash = Convert.FromBase64String((string)userInfo[3]);
            var userPasswordSalt= Convert.FromBase64String((string)userInfo[4]);

            using (var hmac = new HMACSHA512(userPasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
                if (!computedHash.SequenceEqual(userPasswordHash)) {
                    return BadRequest("Wrong Password");
                }
            }
            //if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt)) { 
            //    return BadRequest("Wrong Password");
            //}
            string token = CreateToken(user);
            user.Token = token;

            var refreshToken = generateRefreshToken();
            setRefreshToken(refreshToken); //read about http only cookies

            return Ok(token);
        }

        [HttpPost("Token Validate")]
        public async Task<ActionResult<string>> ValidateToken(UserTokenDto request) {
            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenString = request.Token;
            SecurityToken validatedtoken;
            if (!jwtHandler.CanReadToken(tokenString)) return BadRequest("Cannot Read Token");
            try
            {
                var claimsPrincipal = GetValidationParameters();              
                jwtHandler.ValidateToken(tokenString, claimsPrincipal, out validatedtoken);
            }
            catch (Exception)
            {
                return BadRequest("Cannot Validate the token");
            }
            return Ok("Token validated" + validatedtoken);
        }

        [HttpPost("Token Refresh")]
        public async Task<ActionResult<string>> RefreshToken() {
            var refreshToken = Request.Cookies["refreshToken"];
            //add validation for match and expiry
            string token = CreateToken(user);
            var newRefreshToken= generateRefreshToken();
            setRefreshToken(newRefreshToken);
            return Ok(token);
        }
        private string CreateToken(User user) {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.Username)
            };
            var cred = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims:claims,
                expires:DateTime.UtcNow.AddSeconds(1),
                signingCredentials:cred
                );
            var jwt=new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
 
        }

        private RefreshToken generateRefreshToken() {
            var refreshToken = new RefreshToken()
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.Now
            };
            return refreshToken;
        }
        private void setRefreshToken(RefreshToken refreshToken) {
            var cookieOption = new CookieOptions()
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOption);
            user.RefreshToken = refreshToken.Token;
            user.TokenExpires = refreshToken.Expires;
            user.TokenCreated = refreshToken.Created;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash,out byte[] passwordSalt) {
            
            using (var hmac = new HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            //foreach (var value in UserValidationsWithDb.DbFetchEntry(FileSource)) {
            //    var userPasswordSalt = value[2];
            //    using (var hmac = new HMACSHA512(user.PasswordSalt))
            //    {
            //        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            //        if (computedHash.SequenceEqual(passwordHash)) return true;
            //    }
            //}
            //return false;
            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private static TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {                
                ValidateLifetime = true,
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = "Sample",
                ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(SigningKey.Key) // The same key as the one that generate the token
            };
        }

    }
}
