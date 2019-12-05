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
    internal class OffsetValueNA : Attribute
    {
        public int Value;

        public OffsetValueNA(int val)
        {
            Value = val;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class OffsetValueCN : Attribute
    {
        public int Value;

        public OffsetValueCN(int val)
        {
            Value = val;
        }
    }
}