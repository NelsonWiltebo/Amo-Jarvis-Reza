using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Discord_Bot
{
    public class Commands : ApplicationCommandModule
    {
        #region RandomImage
        [SlashCommand("randomimage", "Visa en random bild från Örsk Army")]
        public async Task RandomImageAsync(InteractionContext ctx)
        {
            Random rand = new Random();
            var files = Directory.GetFiles("H:\\My Drive\\Discord Bot\\Images");
            FileStream file = new FileStream(files[rand.Next(0, files.Length)].ToString(), FileMode.Open);

            var buttonbuilder = new DiscordInteractionResponseBuilder()
                .AddFile(file)
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, "button1", "Ny bild")
                });

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, buttonbuilder);

            Program.client.ComponentInteractionCreated += async (sender, e) =>
            {
                if (e.Interaction.Data.CustomId == "button1")
                {
                    file = new FileStream(files[rand.Next(0, files.Length)].ToString(), FileMode.Open);
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .AddFile(file)
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(ButtonStyle.Primary, "button1", "Ny bild")
                    }));
                }
            };

        }

        #endregion

        #region UploadImage
        [SlashCommand("uploadimage", "Ladda upp en bild till Örsk Army")]
        public async Task UploadImageAsync(InteractionContext ctx, [Option("image", "Ladda upp bild")] DiscordAttachment image)
        {
            WebClient webClient = new WebClient();
            bool hasException = false;

            var message = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Red)
                .WithTitle("temp")
                .WithImageUrl(image.Url);

            try
            {
                if (image != null)
                    webClient.DownloadFile(image.Url, $"H:\\My Drive\\Discord Bot\\Images\\{image.FileName}");
                else
                {
                    hasException = true;
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("Upload unsuccessful! No image attached to command message."));
                }
            }
            catch (Exception)
            {
                message.WithTitle($"Upload of '{image.FileName}' was unsuccessful!");

                hasException = true;
            }
            finally
            {
                if (!hasException)
                {
                    message.WithTitle($"Upload of '{image.FileName}' was successful!")
                            .WithColor(DiscordColor.Green);
                }
            }
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(message));

        }
        #endregion
    }
}
