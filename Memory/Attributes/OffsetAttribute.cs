﻿/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;

namespace LlamaLibrary.Memory.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class OffsetAttribute : Attribute
    {
        public bool Numeric;
        public string Pattern;
        public string PatternCN;

        public OffsetAttribute(string pattern, bool numeric = false, int expectedValue = 0)
        {
            Pattern = pattern;
            PatternCN = pattern;
            Numeric = numeric;
        }

        public OffsetAttribute(string pattern, string cnpattern, bool numeric = false, int expectedValue = 0)
        {
            Pattern = pattern;
            PatternCN = cnpattern;
            Numeric = numeric;
        }
    }

    internal class OffsetCNAttribute : OffsetAttribute
    {
        public OffsetCNAttribute(string pattern, bool numeric = false, int expectedValue = 0) : base("", pattern, numeric, expectedValue)
        {
        }
    }
}