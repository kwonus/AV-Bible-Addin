## Selection Criteria

The selection criteria controls how verses are selected. It is made up of one to three blocks. The ordering of blocks is partly prescribed. When present, the expression block must be in the initial position. The scoping block and the settings-block must follow the expression when provided. The scoping block and settings-block can be in either order (so long as they are listed before the scoping block when present).

- Search Expression Block
- Settings Block
- Scoping Block

| Block Position                         | Block Type                  | Hashtag Utilization Level |
| -------------------------------------- | --------------------------- | ------------------------- |
| ***initial***                          | **Search Expression Block** | full utilization          |
| *after expression block when provided* | **Settings Block**          | partial utilization       |
| *after expression block when provided* | **Scoping Block**           | partial utilization       |

### Search Expression Block

Consider a search to find Moses <u>or</u> Aaron:

*Moses|Aaron*

The order in which the search terms are provided is insignificant. Additionally, the type-case is insignificant. And either name found would constitute a match.

Of course, there are times when word order is significant. Accordingly, searching for explicit strings can be accomplished using double-quotes as follows:

*"Moses said ... Aaron"*

These constructs can even be combined. For example:

*"Moses said ... Aaron|Miriam"*

In all cases, “...” means “followed by”, but the ellipsis allows other words to appear between "said" and "Aaron". Likewise, it allows words to appear between "said" and "Miriam".

### Scoping Block

Sometimes we want to constrain the domain of where we are searching. Say that I want to find mentions of *Moses or Aaron* in Genesis. I can search only Genesis by executing this search:

*Moses|Aaron* < Genesis

If I also want to also include Exodus in the search, this works:

Moses|Aaron < Genesis < Exodus

Scoping allows chapter specification or chapter ranges, as depicted below.

serpent < Genesis 3

serpent < Genesis 3-7

Scoping blaocks can be expressed without a search expression, as follows:

< Genesis 3

That statement would render every verse in Genesis 3, without any highlighting (Highlighting only occurs when combined with a search expression).

The bible instructs us to "rightly divide" the "word of truth" as we study the scriptures [2 Timothy 2:15]. To this end, the AV-Bible application provides some traditional divisions (or groupings). Some of these are overlapping. As there is not universal accord of what the appropriate divisions are, AV-Bible expands the divisions into the books as identified in the table below:

| Identifying Scoping Text        | Variants                                                     | Books                             | Range   |
| ------------------------------- | ------------------------------------------------------------ | --------------------------------- | ------- |
| Old Testament                   | OT / O.T.                                                    | Genesis ***to*** Malachi          | 1 - 39  |
| Law                             | Pentateuch                                                   | Genesis ***to*** Deuteronomy      | 1 - 5   |
| History                         |                                                              | Joshua ***to*** Esther            | 6 - 17  |
| Wisdom and Poetry               | Wisdom & Poetry<br />Wisdom + Poetry<br />Wisdom<br />Poetry | Job ***to*** Song of Solomon      | 18-22   |
| Prophets                        | -                                                            | Isaiah ***to*** Malichi           | 23 - 39 |
| Major Prophets                  | -                                                            | Isaiah ***to*** Daniel            | 23 - 27 |
| Minor Prophets                  | -                                                            | Hosiah ***to*** Malichi           | 28 - 39 |
| New Testament                   | NT / N.T.                                                    | Matthew ***to*** Revelation       | 40 - 66 |
| Gospels and Acts                | Gospels & Acts<br />Gospels + Acts                           | Matthew ***to*** Acts             | 40 - 44 |
| Gospels                         | -                                                            | Matthew ***to*** Luke             | 40 - 43 |
| Epistles                        | Epistle                                                      | Romans ***to*** Jude              | 45 - 65 |
| Church Epistles                 | Church Epistle                                               | Romans ***to*** Colossians        | 45 - 51 |
| Pastoral Epistles               | Pastoral Epistle                                             | 1 Thessalonians ***to*** Philemon | 52 - 57 |
| General Epistles                | General Epistle<br />Jewish Epistles<br />Jewish Epistle     | Hebrews ***to*** Jude             | 58 - 65 |
| General Epistles and Revelation | General Epistles & Revelation<br />General Epistles + Revelation | Hebrews ***to*** Revelation       | 58 - 66 |

For example, if I set the scoping block as below:

Moses < pentateuch

That is equated to to a scoped search as follows:

Moses < Gen < Ex < Lev < Num < Deut

### Settings Block

Any setting is permitted in the search block.

The following settings affect how search expressions behave:

| Setting          | Shorthand | Meaning                                                      | Values                                                       |
| ---------------- | --------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| span             | -         | proximity distance limit                                     | 0 to 999 or verse                                            |
| lexicon.search   | search    | the lexicon to be used for searching                         | av or avx or dual<br />(kjv or modern or both)                |
| similarity.word  | word      | fuzzy phonetics matching as described above, but this prefix only affects similarity matching on the word. | 33% to 99% [fuzzy] **or** ...<br>0 **or** *none*<br>100 **or** *exact* |
| similarity.lemma | lemma     | fuzzy phonetics matching as described above, but this prefix only affects similarity matching on the lemma. | 33% to 99% [fuzzy] **or** ...<br>0 **or** *none*<br>100 **or** *exact* |

The following settings affect how selected verses [i.e. search results] are rendered:

| Setting | Meaning                                      | Values              |
| ------- | -------------------------------------------- | ------------------- |
| render  | the lexicon to be used for display/rendering | av/avx (kjv/modern) |
| format  | format of results on output                  | see Table 7         |

##### Example settings that affect search behavior:

sayest you art&/verb/   +lexicon.search=kjv  +span=7  +similarity.word=85%

The settings block is between the square braces. It is exactly equivalent to this statement:

sayest you art&/verb/ +search=kjv  +span=7  +word=85%

+search=kjv -- use the kjv lexicon for searching [not the modern lexicon (art&/verb/ would never be found in the modern lexicon)]

+word=85% -- exact matches are not necessary, any word need only match with an 85% [sounds-alike] similarity threshold.

+span=7 -- three search terms are expressed in the search. All three must be within a 7-word-span in order to constitute a match.

##### Example settings that affect rendering behavior:

< Genesis 3  +render=modern  +format=html  > C:\users\my-username\Documents\Genesis-3.html

The above statement includes an optional export directive [creating a file]. The following statement does exactly the same thing

+render=avx  +format=html  < Gen3  >  C:\users\my-username\Documents\Genesis-3.html

For more details on what each setting means, perform this command:
@help settings

### Advanced Capabilities and Search Expression Terminology

**Selection Criteria**: Selection what text to render is determined with a search expression, scoping filters, or both.

**Search Expression**: The Search Expression has fragments, and fragments have features. For an expression to match, all fragments must match (Logical AND). For a fragment to match, any feature must match (Logical OR). AND is represented by &. OR is represented by |.

**Unquoted SEARCH clauses:** an unquoted search clause contains one or more search fragments. If there is more than one fragment in the clause, then each fragment is logically AND’ed together.

**Quoted SEARCH clauses:** a quoted clause contains a single string of terms to search. An explicit match on the string is required. However, an ellipsis ( … ) can be used to indicate that other terms may silently intervene within the quoted string.

- It is called *quoted,* as the entire clause is sandwiched on both sides by double-quotes ( " )
- The absence of double-quotes means that the statement is unquoted

**Booleans and Negations:**

**and:** In Boolean logic, **and** means that all terms must be found. In S4T query language, **and** is represented by terms that appear within an unquoted clause. **And** logic is also available on each search-term by using the **&** operator.

**or:** In Boolean logic, **or** means that any term constitutes a match. In S4T, **and** is represented per each search-term by using the **|** operator.

**not:** In Boolean logic, means that the feature must not be found. In S4T, *not* is represented by the hyphen ( **-** ) and applies to individual features within a fragment of a search expression. It is best used in conjunction with other features, because any non-match will be included in results.

hyphen ( **-** ) means that any non-match satisfies the search condition. Used by itself, it would likely return every verse. Therefore, it should be used judiciously.

### Specialized Search tokens in AV-Bible

The lexical search domain of S4T grammar includes all words in the original KJV text. It can also optionally search using a modernized lexicon of the KJV (e.g. hast and has; this is controllable with the search.lexicon setting).  The table below lists linguistic extensions available in S4T query language.

| Search Term        | Operator Type                           | Meaning                                                      | Maps To                                                      | Mask   |
| ------------------ | --------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ | ------ |
| un\*               | wildcard (example)                      | starts with: un                                              | all lexicon entries that start with "un"                     | 0x3FFF |
| \*ness             | wildcard (example)                      | ends with: ness                                              | all lexicon entries that end with "ness"                     | 0x3FFF |
| un\*ness           | wildcard (example)                      | starts with: un<br />ends with: ness                          | all lexicon entries that start with "un", and end with "ness" | 0x3FFF |
| \*profit\*         | wildcard (example)                      | contains: profit                                             | all lexicon entries that contain both "profit"               | 0x3FFF |
| \*pro\*fit\*       | wildcard (example)                      | contains: pro and fit                                        | all lexicon entries that contain both "pro" and "fit" (in any order) | 0x3FFF |
| un\*profit*ness    | wildcard (example)                      | starts with: un<br />contains: profit<br />ends with: ness     | all lexicon entries that start with "un", contain "profit", and end with "ness" | 0x3FFF |
| un\*pro\*fit\*ness | wildcard (example)                      | starts with: un<br />contains: pro and fit<br />ends with: ness | all lexicon entries that start with "un", contain "pro" and "fit", and end with "ness" | 0x3FFF |
| ~ʃɛpɝd*            | phonetic wildcard (example)             | Tilde marks the wildcard as phonetic (wildcards never perform sounds-alike searching) | All lexical entries that start with the sound ʃɛpɝd (this would include shepherd, shepherds, shepherding...) |        |
| ~ʃɛpɝdz            | sounds-alike search using IPA (example) | Tilde marks the search term as phonetic (and if similarity is set between 33 and 99, search handles approximate matching) | This would match the lexical entry "shepherds" (and possibly similar terms, depending on similarity threshold) |        |
| \(is\)             | lemma                                   | search on all words that share the same lemma as is: be, is, are, art, ... | be is are art ...                                            | 0x3FFF |
| /noun/             | lexical marker                          | any word where part of speech is a noun                      | POS12::0x010                                                 | 0x0FF0 |
| /n/                | lexical marker                          | synonym for /noun/                                           | POS12::0x010                                                 | 0x0FF0 |
| /verb/             | lexical marker                          | any word where part of speech is a verb                      | POS12::0x100                                                 | 0x0FF0 |
| /v/                | lexical marker                          | synonym for /verb/                                           | POS12::0x100                                                 | 0x0FF0 |
| /pronoun/          | lexical marker                          | any word where part of speech is a pronoun                   | POS12::0x020                                                 | 0x0FF0 |
| /pn/               | lexical marker                          | synonym for /pronoun/                                        | POS12::0x020                                                 | 0x0FF0 |
| /adjective/        | lexical marker                          | any word where part of speech is an adjective                | POS12::0xF00                                                 | 0x0FFF |
| /adj/              | lexical marker                          | synonym for /adjective/                                      | POS12::0xF00                                                 | 0x0FFF |
| /adverb/           | lexical marker                          | any word where part of speech is an adverb                   | POS12::0xA00                                                 | 0x0FFF |
| /adv/              | lexical marker                          | synonym for /adverb/                                         | POS12::0xA00                                                 | 0x0FFF |
| /determiner/       | lexical marker                          | any word where part of speech is a determiner                | POS12::0xD00                                                 | 0x0FF0 |
| /det/              | lexical marker                          | synonym for /determiner/                                     | POS12::0xD00                                                 | 0x0FF0 |
| /preposition/      | lexical marker                          | any word where part of speech is a preposition               | POS12::0x400                                                 | 0x0FF0 |
| /prep/             | lexical marker                          | any word where part of speech is a preposition               | POS12::0x400                                                 | 0x0FF0 |
| /1p/               | lexical marker                          | any word where it is inflected for 1st person (pronouns and verbs) | POS12::0x100                                                 | 0x3000 |
| /2p/               | lexical marker                          | any word where it is inflected for 2nd person (pronouns and verbs) | POS12::0x200                                                 | 0x3000 |
| /3p/               | lexical marker                          | any word where it is inflected for 3rd person (pronouns, verbs, and nouns) | POS12::0x300                                                 | 0x3000 |
| /singular/         | lexical marker                          | any word that is known to be singular (pronouns, verbs, and nouns) | POS12::0x400                                                 | 0xC000 |
| /plural/           | lexical marker                          | any word that is known to be plural (pronouns, verbs, and nouns) | POS12::0x800                                                 | 0xC000 |
| /WH/               | lexical marker                          | any word that is a WH word (e.g., Who, What, When, Where, How) | POS12::0xC00                                                 | 0xC000 |
| /BoB/              | transition marker                       | any word where it is the first word of the book (e.g. first word in Genesis) | TRAN::0xE0                                                   | 0xF0   |
| /BoC/              | transition marker                       | any word where it is the first word of the chapter           | TRAN::0x60                                                   | 0xF0   |
| /BoV/              | transition marker                       | any word where it is the first word of the verse             | TRAN::0x20                                                   | 0xF0   |
| /EoB/              | transition marker                       | any word where it is the last word of the book (e.g. last word in revelation) | TRAN::0xF0                                                   | 0xF0   |
| /EoC/              | transition marker                       | any word where it is the last word of the chapter            | TRAN::0x70                                                   | 0xF0   |
| /EoV/              | transition marker                       | any word where it is the last word of the verse              | TRAN::0x30                                                   | 0xF0   |
| /Hsm/              | segment marker                          | Hard Segment Marker (end) ... one of \. \? \!                | TRAN::0x40                                                   | 0x07   |
| /Csm/              | segment marker                          | Core Segment Marker (end) ... \:                             | TRAN::0x20                                                   | 0x07   |
| /Rsm/              | segment marker                          | Real Segment Marker (end) ... one of \. \? \! \:             | TRAN::0x60                                                   | 0x07   |
| /Ssm/              | segment marker                          | Soft Segment Marker (end) ... one of \, \; \( \) --          | TRAN::0x10                                                   | 0x07   |
| /sm/               | segment marker                          | Any Segment Marker (end)  ... any of the above               | TRAN::!=0x00                                                 | 0x07   |
| /_/                | punctuation                             | any word that is immediately marked for clausal punctuation  | PUNC::!=0x00                                                 | 0xE0   |
| /!/                | punctuation                             | any word that is immediately followed by an exclamation mark | PUNC::0x80                                                   | 0xE0   |
| /?/                | punctuation                             | any word that is immediately followed by a question mark     | PUNC::0xC0                                                   | 0xE0   |
| /./                | punctuation                             | any word that is immediately followed by a period (declarative) | PUNC::0xE0                                                   | 0xE0   |
| /-/                | punctuation                             | any word that is immediately followed by a hyphen/dash       | PUNC::0xA0                                                   | 0xE0   |
| /;/                | punctuation                             | any word that is immediately followed by a semicolon         | PUNC::0x20                                                   | 0xE0   |
| /,/                | punctuation                             | any word that is immediately followed by a comma             | PUNC::0x40                                                   | 0xE0   |
| /:/                | punctuation                             | any word that is immediately followed by a colon (information follows) | PUNC::0x60                                                   | 0xE0   |
| /'/                | punctuation                             | any word that is possessive, marked with an apostrophe       | PUNC::0x10                                                   | 0x10   |
| /)/                | parenthetical text                      | any word that is immediately followed by a close parenthesis | PUNC::0x0C                                                   | 0x0C   |
| /(/                | parenthetical text                      | any word contained within parenthesis                        | PUNC::0x04                                                   | 0x04   |
| /Italics/          | text decoration                         | italicized words marked with this bit in punctuation byte    | PUNC::0x02                                                   | 0x02   |
| /Jesus/            | text decoration                         | words of Jesus marked with this bit in punctuation byte      | PUNC::0x01                                                   | 0x01   |
| /delta/            | lexicon                                 | [archaic] word can be transformed into modern American English |                                                              |        |
| {type}             | named entity                            | Entities are recognized by MorphAdorner. They are also matched against Hitchcock's database. This functionality is experimental and considered BETA. | type=person man<br />woman tribe city<br />river mountain<br />animal gemstone<br />measurement any<br />any_Hitchcock |        |
| \[FFFF\]           | PN+POS(12)                              | hexadecimal representation of bits for a PN+POS(12) value.   | See Digital-AV SDK                                           | uint16 |
| \[FFFFFFFF\]       | POS(32)                                 | hexadecimal representation of bits for a POS(32) value.      | See Digital-AV SDK                                           | uint32 |
| [string]           | nupos-string                            | NUPOS string representing part-of-speech. This is the preferred syntax over POS(32), even though they are equivalent. NUPOS part-of-speech values have higher fidelity than the 16-bit PN+POS(12) representations. | See Part-of-Speech-for-Digital-AV.docx                       | uint32 |
| 99999:H            | Strongs Number                          | decimal Strongs number for the Hebrew word in the Old Testament | One of Strongs\[4\]                                          | 0x7FFF |
| 99999:G            | Strongs Number                          | decimal Strongs number for the Greek word in the New Testament | One of Strongs\[4\]                                          | 0x7FFF |

### S4T grammar conforms to the Quelle specification

[Quelle]: https://github.com/kwonus/Quelle

specifies two possible implementation levels:

- Level 1 [basic search support]
- Level 2 [search support includes also searching on part-of-speech tags]

S4T is a Level 2 Quelle implementation with augmented search capabilities. S4T grammar extends the Quelle specification, to include AVX-Framework-specific constructs.  These extensions provide additional specialized search features and the ability to manage two distinct lexicons for the biblical texts.

1. S4T grammar represents the biblical text with two substantially similar, but distinct, lexicons. The search.lexicon setting can be specified by the user to control which lexicon is to be searched. Likewise, the render.lexicon setting is used to control which lexicon is used for displaying the biblical text. As an example, the KJV text of "thou art" would be modernized to "you are".

- AV/KJV *(a lexicon that faithfully represents the KJV bible; AV purists should select this setting)*

- AVX/Modern *(a lexicon that that has been modernized to appear more like contemporary English)*

- Dual/Both *(use both lexicons)*

The Dual/Both setting for lexicon.search indicates that searching should consider both lexicons. The The Dual/Both setting for lexicon.render indicates that results should be displayed for both renderings [whether this is side-by-side or in-parallel depends on the format and the application where the display-rendering occurs]. Left unspecified, the lexicon setting applies to lexicon.search and lexicon.render components.

2. S4T grammar provides support for fuzzy-match-logic. The similarity setting can be specified by the user to control the similarity threshold for approximate matching. An exact lexical match is expected when similarity is set to *exact* or 0.  Zero is not really a similarity threshold, but rather 0 is a synonym for *exact*.

Approximate matches are considered when similarity is set between 33% and 99%. Similarity is calculated based upon the phonetic representation for the word.

The minimum permitted similarity threshold is 33%. Any similarity threshold between 1% and 32% produces a syntax error.

A similarity setting of *precise* or 100% is a special case that still uses phonetics, but expects a full phonetic match (e.g. "there" and "their" are a 100% phonetic match).

AV-Bible uses the AV-1769 edition of the sacred text. It substantially agrees with the "Bearing Precious Seed" bibles, as published by local church ministries. The text itself has undergone review by Christian missionaries, pastors, and lay people since the mid-1990's. The original incarnation of the digitized AV-1769 text was implemented in the free PC/Windows app known as:

- AV-Bible - 1995 Edition for Windows 95 & Windows NT 3.5
- AV-Bible - 1997 Edition for Windows 95 & Windows NT 4.0
- AV-Bible - 1999 Edition for Windows 98 & Windows NT 4.0
- AV-Bible - 2000 Edition for Windows Me & Windows 2000
- AV-Bible - 2007 Edition for Windows XP
- AV-Bible - 2011 Edition for Windows Vista
- AV-Bible - 2021 Edition for Windows 10
- AV-Bible - 2024 Edition for Windows 11 (current release; initial release to support S4T grammar)

Decades ago, AV-Bible (aka AV-1995, AV-1997, ... AV-2011), were found on internet bulletin boards and the now defunct bible.advocate.com website. More recent legacy versions are still available at the avbible.net website. Modern editions are distributed on the Microsoft store.

Please see https://Digital-AV.org for additional information about the SDK.
