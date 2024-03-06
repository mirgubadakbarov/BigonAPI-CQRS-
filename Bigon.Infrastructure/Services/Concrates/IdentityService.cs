using Bigon.Infrastructure.Services.Abstracts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace Bigon.Infrastructure.Services.Concrates
{
    public class IdentityService : IIdentityService
    {
        private readonly IActionContextAccessor ctx;

        public IdentityService(IActionContextAccessor ctx)
        {
            this.ctx = ctx;
        }
        public int? GetPrincipalId()
        {
            var userIdStr = ctx.ActionContext.HttpContext.User.Claims.FirstOrDefault(m => m.Type.Equals(ClaimTypes.NameIdentifier))?.Value;

            if (string.IsNullOrWhiteSpace(userIdStr))
                return null;
            
            
            return Convert.ToInt32(userIdStr);
        }
    }
}
