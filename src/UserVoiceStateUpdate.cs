using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DynamicChannels
{
    internal static class UserVoiceStateUpdate
    {
        internal static async Task UserVoiceStateUpdated(
            SocketUser user,
            SocketVoiceState before,
            SocketVoiceState after)
        {
            if (before.VoiceChannel == null || after.VoiceChannel == null ||
                !before.VoiceChannel.Equals(after.VoiceChannel))
            {
                var data = DataUtil.GetData();
                if (before.VoiceChannel != null &&
                    data.TryGetTextChannelFor(
                        before.VoiceChannel.Guild.Id,
                        before.VoiceChannel.Id,
                        out var oldChannelTextId))
                {
                    var channel = before.VoiceChannel.Guild.GetChannel(oldChannelTextId);
                    await channel.RemovePermissionOverwriteAsync(user);
                }

                if (after.VoiceChannel != null &&
                    data.TryGetTextChannelFor(
                        after.VoiceChannel.Guild.Id,
                        after.VoiceChannel.Id,
                        out var newChannelTextId))
                {
                    var channel = after.VoiceChannel.Guild.GetChannel(newChannelTextId);
                    await channel.AddPermissionOverwriteAsync(
                        user,
                        new OverwritePermissions().Modify(viewChannel: PermValue.Allow)
                    );
                }
            }
        }
    }
}