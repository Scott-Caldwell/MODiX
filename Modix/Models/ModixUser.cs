using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Discord;
using Modix.Services.Utilities;

namespace Modix.Models
{
    public class ModixUser
    {
        public string Name { get; set; }

        public string UserId { get; set; }

        public string AvatarHash { get; set; }

        public List<string> Claims { get; set; }

        public string SelectedGuild { get; set; }

        public ulong GetUserIdAsUInt64()
            => ulong.Parse(UserId);

        public ulong GetGuildIdAsUInt64()
            => ulong.Parse(SelectedGuild);

        public static ModixUser FromClaimsPrincipal(ClaimsPrincipal user)
        {
            if (user?.Identity?.Name == null) { return null; }

            var ret = new ModixUser
            {
                Name = user.Identity.Name,
                UserId = user.Claims.FirstOrDefault(d => d.Type == ClaimTypes.NameIdentifier).Value,
                Claims = user.Claims.Where(d => d.Type == ClaimTypes.Role).Select(d => d.Value).ToList()
            };

            return ret;
        }

        public static ModixUser FromIGuildUser(IGuildUser user)
        {
            var ret = new ModixUser
            {
                Name = user.GetFullUsername(),
                UserId = user.Id.ToString(),
                AvatarHash = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()
            };

            return ret;
        }
    }
}
