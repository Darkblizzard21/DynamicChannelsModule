using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DynamicChannels
{
    internal static class DataUtil
    {
        private static Data _data;
        private static readonly string Path = Environment.CurrentDirectory + @"\data.json";

        public static Data GetData()
        {
            if (_data != null)
                return _data;
            try
            {
                using var r = new StreamReader(Path);
                var json = r.ReadToEnd();
                var deserializeObject = JsonConvert.DeserializeObject<Data>(json);
                _data = deserializeObject;
            }
            catch (Exception)
            {
                _data = new Data();
            }

            return _data;
        }

        public static bool SaveData()
        {
            if (_data == null) return false;

            var json = JsonConvert.SerializeObject(_data);
            File.WriteAllText(Path, json);
            return true;
        }
    }

    internal class Data
    {
        public Data()
        {
            _data = new Dictionary<ulong, Dictionary<ulong, ulong>>();
        }

        [JsonProperty] private readonly Dictionary<ulong, Dictionary<ulong, ulong>> _data;

        public bool TryGetTextChannelFor(ulong guildId, ulong channelId, out ulong textId)
        {
            textId = 0;
            if (!_data.TryGetValue(guildId, out var dictionary)) return false;
            if (!dictionary.TryGetValue(channelId, out var id)) return false;
            textId = id;
            return true;
        }

        public void SetTextChannelFor(ulong guildId, ulong channelId, ulong textId)
        {
            if (_data.TryGetValue(guildId, out var dictionary))
            {
                if (dictionary.ContainsKey(channelId))
                {
                    dictionary.Remove(channelId);
                }

                dictionary.Add(channelId, textId);
            }
            else
            {
                _data.Add(guildId, new Dictionary<ulong, ulong> {{channelId, textId}});
            }
        }

        public bool TryDeleteChannelFor(ulong guildId, ulong textId)
        {
            if (!_data.TryGetValue(guildId, out var dictionary)) return false;
            return dictionary.ContainsValue(textId) &&
                   dictionary.Remove(dictionary.Keys.First(key => dictionary[key] == textId));
        }

        public bool TryGetGuild(ulong guildId, out List<(ulong, ulong)> pairs)
        {
            pairs = null;
            if (!_data.TryGetValue(guildId, out var dictionary)) return false;
            pairs = dictionary.Keys.Zip(dictionary.Values).ToList();
            return true;
        }
    }
}