// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Utils;
using osu.Game.Audio;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Tests.Beatmaps;

namespace osu.Game.Rulesets.Mania.Tests
{
    [TestFixture]
    public class ManiaBeatmapSampleConversionTest : BeatmapConversionTest<ConvertMapping<SampleConvertValue>, SampleConvertValue>
    {
        protected override string ResourceAssembly => "osu.Game.Rulesets.Mania";

        public void Test(string name) => base.Test(name);

        protected override IEnumerable<SampleConvertValue> CreateConvertValue(HitObject hitObject)
        {
            yield return new SampleConvertValue
            {
                StartTime = hitObject.StartTime,
                EndTime = hitObject.GetEndTime(),
                Column = ((ManiaHitObject)hitObject).Column,
                NodeSamples = getSampleNames((hitObject as HoldNote)?.NodeSamples)
            };
        }

        private IList<IList<string>> getSampleNames(List<IList<HitSampleInfo>> hitSampleInfo)
            => hitSampleInfo?.Select(samples =>
                                (IList<string>)samples.Select(sample => sample.LookupNames.First()).ToList())
                            .ToList();

        protected override Ruleset CreateRuleset() => new ManiaRuleset();
    }

    public struct SampleConvertValue : IEquatable<SampleConvertValue>
    {
        /// <summary>
        /// A sane value to account for osu!stable using ints everywhere.
        /// </summary>
        private const float conversion_lenience = 2;

        public double StartTime;
        public double EndTime;
        public int Column;
        public IList<IList<string>> NodeSamples;

        public bool Equals(SampleConvertValue other)
            => Precision.AlmostEquals(StartTime, other.StartTime, conversion_lenience)
               && Precision.AlmostEquals(EndTime, other.EndTime, conversion_lenience)
               && samplesEqual(NodeSamples, other.NodeSamples);

        private static bool samplesEqual(ICollection<IList<string>> first, ICollection<IList<string>> second)
        {
            if (first == null && second == null)
                return true;

            // both items can't be null now, so if any single one is, then they're not equal
            if (first == null || second == null)
                return false;

            return first.Count == second.Count
                   && first.Zip(second).All(samples => samples.First.SequenceEqual(samples.Second));
        }
    }
}
