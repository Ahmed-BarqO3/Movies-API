using System.Security.Claims;

namespace Movies.Api.Auth;

public static class IdentityExtension
{
    public static Guid? GetUserId(this HttpContext context)
    {
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "userid")?.Value;
        if(Guid.TryParse(userId,out Guid  id))
        {
            return id;
        }

        return null;
    }
}
