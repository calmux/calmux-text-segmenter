
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

// This was extracted from https://github.com/microsoft/cpprestsdk/blob/cdae258bfb22f948c7b768b4dc56f5f4a2d9b2ce/Release/src/utilities/asyncrt_utils.cpp#L305

#include <string>
#include <stdexcept>

typedef std::basic_string<uint16_t> utf16string;

#define LOW_3BITS 0x7
#define LOW_4BITS 0xF
#define LOW_5BITS 0x1F
#define LOW_6BITS 0x3F
#define BIT4 0x8
#define BIT5 0x10
#define BIT6 0x20
#define BIT7 0x40
#define BIT8 0x80
#define L_SURROGATE_START 0xDC00
#define L_SURROGATE_END 0xDFFF
#define H_SURROGATE_START 0xD800
#define H_SURROGATE_END 0xDBFF
#define SURROGATE_PAIR_START 0x10000

// Create a dedicated type for characters to avoid the issue
// of different platforms defaulting char to be either signed
// or unsigned.
using UtilCharInternal_t = signed char;

inline size_t count_utf8_to_utf16(const std::string& s)
{
    const size_t sSize = s.size();
    auto const sData = reinterpret_cast<const UtilCharInternal_t*>(s.data());
    size_t result {sSize};

    for (size_t index = 0; index < sSize;)
    {
        if (sData[index] >= 0)
        {
            // use fast inner loop to skip single byte code points (which are
            // expected to be the most frequent)
            while ((++index < sSize) && (sData[index] >= 0))
                ;

            if (index >= sSize) break;
        }

        // start special handling for multi-byte code points
        const UtilCharInternal_t c {sData[index++]};

        if ((c & BIT7) == 0)
        {
            throw std::range_error("UTF-8 string character can never start with 10xxxxxx");
        }
        else if ((c & BIT6) == 0) // 2 byte character, 0x80 to 0x7FF
        {
            if (index == sSize)
            {
                throw std::range_error("UTF-8 string is missing bytes in character");
            }

            const UtilCharInternal_t c2 {sData[index++]};
            if ((c2 & 0xC0) != BIT8)
            {
                throw std::range_error("UTF-8 continuation byte is missing leading bit mask");
            }

            // can't require surrogates for 7FF
            --result;
        }
        else if ((c & BIT5) == 0) // 3 byte character, 0x800 to 0xFFFF
        {
            if (sSize - index < 2)
            {
                throw std::range_error("UTF-8 string is missing bytes in character");
            }

            const UtilCharInternal_t c2 {sData[index++]};
            const UtilCharInternal_t c3 {sData[index++]};
            if (((c2 | c3) & 0xC0) != BIT8)
            {
                throw std::range_error("UTF-8 continuation byte is missing leading bit mask");
            }

            result -= 2;
        }
        else if ((c & BIT4) == 0) // 4 byte character, 0x10000 to 0x10FFFF
        {
            if (sSize - index < 3)
            {
                throw std::range_error("UTF-8 string is missing bytes in character");
            }

            const UtilCharInternal_t c2 {sData[index++]};
            const UtilCharInternal_t c3 {sData[index++]};
            const UtilCharInternal_t c4 {sData[index++]};
            if (((c2 | c3 | c4) & 0xC0) != BIT8)
            {
                throw std::range_error("UTF-8 continuation byte is missing leading bit mask");
            }

            const uint32_t codePoint =
                ((c & LOW_3BITS) << 18) | ((c2 & LOW_6BITS) << 12) | ((c3 & LOW_6BITS) << 6) | (c4 & LOW_6BITS);
            result -= (3 - (codePoint >= SURROGATE_PAIR_START));
        }
        else
        {
            throw std::range_error("UTF-8 string has invalid Unicode code point");
        }
    }

    return result;
}

utf16string /*__cdecl conversions::*/utf8_to_utf16(const std::string& s)
{
    // Save repeated heap allocations, use the length of resulting sequence.
    const size_t srcSize = s.size();
    auto const srcData = reinterpret_cast<const UtilCharInternal_t*>(s.data());
    utf16string dest(count_utf8_to_utf16(s), L'\0');
    utf16string::value_type* const destData = &dest[0];
    size_t destIndex = 0;

    for (size_t index = 0; index < srcSize; ++index)
    {
        UtilCharInternal_t src = srcData[index];
        switch (src & 0xF0)
        {
            case 0xF0: // 4 byte character, 0x10000 to 0x10FFFF
            {
                const UtilCharInternal_t c2 {srcData[++index]};
                const UtilCharInternal_t c3 {srcData[++index]};
                const UtilCharInternal_t c4 {srcData[++index]};
                uint32_t codePoint =
                    ((src & LOW_3BITS) << 18) | ((c2 & LOW_6BITS) << 12) | ((c3 & LOW_6BITS) << 6) | (c4 & LOW_6BITS);
                if (codePoint >= SURROGATE_PAIR_START)
                {
                    // In UTF-16 U+10000 to U+10FFFF are represented as two 16-bit code units, surrogate pairs.
                    //  - 0x10000 is subtracted from the code point
                    //  - high surrogate is 0xD800 added to the top ten bits