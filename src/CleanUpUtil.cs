using Discord;
using Discord.WebSocket;

namespace DynamicChannels
{
    internal static class CleanUpUtil
    {
        public static async void CleanUpPermissions(SocketGuild guild)
        {
            var data = DataUtil.GetData().GetData();

            if (!data.TryGetValue(guild.Id, out var value))
                return;

            //await guild.DownloadUsersAsync();
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
                    if (user.IsBot) continue;
                    await channel.RemovePermissionOverwriteAsync(user);
                }
            }
        }
    }
}