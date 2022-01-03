using System.Threading.Tasks;
using Discord.WebSocket;
using DNU;

namespace DynamicChannels
{
    public static class Api
    {
        public static void LoadDynamicChannelsModule(DiscordSocketClient client)
        {
            _client = client;
            client.GuildAvailable += CleanUp;
            client.UserVoiceStateUpdated += UserVoiceStateUpdate.UserVoiceStateUpdated;
            SlashCommandUtil.AddCommand(new ConfigDynamicChannel());
        }

        private static DiscordSocketClient _client;

        private static Task CleanUp(SocketGuild guild)
        {
            CleanUpUtil.CleanUpPermissions(guild);
            _client = null;
            return Task.CompletedTask;
        }
        
    }
}