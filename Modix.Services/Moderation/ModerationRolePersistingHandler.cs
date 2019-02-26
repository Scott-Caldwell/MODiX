using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modix.Services.Messages.Discord;

namespace Modix.Services.Moderation
{
    /// <summary>
    /// Implements a handler that persists infraction-related roles on users who rejoin the guild.
    /// </summary>
    public sealed class ModerationRolePersistingHandler
        : INotificationHandler<UserJoined>
    {
        public ModerationRolePersistingHandler(IModerationService moderationService)
        {
            _moderationService = moderationService;
        }

        public async Task Handle(UserJoined notification, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await _moderationService.AutoPersistRolesAsync(notification.User);
        }

        private readonly IModerationService _moderationService;
    }
}
