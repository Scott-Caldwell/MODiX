using MediatR;
using Modix.Data.Models.Core;

namespace Modix.Services.Messages.Modix
{
    public class DesignatedRoleMappingAdded : INotification
    {
        public ulong GuildId { get; set; }

        public DesignatedRoleMappingBrief DesignatedRoleMapping { get; set; }
    }
}
