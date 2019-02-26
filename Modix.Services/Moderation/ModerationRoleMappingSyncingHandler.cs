using System.Threading;
using System.Threading.Tasks;
using Discord;
using MediatR;
using Modix.Services.Messages.Modix;

namespace Modix.Services.Moderation
{
    /// <summary>
    /// Implements a handler that synchronizes role configurations when a designated role mapping is added or removed.
    /// </summary>
    public sealed class ModerationRoleMappingSyncingHandler :
        INotificationHandler<DesignatedRoleMappingAdded>,
        INotificationHandler<DesignatedRoleMappingRemoved>
    {
        public ModerationRoleMappingSyncingHandler(
            IDiscordClient discordClient,
            IModerationService moderationService)
        {
            _discordClient = discordClient;
            _moderationService = moderationService;
        }

        public async Task Handle(DesignatedRoleMappingAdded notification, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var guild = await _discordClient.GetGuildAsync(notification.GuildId);

            await _moderationService.AutoConfigureGuildAsync(guild);
        }

        public async Task Handle(DesignatedRoleMappingRemoved notification, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var guild = await _discordClient.GetGuildAsync(notification.GuildId);

            await _moderationService.AutoConfigureGuildAsync(guild);
        }

        private readonly IDiscordClient _discordClient;
        private readonly IModerationService _moderationService;
    }
}
