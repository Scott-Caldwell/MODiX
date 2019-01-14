using System;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Modix.Services.AutoCodePaste;
using Modix.Services.Coliru;
using Modix.Services.Utilities;

using Serilog;

namespace Modix.Bot.Modules
{
    public class ColiruModule : ModuleBase
    {
        public ColiruModule(IColiruService coliruService, CodePasteService codePasteService)
        {
            ColiruService = coliruService;
            CodePasteService = codePasteService;
        }

        [Command("evalcpp")]
        [Alias("evalc++")]
        [Summary("Compiles and evaluates C++ code.")]
        public async Task EvalCppAsync(
            [Summary("The C++ source code to compile and evaluate.")]
            [Remainder]
            string cppSource)
        {
            var message = await ReplyAsync("Working...");

            var cts = new CancellationTokenSource(30_000);
            string response;

            try
            {
                response = await ColiruService.EvalCppAsync(cppSource, cts.Token);
            }
            catch (TaskCanceledException ex)
            {
                await message.ModifyAsync(x => { x.Content = $"Gave up waiting for a response from the REPL service."; });
                return;
            }
            catch (Exception ex)
            {
                await message.ModifyAsync(a => { a.Content = $"Exec failed: {ex.Message}"; });
                Log.Error(ex, "Exec Failed");
                return;
            }

            var embed = await BuildEmbedAsync(cppSource, response, "cpp");

            await message.ModifyAsync(x =>
            {
                x.Content = string.Empty;
                x.Embed = embed;
            });

            await Context.Message.DeleteAsync();
        }

        protected IColiruService ColiruService { get; }

        protected CodePasteService CodePasteService { get; }

        private async Task<Embed> BuildEmbedAsync(string source, string response, string language)
        {
            var failed = response?.Contains("error:") ?? false;

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Eval Result")
                .WithDescription(failed ? "Failed" : "Successful")
                .WithColor(failed ? new Color(255, 0, 0) : new Color(0, 255, 0))
                .WithAuthor(a => a.WithIconUrl(Context.User.GetAvatarUrl()).WithName(Context.User.Username));

            embedBuilder.AddField(x => x
                .WithName("Code")
                .WithValue(Format.Code(source, language)));

            if (!(response is null))
            {
                embedBuilder.AddField(x => x
                    .WithName("Result")
                    .WithValue(Format.Code(response.TruncateTo(1000))));

                await embedBuilder.UploadToServiceIfBiggerThan(response, "txt", 1000, CodePasteService);
            }

            return embedBuilder.Build();
        }
    }
}
