// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text;
using osu.Framework.Extensions;
using osu.Framework.Logging;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Osu;

namespace composer.Editor.Tests.Resources
{
    public static class TestResources
    {
        private static int importId;
        
        /// <summary>
        /// Create a test beatmap set model.
        /// </summary>
        /// <param name="difficultyCount">Number of difficulties. If null, a random number between 1 and 20 will be used.</param>
        /// <param name="rulesets">Rulesets to cycle through when creating difficulties. If <c>null</c>, osu! ruleset will be used.</param>
        public static BeatmapSetInfo CreateTestBeatmapSetInfo(int? difficultyCount = null, RulesetInfo[]? rulesets = null!)
        {
            var j = 0;

            rulesets ??= new[] { new OsuRuleset().RulesetInfo };

            RulesetInfo getRuleset() => rulesets[j++ % rulesets.Length];

            var setId = Interlocked.Increment(ref importId);

            var metadata = new BeatmapMetadata
            {
                // Create random metadata, then we can check if sorting works based on these
                Artist = "Some Artist " + RNG.Next(0, 9),
                Title = $"Some Song (set id {setId:000}) {Guid.NewGuid()}",
                Author = { Username = "Some Guy " + RNG.Next(0, 9) },
            };

            Logger.Log($"🛠️ Generating beatmap set \"{metadata}\" for test consumption.");

            var beatmapSet = new BeatmapSetInfo
            {
                OnlineID = setId,
                Hash = new MemoryStream(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).ComputeMD5Hash(),
                DateAdded = DateTimeOffset.UtcNow,
            };

            foreach (var b in getBeatmaps(difficultyCount ?? RNG.Next(1, 20)))
                beatmapSet.Beatmaps.Add(b);

            return beatmapSet;

            IEnumerable<BeatmapInfo> getBeatmaps(int count)
            {
                for (var i = 0; i < count; i++)
                {
                    var beatmapId = setId * 1000 + i;

                    var length = RNG.Next(30000, 200000);
                    double bpm = RNG.NextSingle(80, 200);

                    var diff = (float) i / count * 10;

                    var version = "Normal";
                    if (diff > 6.6)
                        version = "Insane";
                    else if (diff > 3.3)
                        version = "Hard";

                    var rulesetInfo = getRuleset();

                    var hash = Guid.NewGuid().ToString().ComputeMD5Hash();

                    yield return new BeatmapInfo
                    {
                        OnlineID = beatmapId,
                        DifficultyName = $"{version} {beatmapId} (length {TimeSpan.FromMilliseconds(length):m\\:ss}, bpm {bpm:0.#})",
                        StarRating = diff,
                        Length = length,
                        BeatmapSet = beatmapSet,
                        BPM = bpm,
                        Hash = hash,
                        MD5Hash = hash,
                        Ruleset = rulesetInfo,
                        Metadata = metadata.DeepClone(),
                        Difficulty = new BeatmapDifficulty
                        {
                            OverallDifficulty = diff,
                        }
                    };
                }
            }
        }
    }
}
