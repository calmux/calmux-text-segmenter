// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Microsoft.MT.Common.Tokenization
{
    // types for SentencePiece
    public enum SentencePieceModelType
    {
        [XmlEnum(Name = "unigram")]
        Unigram = 0,
        [XmlEnum(Name = "bpe")]
        Bpe,
        [XmlEnum(Name = "word")]
        Word,
        [XmlEnum(Name = "char")]
        Char
    }

    public enum SentencePieceNormalizationRuleName
    {
        [XmlEnum(Name = "nmt_nfkc")]
     