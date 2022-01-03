using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DNU;

namespace DynamicChannels
{
    public class ConfigDynamicChannel : SlashCommand
    {
        public override string Name => "config_dynamic_channels";
        public override string Description => "Foo Bar";
        public override bool IsGlobal => false;

        public override Optional<List<SlashCommandOptionBuilder>> GetOptions()
        {
            return new Optional<List<SlashCommandOptionBuilder>>(new List<SlashCommandOptionBuilder>()
            {
                new SlashCommandOptionBuilder()
                    .WithName("operation")
                    .WithDescription("type of modification you want to make.")
                    .WithRequired(true)
                    .AddChoice("Set", 0)
                    .AddChoice("Remove", 1)
                    .AddChoice("Info", 2)
                    .WithType(ApplicationCommandOptionType.Integer),
                new SlashCommandOptionBuilder()
                    .WithName("dynamic")
                    .WithDescription("text channel that should become the new dynamic channel.")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Channel)
            });
        }

        public override async Task HandleCommand(SocketSlashCommand command)
        {
            var options = command.Data.Options.ToList();
            if (!(command.Channel is SocketGuildChannel channel))
            {
                await command.RespondAsync($"How did you invoke this in a global channel?");
                return;
            }

            var user = command.User as SocketGuildUser;
            if (!await command.CheckAdmin(user))
                return;


            var targetChannel = (IGuildChannel) options[1].Value;
            switch ((long) options[0].Value)
            {
                case 0:
                    //set 
                    if (targetChannel is ITextChannel text)
                    {
                        var voice = user?.VoiceChannel;
                        if (voice == null)
                        {
                            await command.RespondAsync("You need to be in a voice channel you want to bind.");
                        }
                        else
                        {
                            DataUtil.GetData().SetTextChannelFor(targetChannel.GuildId,voice.Id,text.Id);
                            await command.RespondAsync($"-{targetChannel.GetMentionOrName()} is dynamic channel" +
                                $" for {voice.GetMentionOrName()} \n");
                            DataUtil.SaveData();
                        }
                    }
                    else
                    {
                        await command.RespondAsync($"{targetChannel.GetMentionOrName()} is not a text channel.");
                    }
                    break;
                case 1:
                    //Remove
                    var res = DataUtil.GetData().TryDeleteChannelFor(targetChannel.GuildId, targetChannel.Id);
                    if (res)
                    {
                        await command.RespondAsync($"{targetChannel.GetMentionOrName()} is no longer a dynamic channel");
                    }
                    else
                    {
                        await command.RespondAsync($"Something went wrong.");
                    }

                    break;
                case 2:
                    //Info
                    var embedBuilder = new EmbedBuilder()
                        .WithTitle("Overview");
                    if (DataUtil.GetData().TryGetGuild(targetChannel.GuildId, out var pairs) && pairs.Count != 0)
                    {
                        var str = new StringBuilder();
                        foreach (var (item1, item2) in pairs)
                        {
                            str.Append(
                                $"-{(await targetChannel.Guild.GetChannelAsync(item2)).GetMentionOrName()} is dynamic channel");
                            str.Append(
                                $" for {(await targetChannel.Guild.GetChannelAsync(item1)).GetMentionOrName()} \n");
                        }

                        embedBuilder
                            .WithDescription(str.ToString())
                            .WithColor(Color.Green);
                    }
                    else
                    {
                        embedBuilder
                            .WithDescription("There are no dynamicChannels on this server")
                            .WithColor(Color.Red);
                    }

                    await command.RespondAsync(embed: embedBuilder.Build());
                    break;
            }
        }
    }
}