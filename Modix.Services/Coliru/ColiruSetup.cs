using Microsoft.Extensions.DependencyInjection;

namespace Modix.Services.Coliru
{
    /// <summary>
    /// Contains extension methods for configuring the Coliru feature upon application startup.
    /// </summary>
    public static class ColiruSetup
    {
        /// <summary>
        /// Adds the services and classes that make up the Moderation feature to a service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the Coliru services are to be added.</param>
        /// <returns><paramref name="services"/></returns>
        public static IServiceCollection AddColiru(this IServiceCollection services)
            => services
                .AddScoped<IColiruService, ColiruService>();
    }
}
