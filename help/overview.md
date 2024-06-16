## Background

Near the end of the last century, Google pioneered the modern search interface by employing an elegantly simple "search box". This was an evolution away from the complex interfaces that preceded it. Still, it becomes problematic when we want to search for multiple terms, unless we expect to merely "match every term".

Search-for-Truth (S4T) query language provides a concise yet comprehensive syntax for searching and studying the scriptures, configuring, and controlling the AV-Bible application. It gets its name from the Bereans found in Acts 17:11-12, along with those Christians mentioned in 2 Thessalonians 12-13, and Paul's instruction to Timothy in II Timothy 2:15.  God instructs us to "study" his word, and S4T can streamline the mechanics of that activity (only the soul and spirit can actively study the scriptures). In any event, S4T grammar supports Boolean operations such as AND, OR, and NOT. Great care has been taken to support the construction of complex queries. Greater care has been taken to maintain a clear and concise syntax.

S4T is consistent with itself. S4T always favors simplicity over versatility. Avoiding complexity of syntax, makes the grammar easier to explain. S4T also avoids nuance (we chose not to imitate "i" before "e" except after "c"). Without complexity and nuance, S4T is easy to learn, easy to type, and easy to remember.  In fact, search expressions often look no different than they would appear today in a Google or Bing search. Incidentally, S4T is a dialect of Quelle and conforms to the Quelle Specification.

### Grammar Overview

The AV-Bible Addin for Microsoft Word implements a subset of the AV-Word Grammar, known as S4T. This document identified that subset of S4T grammar, suppoorted in the Addin.

| Statement Type       | Syntax                                                       |
| -------------------- | ------------------------------------------------------------ |
| Selection Statement  | Combines search criteria and scoping filters for tailored verse selection.<br/>Configuration settings can also be combined and incorporated into the selection criteria. |
| Imperative Statement | single action for configuration and/or application control (cannot be combined with other actions) |

#### Selection Criteria (includes search operations)

Selection Statement contains Selection Criteria, followed by an <u>optional</u> Directive:

The selection criteria controls how verses are selected. It is made up of one to three blocks. The ordering of blocks is partly prescribed. When present, the expression block must be in the initial position. The scoping block and/or the settings-block follow the expression block (when expressed). So long as scoping clauses are grouped into a single block <u>and</u> the settings clauses are grouped into a single block, those two blocks can be in either order (so long as they are listed after the expression block when it is expressed). 

- Search Expression Block
- Settings Block
- Scoping Block

| Block Position                         | Block Type                  | Hashtag Utilization Level |
| -------------------------------------- | --------------------------- | ------------------------- |
| ***initial***                          | **Search Expression Block** | full utilization          |
| *after expression block when provided* | **Settings Block**          | partial utilization       |
| *after expression block when provided* | **Scoping Block**           | partial utilization       |

## Section 1 - Selection/Search 

#### QuickStart

Consider this proximity search (find Moses and Aaron within a single span of seven words):

*Moses Aaron  +span = 7*

S4T syntax can specify the lexicon to search, by also supplying temporary settings:

*Moses Aaron +span = 7  +lexicon.search = KJV*

The statement above has assigns two settings in the context of search. The search criteria, with the settings means that both Aaron and Moses are required to appear within 7 words of each other, but both names must be present to constitute a match.

Next, consider a search to find Moses <u>or</u> Aaron:

*Moses|Aaron*

The order in which the search terms are provided is insignificant. Additionally, the type-case is insignificant. And either name found would constitute a match.

Of course, there are times when word order is significant. Accordingly, searching for explicit strings can be accomplished using double-quotes as follows:

*"Moses said ... Aaron"*

These constructs can even be combined. For example:

*"Moses said ... Aaron|Miriam"*

In all cases, “...” means “followed by”, but the ellipsis allows other words to appear between "said" and "Aaron". Likewise, it allows words to appear between "said" and "Miriam". 

S4T is designed to be intuitive. It provides the ability to invoke Boolean logic for term-matching and/or linguistic feature-matching. As we saw above, the pipe symbol ( | ) can be used to invoke an *OR* condition.

### Selection Criteria

As we saw in the overview, there three blocks that compose Selection Criteria:

- Expression Block Components

  - *find expression*
  - *complete hashtag utilization*
- Settings Block Components

  - *assign setting*

  - *partial hashtag utilization*
- Scoping Block

  - *filter directives*
  - *partial hashtag utilization*

| Action    | Type             | Position | Action Syntax                  | Repeatable Action                                     |
| --------- | ---------------- | -------- | ------------------------------ | ----------------------------------------------------- |
| *find*    | Expression Block | initial  | search expression or ***#id*** | **no**                                                |
| *utilize* | Expression Block | initial  | ***#tag***                     | **no**: only one hashtag is permitted per block       |
| *assign*  | Settings Block   | initial  | ***+setting = value***         | yes (e.g. ***+format=md +lexicon=kjv +span=verse*** ) |
| *utilize* | Settings Block   | initial  | ***+ #tag***                   | **no**: only one macro is permitted per block         |
| *filter*  | Scoping Block    | post     | ***< scope***                  | yes (e.g. ***< Genesis 3 < Revelation 1-3***)         |
| *utilize* | Scoping Block    | post     | **<** ***#tag***               | **no**: only one macro is permitted per block         |

**Table 1-1** - Summary of actions expressible in the Selection Criteria segment of a Selection/Search imperative statement

Two mutually exclusive optional directives can be issued following the selection criteria. 

#### 1.1 - Search Expression Block

The ampersand symbol can similarly be used to represent *AND* conditions upon terms. As an example. the English language contains words that can sometimes as a noun , and other times as some other part-of-speech. To determine if the bible text contains the word "part" where it is used as a verb, we can issue this command:

"part&/verb/"

The SDK, provided by Digital-AV, has marked each word of the bible text for part-of-speech. With the rich syntax of S4T, this type of search is easy and intuitive.

Of course, part-of-speech expressions can also be used independently of an AND condition, as follows:

"/noun/ ... home" + span = 6

That search would find phrases where a noun appeared within a span of six words, preceding the word "home"

**Valid statement syntax, but no results:**

this&that

/noun/ & /verb/

Both of the statements above are valid, but will not match any results. Search statements attempt to match actual words in  the actual bible text. A word cannot be "this" **and** "that". Likewise, an individual word in a sentence does not operate as a /noun/ **and** a /verb/ at the same time.

**Negating search-terms Example:**

Consider a query for all passages that contain a word beginning with "Lord", followed by any word that is neither a verb nor an adverb:

"Lord\* -/v/ & -/adv/" + span = 15

#### 1.2 - Settings Block

When the same setting appears more than once, only the last setting in the list is preserved.  Example:

+format=md  +format=text

The assignment would be to text.  We call this: "last assignment wins".

Finally, there is a bit more to say about the similarity setting, because it actually has three components. If we issue this command, it affects similarity in two distinct ways:

+similarity = 85%

That command is a concise way of setting two values. It is equivalent to this command

+similarity.word=85%  +similarity.lemma=85%

That is to say, similarity is operative for the lexical word and also the lemma of the word. While not discussed previously, these two similarities thresholds need not be identical. These commands are also valid:

+similarity.word=85%  +similarity.lemma=95%

+similarity.word=85%

+similarity.word=off  +similarity.lemma=100%

+similarity.lemma=off

the lexicon controls operate in a similar manner:

+lexicon=KJV 

That command is a concise way of setting two values. It is equivalent to this command

+lexicon.search=KJV  +lexicon.render=KJV

That is to say, lexicon is operative for searching and rendering. Like the similarity setting, the lexicon setting can also diverge between search and render parts. A common lexicon setting might be:

+lexicon.search=both +lexicon.render=KJV

That setting would search both the KJV (aka AV) lexicon and a modernized lexicon (aka AVX), but verse rendering would only be in KJV.

#### 1.3 - Scoping Block

Sometimes we want to limit the scope of our search. Say that I want to find mentions of the serpent in Genesis. I can search only Genesis by executing this search:

serpent < Genesis

If I also want to search in Genesis and Revelation, this works:

serpent < Genesis < Revelation

Filters also allow Chapter and Verse specifications. To search for the serpent in Genesis Chapter 3, we can do this:

serpent < Genesis 3

Abbreviations are also supported:

vanity < sos < 1co

### Section 2 - Settings 

| Setting Name     | Shorthand | Meaning                                                      | Values                                        | Default Value |
| ---------------- | --------- | ------------------------------------------------------------ | --------------------------------------------- | ------------- |
| span             | -         | proximity distance limit (can be "verse" or number of words) | verse or<br/> 1 to 999                        | verse         |
| lexicon          | -         | Streamlined syntax for setting lexicon.search<br/> and lexicon.render to the same value | av or avx or dual<br/>(kjv or modern or both) | n/a           |
| lexicon.search   | search    | the lexicon to be used for searching                         | av or avx or dual<br/>(kjv or modern or both) | dual / both   |
| lexicon.render   | render    | the lexicon to be used for display/rendering                 | av/avx (kjv/modern)                           | av / kjv      |
| format           | -         | format of results on output                                  | see Table 7                                   | text / utf8   |
| similarity       | -         | Streamlined syntax for setting similarity.word<br/>and similarity.lemma to the same value<br/>Phonetics matching threshold is between 33% and 100%. 100% represents an exact sounds-alike match. Any percentage less than 100, represents a fuzzy sounds-similar match <br/>Similarity matching can be completely disabled by setting this value to off | 33% to 100% **or** *off*                      | off           |
| similarity.word  | word      | fuzzy phonetics matching as described above, but this prefix only affects similarity matching on the word. | 33% to 100% **or** *off*                      | off           |
| similarity.lemma | lemma     | fuzzy phonetics matching as described above, but this prefix only affects similarity matching on the lemma. | 33% to 100% **or** *off*                      | off           |
| revision         | -         | Not really a true setting: it works with the *@get* command to retrieve the revision number of the S4T grammar supported by AV-Engine. This value is read-only. | 4.x.yz                                        | n/a           |
| ALL              | -         | ALL is an aggregate setting: it works with the *@clear* command to reset all variables above to their default values. It is used with *@get* to fetch all settings. | n/a                                           | n/a           |

**TABLE 2-1** - Summary of AV-Bible Settings

## Section 3 - Grammar

### 3.1 - Glossary of S4T Terminology

**Actions:** Actions are complete verb-clauses issued in the imperative [you-understood].  Many actions have one or more parameters.  But just like English, a verb phrase can be a single word with no explicit subject and no explicit object.  Consider this English sentence:

Go!

The subject of this sentence is "you understood".  Similarly, all verbs are issued without an explicit subject. The object of the verb in the one word sentence above is also unstated.  S4T operates in an analogous manner.  Consider this English sentence:

Go Home!

Like the earlier example, the subject is "you understood".  The object this time is defined, and insists that "you" should go home.  Some verbs always have objects, others sometimes do, and still others never do. S4T follows this same pattern and some S4T verbs require direct-objects; and some do not.  In the various tables throughout this document, required and optional parameters are identified, These parameters represent the object of the verb within each respective table.

**Selection Criteria**: Selection what text to render is determined with a search expression, scoping filters, or both.

**Search Expression**: The Search Expression has fragments, and fragments have features. For an expression to match, all fragments must match (Logical AND). For a fragment to match, any feature must match (Logical OR). AND is represented by &. OR is represented by |.

**Unquoted SEARCH clauses:** an unquoted search clause contains one or more search fragments. If there is more than one fragment in the clause, then each fragment is logically AND’ed together.

**Quoted SEARCH clauses:** a quoted clause contains a single string of terms to search. An explicit match on the string is required. However, an ellipsis ( … ) can be used to indicate that other terms may silently intervene within the quoted string.

- It is called *quoted,* as the entire clause is sandwiched on both sides by double-quotes ( " )
- The absence of double-quotes means that the statement is unquoted

**Booleans and Negations:**

**and:** In Boolean logic, **and** means that all terms must be found. With S4T, **and** is represented by terms that appear within an unquoted clause. **And** logic is also available on each search-term by using the **&** operator.

**or:** In Boolean logic, **or** means that any term constitutes a match. With S4T, **and** is represented per each search-term by using the **|** operator.

**not:** In Boolean logic, means that the feature must not be found. With S4T, *not* is represented by the hyphen ( **-** ) and applies to individual features within a fragment of a search expression. It is best used in conjunction with other features, because any non-match will be included in results. 

hyphen ( **-** ) means that any non-match satisfies the search condition. Used by itself, it would likely return every verse. Therefore, it should be used judiciously.

### 3.2 - Specialized Search tokens in AV-Bible

The lexical search domain of S4T includes all words in the original KJV text. It can also optionally search using a modernized lexicon of the KJV (e.g. hast and has; this is controllable with the search.lexicon setting).  The table below lists linguistic extensions available in S4T.

| Search Term        | Operator Type                           | Meaning                                                      | Maps To                                                      | Mask   |
| ------------------ | --------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ | ------ |
| un\*               | wildcard (example)                      | starts with: un                                              | all lexicon entries that start with "un"                     | 0x3FFF |
| \*ness             | wildcard (example)                      | ends with: ness                                              | all lexicon entries that end with "ness"                     | 0x3FFF |
| un\*ness           | wildcard (example)                      | starts with: un<br/>ends with: ness                          | all lexicon entries that start with "un", and end with "ness" | 0x3FFF |
| \*profit\*         | wildcard (example)                      | contains: profit                                             | all lexicon entries that contain both "profit"               | 0x3FFF |
| \*pro\*fit\*       | wildcard (example)                      | contains: pro and fit                                        | all lexicon entries that contain both "pro" and "fit" (in any order) | 0x3FFF |
| un\*profit*ness    | wildcard (example)                      | starts with: un<br/>contains: profit<br/>ends with: ness     | all lexicon entries that start with "un", contain "profit", and end with "ness" | 0x3FFF |
| un\*pro\*fit\*ness | wildcard (example)                      | starts with: un<br/>contains: pro and fit<br/>ends with: ness | all lexicon entries that start with "un", contain "pro" and "fit", and end with "ness" | 0x3FFF |
| ~ʃɛpɝd*            | phonetic wildcard (example)             | Tilde marks the wildcard as phonetic (wildcards never perform sounds-alike searching) | All lexical entries that start with the sound ʃɛpɝd (this would include shepherd, shepherds, shepherding...) | TBD    |
| ~ʃɛpɝdz            | sounds-alike search using IPA (example) | Tilde marks the search term as phonetic (and if similarity is set between 33 and 99, search handles approximate matching) | This would match the lexical entry "shepherds" (and possibly similar terms, depending on similarity threshold) | TDB    |
| \(is)              | lemma                                   | search on all words that share the same lemma as is: be, is, are, art, ... | be is are art ...                                            | 0x3FFF |
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
| [type]             | named entity                            | Entities are recognized by MorphAdorner. They are also matched against Hitchcock's database. This functionality is experimental and considered BETA. | type=person man<br/>woman tribe city<br/>river mountain<br/>animal gemstone<br/>measurement any<br/>any_Hitchcock |        |
| \[FFFF\]           | PN+POS(12)                              | hexadecimal representation of bits for a PN+POS(12) value.   | See Digital-AV SDK                                           | uint16 |
| \[FFFFFFFF\]       | POS(32)                                 | hexadecimal representation of bits for a POS(32) value.      | See Digital-AV SDK                                           | uint32 |
| \[string\]         | nupos-string                            | NUPOS string representing part-of-speech. This is the preferred syntax over POS(32), even though they are equivalent. NUPOS part-of-speech values have higher fidelity than the 16-bit PN+POS(12) representations. | See Part-of-Speech-for-Digital-AV.docx                       | uint32 |
| 99999:H            | Strongs Number                          | decimal Strongs number for the Hebrew word in the Old Testament | One of Strongs\[4\]                                          | 0x7FFF |
| 99999:G            | Strongs Number                          | decimal Strongs number for the Greek word in the New Testament | One of Strongs\[4\]                                          | 0x7FFF |

### 3.4 - S4T conformance to the Quelle specification

Quelle specifies two possible implementation levels:

- Level 1 [basic search support]
- Level 2 [search support includes also searching on part-of-speech tags]

Av-Bible S4T is a Level 2 Quelle implementation with augmented search capabilities. S4T extends Quelle to include AVX-Framework-specific constructs.  However, the Addin does not support the entire S4T grammar. Unsupported grammar is simply not documented here. For full Quelle support, the AV-Bible Windows executable should be utilized.

1. S4T represents the biblical text with two substantially similar, but distinct, lexicons. The search.lexicon setting can be specified by the user to control which lexicon is to be searched. Likewise, the render.lexicon setting is used to control which lexicon is used for displaying the biblical text. As an example, the KJV text of "thou art" would be modernized to "you are".

   - AV/KJV *(a lexicon that faithfully represents the KJV bible; AV purists should select this setting)*

   - AVX/Modern *(a lexicon that that has been modernized to appear more like contemporary English)*

   - Dual/Both *(use both lexicons)*

   The Dual/Both setting for lexicon.search indicates that searching should consider both lexicons. The The Dual/Both setting for lexicon.render indicates that results should be displayed for both renderings [whether this is side-by-side or in-parallel depends on the format and the application where the display-rendering occurs]. Left unspecified, the lexicon setting applies to lexicon.search and lexicon.render components.

2. S4T provides support for fuzzy-match-logic. The similarity setting can be specified by the user to control the similarity threshold for approximate matching. An exact lexical match is expected when similarity is set to *off*. 

   Phonetics matches are enabled when similarity is set between 33% and 100%. Similarity is calculated based upon the phonetic representation for the word.

   The minimum permitted similarity threshold is 33%. Any similarity threshold between 1% and 32% produces a syntax error.

   A similarity setting of 100% still uses phonetics, but expects an exact phonetic match (e.g. "there" and "their" are a 100% phonetic match).

AV-Bible uses the AV-1769 edition of the sacred text. It substantially agrees with the "Bearing Precious Seed" bibles, as published by local church ministries. The text itself has undergone review by Christian missionaries, pastors, and lay people since the mid-1990's. The original incarnation of the digitized AV-1769 text was implemented in the free PC/Windows app known as:

- AV-Bible - 1995 Edition for Windows 95 & Windows NT 3.5
- AV-Bible - 1997 Edition for Windows 95 & Windows NT 4.0
- AV-Bible - 1999 Edition for Windows 98 & Windows NT 4.0
- AV-Bible - 2000 Edition for Windows Me & Windows 2000
- AV-Bible - 2007 Edition for Windows XP
- AV-Bible - 2011 Edition for Windows Vista
- AV-Bible - 2021 Edition for Windows 10
- AV-Bible - 2024 Edition for Windows 11 (current release; initial release to support S4T)

Decades ago, AV-Bible (aka AV-1995, AV-1997, ... AV-2011), were found on internet bulletin boards and the now defunct bible.advocate.com website. More recent legacy versions are still available at the avbible.net website. Modern editions are distributed on the Microsoft store.

Please see https://Digital-AV.org for additional information about the SDK.

