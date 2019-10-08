using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace Modix.Services.Moderation
{
    public class AttachmentPurgingBehavior : BehaviorBase
    {
        public static readonly IReadOnlyCollection<string> WhitelistedExtensions = new[]
        {
            ".bmp",
            ".cs",
            ".fs",
            ".gif",
            ".gifv",
            ".jpeg",
            ".jpg",
            ".json",
            ".log",
            ".mp4",
            ".pdf",
            ".png",
            ".svg",
            ".tif",
            ".tiff",
            ".txt",
            ".vb",
            ".xaml",
            ".xml",
            ".yaml",
            ".yml",
        };

        private DiscordSocketClient DiscordClient { get; }

        public AttachmentPurgingBehavior(DiscordSocketClient discordClient, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            DiscordClient = discordClient;
        }

        private async Task Handle(IMessage message)
        {
            if (!message.Attachments.Any())
                return;

            if (!(message is IUserMessage userMessage) || !(userMessage.Author is IGuildUser || userMessage.Author.IsBot))
                return;

            // Ensure that the attachments are whitelisted
            if (message.Attachments.All(x => WhitelistedExtensions.Any(ext => x.Filename.EndsWith(ext, StringComparison.OrdinalIgnoreCase))))
                return;

            await TryDeleteAndReplyToUser(message);
        }

        private async Task TryDeleteAndReplyToUser(IMessage message)
        {
            try
            {
                var attachments = string.Join(", ", message.Attachments.Select(x => x.Filename));

                await SelfExecuteRequest<IModerationService>(async moderationService =>
                {
                    await moderationService.DeleteMessageAsync(message, $"Message had suspicious files attached: {attachments}", DiscordClient.CurrentUser.Id);
                });

                var reply = GetReplyToUser(message);
                await message.Channel.SendMessageAsync(reply);
            }
            catch (Exception e)
            {
                Log.Warning(e, "Failed to remove message {messageId} with suspicious file(s) attached in {channelName}", message.Id, message.Channel.Name);
            }
        }

        private static string GetReplyToUser(IMessage message)
            => $"Please don't upload any potentially harmful files {message.Author.Mention}, your message has been removed";

        internal protected override Task OnStartingAsync()
        {
            DiscordClient.MessageReceived += Handle;
            return Task.CompletedTask;
        }

        internal protected override Task OnStoppedAsync()
        {
            DiscordClient.MessageReceived -= Handle;
            return Task.CompletedTask;
        }
    }
}
