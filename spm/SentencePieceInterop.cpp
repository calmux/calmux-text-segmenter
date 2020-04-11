
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

#include <stdio.h>
#include <iostream>

#include <sentencepiece_processor.h>
#include <memory>  // for unique_ptr
#include <fstream>
#include "unicode_conversions.h"

// This requires libsentencepiece.so. If you follow the build instructions
// from C++ source on https://github.com/google/sentencepiece, the necessary
// header file and library will be installed in the system.

// ---------------------------------------------------------------------------
// C++ implementation of the functionality
// ---------------------------------------------------------------------------

class SentencePieceInterop
{
    std::unique_ptr<sentencepiece::SentencePieceProcessor> m_processor;

    void check_status(sentencepiece::util::Status status, const char* what)
    {
        if (status.ok())
            return;
        std::string err = status.ToString();
        std::cerr << err << std::endl;
        throw std::runtime_error(std::string("SentencePiece error ") + what + ": " + err);
    }
public:
    SentencePieceInterop(const uint16_t* modelPath, const uint16_t** vocab, size_t vocabSize)
    {
        m_processor.reset(new sentencepiece::SentencePieceProcessor());
        // load the model file
        const auto status = m_processor->Load(utf16_to_utf8(utf16string(modelPath)));
        // implant the restricted vocabulary, if given
        if (vocab && vocabSize > 0)
        {
            std::vector<std::string> vocab_str;
            for (size_t i = 0; i < vocabSize; i++)
            {
                vocab_str.push_back(utf16_to_utf8(utf16string(vocab[i])));
            }

            m_processor->SetVocabulary(vocab_str);
        }
        check_status(status, "loading");
    }

    int EncodeAsIds(const uint16_t* word, int* pieceIdBuffer, size_t pieceIdBufferSize)
    {
        std::string wordInUtf8 = utf16_to_utf8(utf16string(word));
        auto piece_ids = m_processor->EncodeAsIds(sentencepiece::util::min_string_view(wordInUtf8));
        if (piece_ids.size() > pieceIdBufferSize)
           return -((int)piece_ids.size());
            
        std::copy(piece_ids.begin(), piece_ids.end(), pieceIdBuffer);
        return (int)piece_ids.size();
    }

    int UCS2LengthOfPieceId(int pieceId)
    {
        if (m_processor->IsUnknown(pieceId))
            return -1;
        auto utf8 = m_processor->IdToPiece(pieceId);
        return (int)count_utf8_to_utf16(utf8);
    }
};

// ---------------------------------------------------------------------------
// C/C++ interop and exported C functions
//  - intptr_t object = LoadModel(void* model, size_t modelSize, char** vocab, size_t vocabSize)
//  - length = EncodeAsIds(intptr_t object, const char* wordInUtf8, int* pieceIdBuffer, size_t pieceIdBufferSIze)  // pieceIdBuffer size >= strlen(word)+1
//  - n = UCS2LengthOfPieceId(intptr_t object, int pieceId)
//  - UnloadModel(intptr_t object)
// ---------------------------------------------------------------------------

#if defined(_MSC_VER)
    //  Microsoft 
#define EXPORT __declspec(dllexport)
#define IMPORT __declspec(dllimport)
#elif defined(__GNUC__)
    //  GCC
#define EXPORT __attribute__((visibility("default")))
#define IMPORT
#else
    //  do nothing and hope for the best?
#define EXPORT
#define IMPORT
#pragma warning Unknown dynamic link import/export semantics.
#endif

extern "C" {

intptr_t EXPORT LoadModel(const uint16_t* modelPath, const uint16_t** vocab, size_t vocabSize)
{
    try
    {
        return (intptr_t) new SentencePieceInterop(modelPath, vocab, vocabSize);
    }
    catch(...)  // @TODO: how to return meaningful error information?
    {
        return (intptr_t) nullptr;
    }
}

int EXPORT EncodeAsIds(intptr_t object, const uint16_t* word, int* pieceIdBuffer, size_t pieceIdBufferSize)
{
    try
    {
        return (int)((SentencePieceInterop*)object)->EncodeAsIds(word, pieceIdBuffer, pieceIdBufferSize);
    }
    catch(...)  // @TODO: how to return meaningful error information?
    {
        return -1;
    }
}

int EXPORT UCS2LengthOfPieceId(intptr_t object, int pieceId)
{
    try
    {
        return ((SentencePieceInterop*)object)->UCS2LengthOfPieceId(pieceId);
    }
    catch(...)  // @TODO: how to return meaningful error information?
    {
        return 0;  // 0 is an invalid length
    }
}

void EXPORT UnloadModel(intptr_t object)