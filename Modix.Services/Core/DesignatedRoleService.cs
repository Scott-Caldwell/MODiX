using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Modix.Data.Models.Core;
using Modix.Data.Repositories;
using Modix.Services.Messages.Modix;
using Modix.Services.NotificationDispatch;

namespace Modix.Services.Core
{
    /// <summary>
    /// Provides methods for managing and interacting with Discord roles, designated for use within the application.
    /// </summary>
    public interface IDesignatedRoleService
    {
        /// <summary>
        /// Assigns a role to a given designation, within a given guild.
        /// </summary>
        /// <param name="guildId">The Discord snowflake ID of the guild within which the designation is being made</param>
        /// <param name="roleId">The Discord snowflake ID of the role being designated</param>
        /// <param name="type">The type of designation to be made</param>
        /// <returns>A <see cref="Task"/> that will complete when the operation has completed.</returns>
        Task AddDesignatedRoleAsync(ulong guildId, ulong roleId, DesignatedRoleType type);

        /// <summary>
        /// Unassigns a role's previously given designation.
        /// </summary>
        /// <param name="guildId">The Discord snowflake ID of the guild for which a designation is being removed</param>
        /// <param name="roleId">The Discord snowflake ID of the role whose designation is being removed</param>
        /// <param name="type">The type of designation to be removed</param>
        /// <returns>A <see cref="Task"/> that will complete when the operation has completed.</returns>
        Task RemoveDesignatedRoleAsync(ulong guildId, ulong roleId, DesignatedRoleType type);

        /// <summary>
        /// Unassigns a role designation by ID
        /// </summary>
        /// <param name="id">The ID of the assignment to remove</param>
        /// <returns>A <see cref="Task"/> that will complete when the operation has completed.</returns>
        Task RemoveDesignatedRoleByIdAsync(long id);

        /// <summary>
        /// Retrieves the current designated roles, for a given guild.
        /// </summary>
        /// <param name="guildId">The Discord snowflake ID of the guild whose role designations are to be retrieved.</param>
        /// <returns>
        /// A <see cref="Task"/> that will complete when the operation has completed,
        /// containing the requested role designations.
        /// </returns>
        Task<IReadOnlyCollection<DesignatedRoleMappingBrief>> GetDesignatedRolesAsync(ulong guildId);

        /// <summary>
        /// Retrieves designated roles, based on an arbitrary set of search criteria.
        /// </summary>
        /// <param name="searchCriteria">The search criteria, defining the role designations to be retrieved.</param>
        /// <returns>
        /// A <see cref="Task"/> that will complete when the operation has completed,
        /// containing the requested role designations.
        /// </returns>
        Task<IReadOnlyCollection<DesignatedRoleMappingBrief>> SearchDesignatedRolesAsync(DesignatedRoleMappingSearchCriteria searchCriteria);

        /// <summary>
        /// Checks if the given role has the given designation
        /// </summary>
        /// <param name="guild">The Id of the guild where the role is located</param>
        /// <param name="channel">The Id of the role to check the designation for</param>
        /// <param name="designation">The <see cref="DesignatedRoleType"/> to check for</param>
        Task<bool> RoleHasDesignationAsync(ulong guildId, ulong roleId, DesignatedRoleType designation);
    }

    public class DesignatedRoleService : IDesignatedRoleService
    {
        public DesignatedRoleService(
            IAuthorizationService authorizationService,
            IDesignatedRoleMappingRepository designatedRoleMappingRepository,
            INotificationDispatchService notificationDispatchService)
        {
            AuthorizationService = authorizationService;
            DesignatedRoleMappingRepository = designatedRoleMappingRepository;
            NotificationDispatchService = notificationDispatchService;
        }

        /// <inheritdoc />
        public async Task AddDesignatedRoleAsync(ulong guildId, ulong roleId, DesignatedRoleType type)
        {
            AuthorizationService.RequireAuthenticatedUser();
            AuthorizationService.RequireClaims(AuthorizationClaim.DesignatedRoleMappingCreate);

            using (var transaction = await DesignatedRoleMappingRepository.BeginCreateTransactionAsync())
            {
                if (await DesignatedRoleMappingRepository.AnyAsync(new DesignatedRoleMappingSearchCriteria()
                {
                    GuildId = guildId,
                    RoleId = roleId,
                    Type = type,
                    IsDeleted = false
                }))
                    throw new InvalidOperationException($"Role {roleId} already has a {type} designation");

                var mapping = await DesignatedRoleMappingRepository.CreateAsync(new DesignatedRoleMappingCreationData()
                {
                    GuildId = guildId,
                    RoleId = roleId,
                    Type = type,
                    CreatedById = AuthorizationService.CurrentUserId.Value
                });

                transaction.Commit();

                await NotificationDispatchService.PublishScopedAsync(new DesignatedRoleMappingAdded()
                {
                    GuildId = guildId,
                    DesignatedRoleMapping = mapping,
                });
            }
        }

        /// <inheritdoc />
        public async Task RemoveDesignatedRoleAsync(ulong guildId, ulong roleId, DesignatedRoleType type)
        {
            AuthorizationService.RequireAuthenticatedUser();
            AuthorizationService.RequireClaims(AuthorizationClaim.DesignatedRoleMappingDelete);

            using (var transaction = await DesignatedRoleMappingRepository.BeginDeleteTransactionAsync())
            {
                var criteria = new DesignatedRoleMappingSearchCriteria()
                {
                    GuildId = guildId,
                    RoleId = roleId,
                    Type = type,
                    IsDeleted = false
                };

                var mappings = await DesignatedRoleMappingRepository.SearchBriefsAsync(criteria);

                var deletedCount = await DesignatedRoleMappingRepository.DeleteAsync(criteria, AuthorizationService.CurrentUserId.Value);

                if (deletedCount == 0)
                    throw new InvalidOperationException($"Role {roleId} does not have a {type} designation");

                transaction.Commit();

                foreach (var mapping in mappings)
                {
                    await NotificationDispatchService.PublishScopedAsync(new DesignatedRoleMappingAdded()
                    {
                        GuildId = guildId,
                        DesignatedRoleMapping = mapping,
                    });
                }
            }
        }

        /// <inheritdoc />
        public async Task RemoveDesignatedRoleByIdAsync(long id)
        {
            AuthorizationService.RequireAuthenticatedUser();
            AuthorizationService.RequireClaims(AuthorizationClaim.DesignatedRoleMappingDelete);

            using (var transaction = await DesignatedRoleMappingRepository.BeginDeleteTransactionAsync())
            {
                var criteria = new DesignatedRoleMappingSearchCriteria()
                {
                    Id = id,
                    IsDeleted = false
                };

                var mappings = await DesignatedRoleMappingRepository.SearchBriefsAsync(criteria);

                var deletedCount = await DesignatedRoleMappingRepository.DeleteAsync(criteria, AuthorizationService.CurrentUserId.Value);

                if (deletedCount == 0)
                    throw new InvalidOperationException($"No role assignment exists with id {id}");

                transaction.Commit();

                foreach (var mapping in mappings)
                {
                    await NotificationDispatchService.PublishScopedAsync(new DesignatedRoleMappingAdded()
                    {
                        GuildId = AuthorizationService.CurrentGuildId.Value,
                        DesignatedRoleMapping = mapping,
                    });
                }
            }
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<DesignatedRoleMappingBrief>> GetDesignatedRolesAsync(ulong guildId)
        {
            AuthorizationService.RequireClaims(AuthorizationClaim.DesignatedRoleMappingRead);

            return DesignatedRoleMappingRepository.SearchBriefsAsync(new DesignatedRoleMappingSearchCriteria()
            {
                GuildId = guildId,
                IsDeleted = false
            });
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<DesignatedRoleMappingBrief>> SearchDesignatedRolesAsync(DesignatedRoleMappingSearchCriteria searchCriteria)
            => DesignatedRoleMappingRepository.SearchBriefsAsync(searchCriteria);

        public async Task<bool> RoleHasDesignationAsync(ulong guildId, ulong roleId, DesignatedRoleType designation)
        {
            return (await SearchDesignatedRolesAsync(new DesignatedRoleMappingSearchCriteria
            {
                GuildId = guildId,
                RoleId = roleId,
                IsDeleted = false,
                Type = designation
            })).Any();
        }

        /// <summary>
        /// A <see cref="IAuthorizationService"/> to be used to perform authorization.
        /// </summary>
        internal protected IAuthorizationService AuthorizationService { get; }

        /// <summary>
        /// An <see cref="IDesignatedRoleMappingRepository"/> for storing and retrieving role designation data.
        /// </summary>
        internal protected IDesignatedRoleMappingRepository DesignatedRoleMappingRepository { get; }

        /// <summary>
        /// A service to handle dispatching notifications.
        /// </summary>
        internal protected INotificationDispatchService NotificationDispatchService { get; }
    }
}
