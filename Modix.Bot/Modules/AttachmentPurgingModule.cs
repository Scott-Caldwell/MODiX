using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Modix.Services.Moderation;

namespace Modix.Modules
{
    [Name("Attachment Purging")]
    [Summary("Retrieve information related to the attachment purging functionality.")]
    public class AttachmentPurgingModule : ModuleBase
    {
        [Command("attachment whitelist")]
        [Summary("Retrieves the list of whitelisted attachment file extensions.")]
        public async Task GetWhitelistAsync()
        {
            var blacklistBuilder = new StringBuilder()
                .AppendLine($"{Format.Bold("Blacklisted Extensions")}:")
                .Append("```")
                .AppendJoin(", ", AttachmentPurgingBehavior.WhitelistedExtensions.OrderBy(d => d))
                .Append("```");

            await ReplyAsync(blacklistBuilder.ToString());
        }
    }
}
