﻿// Copyright (c) Microsoft Corporation.
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
        /// If set to false, --vocab_size is considered as a soft limit. ( default: true )
        /// </summary>
        public bool? HardVocabLimit { get; set; }
        /// <summary>
        /// Comma separated list of input sentences )  type: string 
        /// </summary>
        public string input { get; set; }
        /// <summary>
        /// Input format. Supported format is 'text' or 'tsv'. ( default: 'text' )
        /// </summary>
        public SentencePieceInputFormat? InputFormat { get; set; }
        /// <summary>
        /// Maximum size of sentences the trainer loads ( default: 10000000 )
        /// </summary>
        public Int32? InputSentenceSize { get; set; }
        /// <summary>
        /// Maximum length of sentence in bytes ( default: 2048)
        /// </summary>
        public Int32? MaxSentenceLength { get; set; }
        /// <summary>
        /// Maximum length of sentence piece ( default: 16 )
        /// </summary>
        public Int32? MaxSentencepieceLength { get; set; }
        /// <summary>
        /// Maximum size of sentences to make seed sentence piece ( default: 2000000 )
        /// </summary>
        public Int32? MiningSentenceSize { get; set; }
        /// <summary>
        /// Output model prefix
        /// </summary>
        public string ModelPrefix { get; set; }
        /// <summary>
        /// Model algorithm: unigram, bpe, word or char ( default: unigram )
        /// </summary>
        public SentencePieceModelType? ModelType { get; set; }
        /// <summary>
        /// Normalization rule name. Choose from nfkc or identity ( default: nmt_nfkc )
        /// </summary>
        public SentencePieceNormalizationRuleName? NormalizationRuleName { get; set; }
        /// <summary>
        /// Normalization rule TSV file. 
        /// </summary>
        public string NormalizationRuleTsv { get; set; }
        /// <summary>
        /// Number of EM sub-iterations ( default: 2 )
        /// </summary>
        public Int32? NumSubIterations { get; set; }
        /// <summary>
        /// Number of threads for training ( default: 16 )
        /// </summary>
        public Int32? NumThreads { get; set; }
        /// <summary>
        /// Override PAD (&lt;pad&gt;) id. Set -