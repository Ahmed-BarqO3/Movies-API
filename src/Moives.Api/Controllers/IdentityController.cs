using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Movies.Contracts.Requsets;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Movies.Api.Controllers;

[ApiController]
public class IdentityController: ControllerBase
{
    private IConfiguration config;

    public IdentityController(IConfiguration config)
    {
        this.config = config;
    }


    [HttpPost("token")] 
    public  async Task<IActionResult> GenerateToken([FromBody] TokenGenerationRequest request)
    {
        var token = new JsonWebTokenHandler();
        var key = config["Jwt:Key"];
        
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, request.email),
            new (JwtRegisteredClaimNames.Sub, request.email),
            new("userid",request.userId.ToString())
        };

        foreach (var claim in request.CustomClaims)
        {
            var jsonElement = (JsonElement)claim.Value;
            var value = jsonElement.ValueKind switch
            {
                JsonValueKind.Number => ClaimValueTypes.Double,
                JsonValueKind.True => ClaimValueTypes.Boolean,
                JsonValueKind.False => ClaimValueTypes.Boolean,
                _ => ClaimValueTypes.String
            };
            
            claims.Add(new Claim(claim.Key, claim.Value.ToString()!, value));
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"],
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        return Ok(token.CreateToken(tokenDescriptor));
    }
}
