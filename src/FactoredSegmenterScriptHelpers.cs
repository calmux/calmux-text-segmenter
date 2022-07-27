
﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

// This is meant as an extension of Unicode.cs. It should be merged into there,
// once the code in here has reached a sufficient level of maturity and generality
// across languages, and generally support surrogate pairs.

using System.Collections.Generic;
using System.Linq;

namespace Common.Text
{
    /// <summary>
    /// Helper class for Unicode characters.
    /// @BUGBUG: These do not work with surrogate pairs.
    /// </summary>
    public static class ScriptExtensions
    {
        /// <summary>
        /// Helper to test whether a character has a character code in range min..max
        /// </summary>
        public static bool IsInRange(this char c, int min, int max) => (c >= (char)min && c <= (char)max);

        /// <summary>
        /// Is character a combining character?
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsCombiner(this char c) => c.GetUnicodeMajorDesignation() == 'M';

        /// <summary>
        /// Is character a Variation Selector? [https://en.wikipedia.org/wiki/Variation_Selectors_(Unicode_block)]
        /// Note that these are included in IsCombiner as well.
        /// </summary>
        //public static bool IsVariationSelector(this char c) => c.IsInRange(0xfe00, 0xfe0f);

        /// <summary>
        /// Helper to determine whether a character is a numeral.
        /// This includes numeral characters that are not classified as such in Unicode,
        /// such as Chinese numbers.
        /// This is meant for FactoredSegmenter, which uses this to prevent numeral characters
        /// from being merged in SentencePiece.
        public static bool IsNumeral(this char c)
        {
            // @BUGBUG: currently known failures:
            //  - Arabic fractions: ٠٫٢٥

            // Chinese numeral letters are not classified as digits in Unicode
            if (ScriptHelpers.ChineseDigits.Contains(c))
                return true;
            else
                return Unicode.GetUnicodeMajorDesignation(c) == 'N';
        }

        /// <summary>
        /// Is this character considered a letter inside FactoredSegmenter?
        /// This also returns true for combiners that are typically used with letters.
        /// @TODO: decide how to handle wide letters, or all sorts of weird letters such as exponents
        ///        Are those letters? Are those capitalizable?
        ///        Then remove this wrapper.
        /// </summary>
        public static bool IsLetterOrLetterCombiner(this char c)
            => char.IsLetter(c) ||
               (c.IsCombiner() && c.GetCombinerTypicalMajorDesignation() == 'L');

        ///// <summary>
        ///// Combine IsLetter() and IsNumeral(), which is used a few times in this combination.
        ///// </summary>
        ///// <param name="c"></param>
        ///// <returns></returns>
        //public static bool IsLetterOrNumeral(this char c) => c.IsLetter() || c.IsNumeral();

        /// <summary>
        /// Tests whether a character is a bicameral letter.
        /// @TODO: should we consider German ess-zet as bicameral? Lower=upper, but
        /// as of recently, an upper-case ess-zet exist.
        /// One can all-caps a word with ess-zet. This is currently special-cased in FactoredSegmenter.
        /// </summary>
        public static bool IsBicameral(this char c) => char.ToLowerInvariant(c) != char.ToUpperInvariant(c);

        /// <summary>
        /// Replacement for IsLower() that handles Roman numeral X correctly
        /// We define a lower-case letter as one that is bicameral in the first place, and of the lower kind.
        /// </summary>
        public static bool HasAndIsLower(this char c) => c != char.ToUpperInvariant(c);
        /// <summary>
        /// Same as HasAndIsLower() except for upper-case.
        /// </summary>
        public static bool HasAndIsUpper(this char c) => char.ToLowerInvariant(c) != c;

        /// <summary>
        /// String/index version of HasAndIsLower().
        /// @BUGBUG: Does not handle surrogate pairs.
        /// </summary>
        public static bool HasAndIsLowerAt(this string s, int index) => s[index].HasAndIsLower();

        /// <summary>
        /// String/index version of HasAndIsUpper().
        /// @BUGBUG: Does not handle surrogate pairs.
        /// </summary>
        public static bool HasAndIsUpperAt(this string s, int index) => s[index].HasAndIsUpper();

        /// <summary>
        /// Test if string is a single Unicode character, with support for surrogate pairs.
        /// Used for detecting unrepresentable Unicode characters.
        /// </summary>
        public static bool IsSingleCharConsideringSurrogatePairs(this string s)
        {
            var length = s.Length;
            return length == 1 ||
                  (length == 2 && char.IsSurrogatePair(s, 0));
        }

        /// <summary>
        /// Capitalize the first letter of a string and return the result.
        /// This function attempts to be efficient and not allocate a new string
        /// if the string is unchanged.
        /// </summary>
        public static string Capitalized(this string s)
        {
            if (!string.IsNullOrEmpty(s) && s.First().HasAndIsLower())
            {
                var a = s.ToArray();
                a[0] = char.ToUpperInvariant(a[0]);
                return new string(a);
            }
            else
                return s;
        }

        /// <summary>
        /// Define a "typical" use for combining marks. FactoredSegmenter requires pieces to
        /// be classifyable as being of word nature or not. Combiners depend on context.
        /// This can lead to a contradiction if a combiner gets separated from its preceding
        /// character by SentencePiece (which we allow since in Hindi, some combiners are morphemes).
        /// The problem is that each lemma has a unique factor set. But if the lemma is a
        /// combiner that is used both with a letter or with punctuation in the corpus,