using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsureThat;

namespace Augment
{
    /// <summary>
    /// Defines a generic range class.
    /// </summary>
    /// <typeparam name="T">The type of the range.</typeparam>
    /// <remarks>
    /// Obtained from http://spacklenet.codeplex.com/
    /// </remarks>
    [DebuggerDisplay("Range=({Start},{End})")]
    public sealed class Range<T> : IEquatable<Range<T>> where T : IComparable<T>
    {
        /// <summary>
        /// Creates a new instance.  If end is less than start, the values are reversed.
        /// </summary>
        /// <param name="start">Start of the range.</param>
        /// <param name="end">End of the range.</param>
        public Range(T start, T end)
        {
            if (start.CompareTo(end) <= 0)
            {
                Start = start;
                End = end;
            }
            else
            {
                Start = end;
                End = start;
            }
        }

        /// <summary>
        /// Determines whether two specified ranges have the same value. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if the value of a is the same as the value of b; otherwise, false. </returns>
        public static bool operator ==(Range<T> a, Range<T> b)
        {
            bool areEqual = false;

            if (object.ReferenceEquals(a, b))
            {
                areEqual = true;
            }

            if ((object)a != null && (object)b != null)
            {
                areEqual = a.Equals(b);
            }

            return areEqual;
        }

        /// <summary>
        /// Determines whether two specified ranges have different values. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if the value of a is different from the value of b; otherwise, false. </returns>
        public static bool operator !=(Range<T> a, Range<T> b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Checks to see if the given value is within the current range (inclusive).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>Returns <c>true</c> if <paramref name="value"/> is in the range; otherwise, <c>false</c>.</returns>
        public bool Contains(T value)
        {
            return value.CompareTo(Start) >= 0 && value.CompareTo(End) <= 0;
        }

        /// <summary>
        /// Determines whether this instance and another have the same value. 
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if other is a ranges and its value is the same as this instance; otherwise, false.</returns>
        public bool Equals(Range<T> other)
        {
            var areEqual = false;

            if (other != null)
            {
                areEqual = Start.CompareTo(other.Start) == 0 && End.CompareTo(other.End) == 0;
            }

            return areEqual;
        }

        /// <summary>
        /// Determines whether this instance and a specified object, have the same value. 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if obj is a range and its value is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Range<T>);
        }

        /// <summary>
        /// Returns the hash code.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>		
        public override int GetHashCode()
        {
            var hashCode = Start.GetHashCode();

            if (End.CompareTo(Start) != 0)
            {
                hashCode ^= End.GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// Returns the intersection of two ranges
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Range<T> Intersect(Range<T> target)
        {
            Ensure.That(target, "target").IsNotNull();

            Range<T> intersection = null;

            if (Contains(target.Start) || Contains(target.End))
            {
                T intersectionStart = Start.CompareTo(target.Start) >= 0 ? Start : target.Start;

                T intersectionEnd = End.CompareTo(target.End) <= 0 ? End : target.End;

                intersection = new Range<T>(intersectionStart, intersectionEnd);
            }

            return intersection;
        }

        /// <summary>
        /// Returns the union of two ranges
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Range<T> Union(Range<T> target)
        {
            Ensure.That(target, "target").IsNotNull();

            Range<T> intersection = null;

            if (Contains(target.Start) || Contains(target.End) || target.Contains(Start) || target.Contains(End))
            {
                T intersectionStart = Start.CompareTo(target.Start) >= 0 ? target.Start : Start;

                T intersectionEnd = End.CompareTo(target.End) <= 0 ? target.End : End;

                intersection = new Range<T>(intersectionStart, intersectionEnd);
            }

            return intersection;
        }

        /// <summary>
        /// End of range
        /// </summary>
        public T End { get; private set; }

        /// <summary>
        /// Start of range
        /// </summary>
        public T Start { get; private set; }
    }
}