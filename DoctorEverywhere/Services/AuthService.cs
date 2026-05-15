using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DoctorEverywhere.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            
        }
        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
               throw new UnauthorizedAccessException();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            return token;
        }

        public async Task RegisterDoctor(RegisterDoctorDto registerDoctorDto)
        {
            var user = new ApplicationUser { UserName = registerDoctorDto.Username };
            var result = await _userManager.CreateAsync(user, registerDoctorDto.Password);

            if (!result.Succeeded)
                throw new EntityNotCreatedException("Failed to create doctor.");
            await _userManager.AddToRoleAsync(user, "Doctor");

            var doctorProfile = new Doctor
            {
                ApplicationUserId = user.Id,
                FirstName = registerDoctorDto.FirstName,
                LastName = registerDoctorDto.LastName,
                Specialty = registerDoctorDto.Specialty,
                Office = new Office
                {
                    Name = registerDoctorDto.OfficeName,
                    Address = registerDoctorDto.OfficeAddress,
                    City = registerDoctorDto.OfficeCity,
                    PostalCode = registerDoctorDto.OfficePostalCode,
                    Latitude = registerDoctorDto.Latitude,
                    Longitude = registerDoctorDto.Longitude
                }
            };

            _context.Doctors.Add(doctorProfile);
            await _context.SaveChangesAsync();
        }

        public async Task RegisterPatient(RegisterPatientDto registerPatientDto)
        {
            var user = new ApplicationUser { UserName = registerPatientDto.Username };
            var result = await _userManager.CreateAsync(user, registerPatientDto.Password);

            if (!result.Succeeded)
                throw new EntityNotCreatedException("Failed to create patient.");

            await _userManager.AddToRoleAsync(user, "Patient");

            var patientProfile = new Patient
            {
                ApplicationUserId = user.Id,
                FirstName = registerPatientDto.FirstName,
                LastName = registerPatientDto.LastName
            };

            _context.Patients.Add(patientProfile);
            await _context.SaveChangesAsync();
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            // Standard claims
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), // The ApplicationUserId
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // Add Role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
