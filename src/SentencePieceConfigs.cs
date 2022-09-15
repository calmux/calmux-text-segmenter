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
    public class SentencePieceTrainConfig : SegmenterTrainConfigBase, ISentencePieceConfig
    {
        /// <summary>
        /// comma-separated list of languages this model can accept
        /// </summary>
        public string AcceptLanguage { get; set; }
        /// <summary>
        /// Add dummy whitespace at the beginning of text ( default: true )
        /// </summary>
        public bool? AddDummyPrefix { get; set; }
        /// <summary>
        /// Override BOS (&lt;s&gt;) id. Set -1 to disable BOS ( default: -1 )
        /// @BUGBUG: BosId, eosId and UnkId should not be user-specifyable, as they are controlled by Flo
        /// </summary>
        public Int32 BosId { get; set; } = -1;
        /// <summary>
        /// Character coverage to determine the minimum symbols ( default: 0.9995 )
        /// </summary>
        public double? CharacterCoverage { get; set; }
        /// <summary>
        /// Comma separated list of control symbols
        /// </summary>
        public string ControlSymbols { get; set; }
        /// <summary>
        /// Override EOS ((&lt;/s&gt;)) id. Set -1 to disable EOS. ( default: 0 )
        /// @BUGBUG: BosId, eosId and UnkId should not be user-specifyable, as they are controlled by Flo
        /// </summary>
        public Int32 EosId { get; set; } = 0;
        /// <summary>
        /// If set to false,