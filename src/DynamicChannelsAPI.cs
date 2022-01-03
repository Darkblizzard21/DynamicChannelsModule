using Discord.WebSocket;
using DNU;

namespace DynamicChannels
{
    public static class Api
    {
        public static void LoadDynamicChannelsModule(DiscordSocketClient client)
        {
            var data = DataUtil.GetData();
            data.SetTextChannelFor(742355539647922246, 742355539647922252, 927321250605461544);
            DataUtil.SaveData();
            
            client.UserVoiceStateUpdated += UserVoiceStateUpdate.UserVoiceStateUpdated;
            SlashCommandUtil.AddCommand(new ConfigDynamicChannel());
        }
    }
}