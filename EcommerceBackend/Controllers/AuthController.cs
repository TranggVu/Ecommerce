using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    //API đăng nhập
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)    //async xử lý bất đồng bộ 
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>u.Username == request.Username);
            String HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
            if(user == null || user.Password != HashPassword )
               return Unauthorized();
            // Tạo token cho phiên đăng nhập
            var token = GenerateJwtToken(user);
            // Trả về thông tin người dùng và token
            return OK(new {Token = token, User = user});



        }
    // Phương thức tạo token
    private String GenerateJwtToken(User user){
        var jktKey = _config["Jkt:Key"] ??throw new InvalidOperationException("Jkt key chưa đợc cấu hình");
        var securityKey = new SymmertricsSecurityKey(Econding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]{
                new Claims(JwtKeyRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claims(ClaimsTypes.Role,user.Role)
        };
        var token = new JwtSecurityToken(
            issuer:_config["Jkt:Issuer"],
            audience:_config["Jkt:Audience"],
            claims:claims,
            expires: DateTime.Now.AddMinutes(
                Convert.ToDouble(_config("Jkt:ExpiryInMinutes"))
            ),
            signingCredentials:credentials
        )
        return new JktSecurityTokenHandler().WriteToken(token);
    }
}
public class LoginRequest