using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Modix.Services.Utilities;
using Newtonsoft.Json;

namespace Modix.Services.Coliru
{
    /// <summary>
    /// This service provides the ability to compile and evaluate C++, C, Python, and Haskell code.
    /// </summary>
    public interface IColiruService
    {
        /// <summary>
        /// Compiles and evaluates C++ code using the Coliru API.
        /// </summary>
        /// <param name="cppSource">The C++ source code to be compiled and evaluated.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>
        /// A <see cref="Task"/> that completes when the operation has completed,
        /// containing the result of the compilation and evaluation.
        /// </returns>
        Task<string> EvalCppAsync(string cppSource, CancellationToken cancellationToken);
    }

    internal class ColiruService : IColiruService
    {
        /// <inheritdoc />
        public async Task<string> EvalCppAsync(string cppSource, CancellationToken cancellationToken)
        {
            var content = FormatUtilities.StripFormatting(cppSource);

            var request = new ColiruRequest()
            {
                Command = "g++ main.cpp && ./a.out",
                Source = content,
            };

            var requestJson = JsonConvert.SerializeObject(request);

            var response = await Client.PostAsync(Endpoint, FormatUtilities.BuildContent(requestJson), cancellationToken);

            var responseData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new WebException($"Error encountered while querying the Coliru API: {responseData}");
            }

            return responseData;
        }

        /// <summary>
        /// An HTTP client to use to query the Coliru API endpoint.
        /// </summary>
        protected static HttpClient Client { get; } = new HttpClient();

        protected static string Endpoint { get; } = "http://coliru.stacked-crooked.com/compile";
    }
}
