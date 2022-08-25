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
        Nfkc = 0,
        [XmlEnum(Name = "identity")]
        Identity
    }

    // Note: The following cannot be specified by Flo users, as these are under Flo's control.
    public enum SentencePieceInputFormat
    {
        [XmlEnum(Name = "text")]
        Text = 0,
        [XmlEnum(Name = "tsv")]
        Tsv
    }

    public enum SentencePieceEncodeFormat
    {
        [XmlEnum(Name = "piece")]
        Piece = 0,
        [XmlEnum(Name = "id")]
        Id,
        [XmlEnum(Name = "proto")]
        Proto,
        [XmlEnum(Name = "nbest_piece")]
        NBest_Piece,
        [XmlEnum(Name = "nbest_id")]
        NBest_Id,
        [XmlEnum(Name = "nbest_proto")]
        NBest_Proto
    }

    public enum SentencePieceDecodeInputFormat
    {
        [XmlEnum(Name = "piece")]
        Piece = 0,
        [XmlEnum(Name = "id")]
        Id
    }

    public enum SentencePieceDecodeOutputFormat
    {
        [XmlEnum(Name = "string")]
        String = 0,
        [XmlEnum(Name = "proto")]
        Proto
    }

    /// <summary>
    /// Class to hold all parameters for the SentencePiece training tool.
    /// </summary>
    public class Sent