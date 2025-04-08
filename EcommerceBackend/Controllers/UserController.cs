
using Microsoft.AspNetcore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;


namespace Controllers{
   [ApiController]
   [Route("api/users")]
   public class UserController : ControllerBase
   {
     private readonly AppDbContext _context;
     public UserController (AppDbContext context)
     {
        _context = context;
     }
     //tao moi nguoi dung (user)
     [HttpPost]
     public async Task<ActionResult<User>> CreateUser(
     [FromBody] CreateUserRequest request)
        {
             // kiem tra xem username da ton tai chua
             if(await _context.Users.AnyAsync(
                u =>u.Username == request.Username
             ))
               return BadRequest("Username da ton tai ");
            var user = new User{
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Role = "Nhan vien",
                Phone = request.Phone,
                IsActive = true
             };
             _context.Users.Add(user);
             await _context.SaveChangesAsync();
             return CreateAction(nameof(GetUser),
                                    new{id= user.Id},user);
             
             
        }
        //lay nguoi dung
        [HttpGet("{Id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user == null)
             { return NotFound();}
            return user;

        }
     
   }
   public class CreateUserRequest{
    public string Username{get; set;}
    public string Password{get; set;}
    public string FullName{get; set;}
    public string Phone{get; set;}
    public string Username{get; set;}

   }
}