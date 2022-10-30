#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Windows.UI.Xaml.Data;
using Inventory.Uwp.Library.Common;

namespace Inventory.Uwp.Library.Extensions
{
    public static partial class ItemIndexRangeExtensions
    {
        public static int IndexCount(this IEnumerable<ItemIndexRange> ranges)
        {
            return ranges.Normalize().Sum(r => (int)r.Length);
        }

        public static IList<ItemIndexRange> Normalize(this IList<ItemIndexRange> ranges)
        {
            return Normalize((IEnumerable<ItemIndexRange>)ranges).ToList();
        }
        public static IEnumerable<ItemIndexRange> Normalize(this IEnumerable<ItemIndexRange> ranges)
        {
            if (ranges.Any())
            {
                return Merge(ranges.Skip(1), ranges.First());
            }
            return Enumerable.Empty<ItemIndexRange>();
        }

        public static IList<ItemIndexRange> Merge(this IList<ItemIndexRange> ranges, ItemIndexRange range)
        {
            return Merge((IEnumerable<ItemIndexRange>)ranges, range).ToList();
        }
        public static IEnumerable<ItemIndexRange> Merge(this IEnumerable<ItemIndexRange> ranges, ItemIndexRange range)
        {
            var sorted = ranges.Concat(new[] { range }).OrderByDescending(r => r.Length).OrderBy(r => r.FirstIndex);
            foreach (var item in MergeInternal(sorted.Skip(1), sorted.First()))
            {
                yield return item;
            }
        }

        private static IEnumerable<ItemIndexRange> MergeInternal(this IEnumerable<ItemIndexRange> ranges, ItemIndexRange range)
        {
            if (ranges.Any())
            {
                var merge = Merge(ranges.First(), range).ToArray();
                if (merge.Length == 2)
                {
                    yield return merge[0];
                }
                range = merge.Last();

                foreach (var item in MergeInternal(ranges.Skip(1), range))
                {
                    yield return item;
                }

                yield break;
            }
            yield return range;
        }

        public static IEnumerable<ItemIndexRange> Merge(this ItemIndexRange me, ItemIndexRange range)
        {
            if (me.GreaterThan(range))
            {
                foreach (var item in Merge(range, me))
                {
                    yield return item;
                }
                yield break;
            }

            if (me.Contains(range))
            {
                yield return me;
                yield break;
            }

            if (range.Contains(me))
            {
                yield return range;
                yield break;
            }

            if (me.LastIndex >= range.FirstIndex || me.LastIndex >= range.FirstIndex - 1)
            {
                yield return CreateRange(me.FirstIndex, range.LastIndex);
                yield break;
            }

            yield return me;
            yield return range;
        }

        public static IList<ItemIndexRange> Subtract(this IList<ItemIndexRange> ranges, ItemIndexRange range)
        {
            return Subtract((IEnumerable<ItemIndexRange>)ranges, range).ToList();
        }
        public static IEnumerable<ItemIndexRange> Subtract(this IEnumerable<ItemIndexRange> ranges, ItemIndexRange range)
        {
            if (ranges.Any())
            {
                foreach (var r in ranges)
                {
                    foreach (var item in Subtract(r, range))
                    {
                        yield return item;
                    }
                }
            }
        }

        public static IEnumerable<ItemIndexRange> Subtract(this ItemIndexRange me, ItemIndexRange range)
        {
            if (range.LastIndex < me.FirstIndex)
            {
                yield return me;
                yield break;
            }

            if (range.FirstIndex > me.LastIndex)
            {
                yield return me;
                yield break;
            }

            if (range.FirstIndex <= me.FirstIndex)
            {
                if (range.LastIndex >= me.LastIndex)
                {
                    yield break;
                }
                yield return new ItemIndexRange(range.LastIndex + 1, (uint)(me.LastIndex - range.LastIndex));
                yield break;
            }

            if (range.FirstIndex > me.FirstIndex && range.LastIndex >= me.LastIndex)
            {
                yield return new ItemIndexRange(me.FirstIndex, (uint)(range.FirstIndex - me.FirstIndex));
                yield break;
            }

            yield return new ItemIndexRange(me.FirstIndex, (uint)(range.FirstIndex - me.FirstIndex));
            yield return new ItemIndexRange(range.LastIndex + 1, (uint)(me.LastIndex - range.LastIndex));
        }

        public static bool GreaterThan(this ItemIndexRange me, ItemIndexRange range)
        {
            return me.FirstIndex > range.FirstIndex || (me.FirstIndex == range.FirstIndex && me.LastIndex > range.LastIndex);
        }

        public static bool Contains(this ItemIndexRange me, ItemIndexRange range)
        {
            return me.FirstIndex <= range.FirstIndex && me.LastIndex >= range.LastIndex;
        }

        public static bool Intersects(this IEnumerable<ItemIndexRange> me, ItemIndexRange range)
        {
            foreach (var item in me)
            {
                if (item.Intersects(range))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool Intersects(this ItemIndexRange me, ItemIndexRange range)
        {
            return (range.FirstIndex >= me.FirstIndex && range.FirstIndex <= me.LastIndex) || (range.LastIndex >= me.FirstIndex && range.LastIndex <= me.LastIndex);
        }
        public static bool Intersects(this ItemIndexRange me, int firstIndex, uint Length)
        {
            int LastIndex = firstIndex + (int)Length - 1;
            return (firstIndex >= me.FirstIndex && firstIndex <= me.LastIndex) || (LastIndex >= me.FirstIndex && LastIndex <= me.LastIndex);
        }

        public static IEnumerable<IndexRange> GetIndexRanges(this IReadOnlyList<ItemIndexRange> ranges)
        {
            foreach (var range in ranges)
            {
                yield return new IndexRange { Index = range.FirstIndex, Length = (int)range.Length };
            }
        }

        private static ItemIndexRange CreateRange(int firstIndex, int lastIndex)
        {
            return new ItemIndexRange(firstIndex, (uint)(lastIndex - firstIndex + 1));
        }

        #region AsString
        public static string AsString(this IEnumerable<ItemIndexRange> ranges)
        {
            using (var writer = new StringWriter())
            {
                foreach (var item in ranges)
                {
                    writer.Write(item.AsString());
                }
                return writer.ToString();
            }
        }

        public static string AsString(this ItemIndexRange me)
        {
            return String.Format("[{0},{1}]", me.FirstIndex, me.LastIndex);
        }
        #endregion
    }
}
