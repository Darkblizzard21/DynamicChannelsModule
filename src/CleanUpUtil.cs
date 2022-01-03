using Discord;
using Discord.WebSocket;

namespace DynamicChannels
{
    internal static class CleanUpUtil
    {
        public static async void CleanUpPermissions(DiscordSocketClient client)
        {
            var data = DataUtil.GetData().GetData();

            foreach (var (key, value) in data)
            {
                var guild = client.GetGuild(key);
                if (guild == null) continue;
                foreach (var valuePair in value)
                {
                    var channel = guild.GetChannel(valuePair.Value);
                    if (channel == null) continue;
                    //Clean up
                    var overrides = channel.PermissionOverwrites;
                    foreach (var overwrite in overrides)
                    {
                        if (overwrite.TargetType != PermissionTarget.User) continue;
                        var user = guild.GetUser(overwrite.TargetId);
                        if(user.IsBot) continue;
                        await channel.RemovePermissionOverwriteAsync(user);
                    }
                }
            }
        }
    }
}