using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace Augment.SqlServer.Development
{
    static class StringExtensions
    {
        public static string[] SplitIdentifiers(this string name)
        {
            string[] identifiers = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            return identifiers.ToArray();
        }

        public static string ToNormalizedName(this IEnumerable<string> identifiers)
        {
            Ensure.That(identifiers, nameof(identifiers)).IsNotNull();

            return identifiers.Join(".").ToLower();
        }

        public static string ToNormalizedName(this string name)
        {
            return name.SplitIdentifiers().ToNormalizedName();
        }
    }
}
