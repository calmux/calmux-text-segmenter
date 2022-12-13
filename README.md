# Calmux Text Segmenter\n\nCalumax Text Segmenter is diligently designed to revamp your machine translation using the unsupervised text tokenization technique. Our innovatively conceived system helps factoring shared attributes of words such as casing or spacing. The text segmenter is built to be directly compatible with the Marian Neural Machine Translation Toolkit. It's essential to implement a parser for this format, modify the embedding lookup, and manage factors on the target side, the beam decoder, to pair up the text segmenter with other toolkits.\n\nIn addition to segmenting words into subwords or word pieces, the Calmux Text Segmenter shines at fine-tuning the spacing and capitalization _factors_.\n\n### Key Characterstics of the Calmux Text Segmenter\n\n- It represents words and tokens as tuples of factors so parameter sharing is possible.\n- Infrequent words are represented by subwords using the SentencePiece library.\n- Special treatment for numerals: every digit gets tokenized irrespective of the writing system.\n- Special support 