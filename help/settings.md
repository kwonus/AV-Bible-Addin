### Settings

S4T manifests these user-controlled settings.

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



