using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVX
{
    public class BookInfo
    {
        public string[] Abbreviations { get; private set; }
        private byte[] _VerseCountsByChapter;

        public byte[] VerseCountsByChapter
        {
            get
            {
                if (!Initialized)
                    Initialize();
                return this._VerseCountsByChapter;
            }
        }

        public byte ChapterCount { get => (byte)(_VerseCountsByChapter.Length - 1); }
        public string Name { get => Abbreviations[0]; }

        private static bool Initialized = false;

        private static void Initialize()
        {
            int i = 0;
            foreach (BookInfo book in Metrics)
            {
                if (book == null) continue;

                for (byte c = 1; c <= book.ChapterCount; c++)
                    book._VerseCountsByChapter[c] = RunningListOfVerseCounts[i++];
            }
            Initialized = true;
        }

        private BookInfo(string[] abbrev, byte cnt)
        {
            this.Abbreviations = abbrev;
            this._VerseCountsByChapter = new byte[cnt+1];
            this._VerseCountsByChapter[0] = 0; // there is never a chapter-zero
        }

        public static BookInfo GetBook(string spec)
        {
            string norm = spec.Trim();
            foreach (BookInfo book in Metrics)
            {
                if (book == null) continue;

                foreach (var x in book.Abbreviations)
                    if (x.Equals(spec, StringComparison.InvariantCultureIgnoreCase))
                        return book;
            }
            norm = norm.Replace("'", "").Replace(" ", "").Replace("\t", "");
            foreach (BookInfo book in Metrics)
            {
                if (book == null) continue;

                foreach (var x in book.Abbreviations)
                {
                    if (x.IndexOf(' ') == -1 && x.IndexOf("'") == -1)
                    {
                        if (x.Equals(norm, StringComparison.InvariantCultureIgnoreCase))
                            return book;
                    }
                    else
                    {
                        var xnorm = x.Replace("'", "").Replace(" ", "");
                        if (xnorm.Equals(norm, StringComparison.InvariantCultureIgnoreCase))
                            return book;
                    }
                }
            }
            if (spec.StartsWith("I ",  StringComparison.InvariantCultureIgnoreCase)
            ||  spec.StartsWith("I\t", StringComparison.InvariantCultureIgnoreCase))
            {
                norm = "1" + norm.Substring(1);
            }
            norm = norm.Replace("III", "3")
                       .Replace("3rd", "3")
                       .Replace("II",  "2")
                       .Replace("2nd", "2")
                       .Replace("1st", "1");
            foreach (BookInfo book in Metrics)
            {
                if (book == null) continue;

                foreach (var x in book.Abbreviations)
                {
                    if (x.IndexOf(' ') == -1 && x.IndexOf("'") == -1)
                    {
                        if (x.Equals(norm, StringComparison.InvariantCultureIgnoreCase))
                            return book;
                    }
                    else
                    {
                        var xnorm = x.Replace("'", "").Replace(" ", "");
                        if (xnorm.Equals(norm, StringComparison.InvariantCultureIgnoreCase))
                            return book;
                    }
                }
            }
            return null;
        }

        static BookInfo B01 = new BookInfo(
            new string[] { "Genesis", "Ge", "Gen", "Gen", "Gn" },
            50
        );
        static BookInfo B02 = new BookInfo(
            new string[] { "Exodus", "Ex", "Exo", "Exo", "Exod" },
            40
        );
        static BookInfo B03 = new BookInfo(
            new string[] { "Leviticus", "Le", "Lev", "Lev", "Lv" },
            27
        );
        static BookInfo B04 = new BookInfo(
            new string[] { "Numbers", "Nu", "Num", "Numb", "Nb" },
            36
        );
        static BookInfo B05 = new BookInfo(
            new string[] { "Deuteronomy", "Dt", "D't", "Deut", "De" },
            34
        );
        static BookInfo B06 = new BookInfo(
            new string[] { "Joshua", "Js", "Jsh", "Josh", "Jos" },
            24
        );
        static BookInfo B07 = new BookInfo(
            new string[] { "Judges", "Jg", "Jdg", "Judg", "Jdgs" },
            21
        );
        static BookInfo B08 = new BookInfo(
            new string[] { "Ruth", "Ru", "Rth", "Ruth", "Rut" },
            4
        );
        static BookInfo B09 = new BookInfo(
            new string[] { "1 Samuel", "1S", "1Sm", "1Sam", "1Sa" },
            31
        );
        static BookInfo B10 = new BookInfo(
            new string[] { "2 Samuel", "2S", "2Sm", "2Sam", "2Sa" },
            24
        );
        static BookInfo B11 = new BookInfo(
            new string[] { "1 Kings", "1K", "1Ki", "1Kgs", "1Kg", "1Kin" },
            22
        );
        static BookInfo B12 = new BookInfo(
            new string[] { "2 Kings", "2K", "2Ki", "2Kgs", "2Kg", "2Kin" },
            25
        );
        static BookInfo B13 = new BookInfo(
            new string[] { "1 Chronicles", "", "1Ch", "1Chr", "1Chron" },
            29
        );
        static BookInfo B14 = new BookInfo(
            new string[] { "2 Chronicles", "", "2Ch", "2Chr", "2Chron" },
            36
        );
        static BookInfo B15 = new BookInfo(
            new string[] { "Ezra", "", "Ezr", "Ezra" },
            10
        );
        static BookInfo B16 = new BookInfo(
            new string[] { "Nehemiah", "Ne", "Neh", "Neh" },
            13
        );
        static BookInfo B17 = new BookInfo(
            new string[] { "Esther", "Es", "Est", "Est", "Esth" },
            10
        );
        static BookInfo B18 = new BookInfo(
            new string[] { "Job", "Jb" },
            42
        );
        static BookInfo B19 = new BookInfo(
            new string[] { "Psalms", "Ps", "Psa", "Pslm", "Psm", "Pss" },
            150
        );
        static BookInfo B20 = new BookInfo(
            new string[] { "Proverbs", "Pr", "Pro", "Prov", "Prv" },
            31
        );
        static BookInfo B21 = new BookInfo(
            new string[] { "Ecclesiastes", "Ec", "Ecc", "Eccl", "Eccle", "Qoh" },
            12
        );
        static BookInfo B22 = new BookInfo(
            new string[] { "Song of Solomon", "So", "SoS", "Song", "SS", "Cant" },
            8
        );
        static BookInfo B23 = new BookInfo(
            new string[] { "Isaiah", "Is", "Isa" },
            66
        );
        static BookInfo B24 = new BookInfo(
            new string[] { "Jeremiah", "Je", "Jer", "Jer", "Jeremy", "Jr" },
            52
        );
        static BookInfo B25 = new BookInfo(
            new string[] { "Lamentations", "La", "Lam" },
            5
        );
        static BookInfo B26 = new BookInfo(
            new string[] { "Ezekiel", "", "Eze", "Ezek", "Ezk" },
            48
        );
        static BookInfo B27 = new BookInfo(
            new string[] { "Daniel", "Da", "Dan", "Dan", "Dn" },
            12
        );
        static BookInfo B28 = new BookInfo(
            new string[] { "Hosea", "Ho", "Hos" },
            14
        );
        static BookInfo B29 = new BookInfo(
            new string[] { "Joel", "Jl", "Jol", "Joe" },
            3
        );
        static BookInfo B30 = new BookInfo(
            new string[] { "Amos", "Am", "Amo" },
            9
        );
        static BookInfo B31 = new BookInfo(
            new string[] { "Obadiah", "Ob", "Obd", "Obad" },
            1
        );
        static BookInfo B32 = new BookInfo(
            new string[] { "Jonah", "Jnh", "Jona" },
            4
        );
        static BookInfo B33 = new BookInfo(
            new string[] { "Micah", "Mc", "Mic", "Mica", "Mi" },
            7
        );
        static BookInfo B34 = new BookInfo(
            new string[] { "Nahum", "Na", "Nah" },
            3
        );
        static BookInfo B35 = new BookInfo(
            new string[] { "Habakkuk", "Hb", "Hab" },
            3
        );
        static BookInfo B36 = new BookInfo(
            new string[] { "Zephaniah", "Zp", "Zep", "Zeph", "Zph" },
            3
        );
        static BookInfo B37 = new BookInfo(
            new string[] { "Haggai", "Hg", "Hag" },
            2
        );
        static BookInfo B38 = new BookInfo(
            new string[] { "Zechariah", "Zc", "Zec", "Zech", "Zch" },
            14
        );
        static BookInfo B39 = new BookInfo(
            new string[] { "Malachi", "Ml", "Mal" },
            4
        );
        static BookInfo B40 = new BookInfo(
            new string[] { "Matthew", "Mt", "Mat", "Matt" },
            28
        );
        static BookInfo B41 = new BookInfo(
            new string[] { "Mark", "Mk", "Mrk" },
            16
        );
        static BookInfo B42 = new BookInfo(
            new string[] { "Luke", "Lk", "Luk", "Lu" },
            24
        );
        static BookInfo B43 = new BookInfo(
            new string[] { "John", "Jn", "Jhn", "John", "Joh" },
            21
        );
        static BookInfo B44 = new BookInfo(
            new string[] { "Acts", "Ac", "Act", },
            28
        );
        static BookInfo B45 = new BookInfo(
            new string[] { "Romans", "Ro", "Rom", "Rm" },
            16
        );
        static BookInfo B46 = new BookInfo(
            new string[] { "1 Corinthians", "1Co", "1Cor" },
            16
        );
        static BookInfo B47 = new BookInfo(
            new string[] { "2 Corinthians", "2Co", "2Cor" },
            13
        );
        static BookInfo B48 = new BookInfo(
            new string[] { "Galatians", "Ga", "Gal" },
            6
        );
        static BookInfo B49 = new BookInfo(
            new string[] { "Ephesians", "Ep", "Eph" },
            6
        );
        static BookInfo B50 = new BookInfo(
            new string[] { "Philippians", "Pp", "Php", "Phil", "Philip" },
            4
        );
        static BookInfo B51 = new BookInfo(
            new string[] { "Colossians", "Co", "Col", "Col" },
            4
        );
        static BookInfo B52 = new BookInfo(
            new string[] { "1 Thessalonians", "1Th", "1Th", "1Thess", "1Thes" },
            5
        );
        static BookInfo B53 = new BookInfo(
            new string[] { "2 Thessalonians", "2Th", "2Th", "2Thess", "2Thes" },
            3
        );
        static BookInfo B54 = new BookInfo(
            new string[] { "1 Timothy", "1Ti", "1Tim" },
            6
        );
        static BookInfo B55 = new BookInfo(
            new string[] { "2 Timothy", "2Ti", "2Tim" },
            4
        );
        static BookInfo B56 = new BookInfo(
            new string[] { "Titus", "Ti" },
            3
        );
        static BookInfo B57 = new BookInfo(
            new string[] { "Philemon", "Phm", "Phm", "Philem" },
            1
        );
        static BookInfo B58 = new BookInfo(
            new string[] { "Hebrews", "Heb", "Hbr", "Hbrws" },
            13
        );
        static BookInfo B59 = new BookInfo(
            new string[] { "James", "Jm", "Jam" },
            5
        );
        static BookInfo B60 = new BookInfo(
            new string[] { "1 Peter", "1P", "1Pe", "1Pet", "1Pt" },
            5
        );
        static BookInfo B61 = new BookInfo(
            new string[] { "2 Peter", "2P", "2Pe", "2Pet", "2Pt" },
            3
        );
        static BookInfo B62 = new BookInfo(
            new string[] { "1 John", "1J", "1Jn", "1Jhn" },
            5
        );
        static BookInfo B63 = new BookInfo(
            new string[] { "2 John", "2J", "2Jn", "2Jhn" },
            1
        );
        static BookInfo B64 = new BookInfo(
            new string[] { "3 John", "3J", "3Jn", "3Jhn" },
            1
        );
        static BookInfo B65 = new BookInfo(
            new string[] { "Jude", "Jd" },
            1
        );
        static BookInfo B66 = new BookInfo(
            new string[] { "Revelation", "Re", "Rev", "Rv" },
            22
        );
        public static BookInfo[] Metrics { get; private set; } = new BookInfo[] {
            null,
            B01, B02, B03, B04, B05, B06, B07, B08, B09, B10,
            B11, B12, B13, B14, B15, B16, B17, B18, B19, B20,
            B21, B22, B23, B24, B25, B26, B27, B28, B29, B30,
            B31, B32, B33, B34, B35, B36, B37, B38, B39, B40,
            B41, B42, B43, B44, B45, B46, B47, B48, B49, B50,
            B51, B52, B53, B54, B55, B56, B57, B58, B59, B60,
            B61, B62, B63, B64, B65, B66
        };

        // hijacked column of data from old av-chapter.ix ascii representation (in Z-Series of Digital-AV repo)
        static byte[] RunningListOfVerseCounts =
        {
                31,
                25,
                24,
                26,
                32,
                22,
                24,
                22,
                29,
                32,
                32,
                20,
                18,
                24,
                21,
                16,
                27,
                33,
                38,
                18,
                34,
                24,
                20,
                67,
                34,
                35,
                46,
                22,
                35,
                43,
                55,
                32,
                20,
                31,
                29,
                43,
                36,
                30,
                23,
                23,
                57,
                38,
                34,
                34,
                28,
                34,
                31,
                22,
                33,
                26,
                22,
                25,
                22,
                31,
                23,
                30,
                25,
                32,
                35,
                29,
                10,
                51,
                22,
                31,
                27,
                36,
                16,
                27,
                25,
                26,
                36,
                31,
                33,
                18,
                40,
                37,
                21,
                43,
                46,
                38,
                18,
                35,
                23,
                35,
                35,
                38,
                29,
                31,
                43,
                38,
                17,
                16,
                17,
                35,
                19,
                30,
                38,
                36,
                24,
                20,
                47,
                8,
                59,
                57,
                33,
                34,
                16,
                30,
                37,
                27,
                24,
                33,
                44,
                23,
                55,
                46,
                34,
                54,
                34,
                51,
                49,
                31,
                27,
                89,
                26,
                23,
                36,
                35,
                16,
                33,
                45,
                41,
                50,
                13,
                32,
                22,
                29,
                35,
                41,
                30,
                25,
                18,
                65,
                23,
                31,
                40,
                16,
                54,
                42,
                56,
                29,
                34,
                13,
                46,
                37,
                29,
                49,
                33,
                25,
                26,
                20,
                29,
                22,
                32,
                32,
                18,
                29,
                23,
                22,
                20,
                22,
                21,
                20,
                23,
                30,
                25,
                22,
                19,
                19,
                26,
                68,
                29,
                20,
                30,
                52,
                29,
                12,
                18,
                24,
                17,
                24,
                15,
                27,
                26,
                35,
                27,
                43,
                23,
                24,
                33,
                15,
                63,
                10,
                18,
                28,
                51,
                9,
                45,
                34,
                16,
                33,
                36,
                23,
                31,
                24,
                31,
                40,
                25,
                35,
                57,
                18,
                40,
                15,
                25,
                20,
                20,
                31,
                13,
                31,
                30,
                48,
                25,
                22,
                23,
                18,
                22,
                28,
                36,
                21,
                22,
                12,
                21,
                17,
                22,
                27,
                27,
                15,
                25,
                23,
                52,
                35,
                23,
                58,
                30,
                24,
                42,
                15,
                23,
                29,
                22,
                44,
                25,
                12,
                25,
                11,
                31,
                13,
                27,
                32,
                39,
                12,
                25,
                23,
                29,
                18,
                13,
                19,
                27,
                31,
                39,
                33,
                37,
                23,
                29,
                33,
                43,
                26,
                22,
                51,
                39,
                25,
                53,
                46,
                28,
                34,
                18,
                38,
                51,
                66,
                28,
                29,
                43,
                33,
                34,
                31,
                34,
                34,
                24,
                46,
                21,
                43,
                29,
                53,
                18,
                25,
                27,
                44,
                27,
                33,
                20,
                29,
                37,
                36,
                21,
                21,
                25,
                29,
                38,
                20,
                41,
                37,
                37,
                21,
                26,
                20,
                37,
                20,
                30,
                54,
                55,
                24,
                43,
                26,
                81,
                40,
                40,
                44,
                14,
                47,
                40,
                14,
                17,
                29,
                43,
                27,
                17,
                19,
                8,
                30,
                19,
                32,
                31,
                31,
                32,
                34,
                21,
                30,
                17,
                18,
                17,
                22,
                14,
                42,
                22,
                18,
                31,
                19,
                23,
                16,
                22,
                15,
                19,
                14,
                19,
                34,
                11,
                37,
                20,
                12,
                21,
                27,
                28,
                23,
                9,
                27,
                36,
                27,
                21,
                33,
                25,
                33,
                27,
                23,
                11,
                70,
                13,
                24,
                17,
                22,
                28,
                36,
                15,
                44,
                11,
                20,
                32,
                23,
                19,
                19,
                73,
                18,
                38,
                39,
                36,
                47,
                31,
                22,
                23,
                15,
                17,
                14,
                14,
                10,
                17,
                32,
                3,
                22,
                13,
                26,
                21,
                27,
                30,
                21,
                22,
                35,
                22,
                20,
                25,
                28,
                22,
                35,
                22,
                16,
                21,
                29,
                29,
                34,
                30,
                17,
                25,
                6,
                14,
                23,
                28,
                25,
                31,
                40,
                22,
                33,
                37,
                16,
                33,
                24,
                41,
                30,
                24,
                34,
                17,
                6,
                12,
                8,
                8,
                12,
                10,
                17,
                9,
                20,
                18,
                7,
                8,
                6,
                7,
                5,
                11,
                15,
                50,
                14,
                9,
                13,
                31,
                6,
                10,
                22,
                12,
                14,
                9,
                11,
                12,
                24,
                11,
                22,
                22,
                28,
                12,
                40,
                22,
                13,
                17,
                13,
                11,
                5,
                26,
                17,
                11,
                9,
                14,
                20,
                23,
                19,
                9,
                6,
                7,
                23,
                13,
                11,
                11,
                17,
                12,
                8,
                12,
                11,
                10,
                13,
                20,
                7,
                35,
                36,
                5,
                24,
                20,
                28,
                23,
                10,
                12,
                20,
                72,
                13,
                19,
                16,
                8,
                18,
                12,
                13,
                17,
                7,
                18,
                52,
                17,
                16,
                15,
                5,
                23,
                11,
                13,
                12,
                9,
                9,
                5,
                8,
                28,
                22,
                35,
                45,
                48,
                43,
                13,
                31,
                7,
                10,
                10,
                9,
                8,
                18,
                19,
                2,
                29,
                176,
                7,
                8,
                9,
                4,
                8,
                5,
                6,
                5,
                6,
                8,
                8,
                3,
                18,
                3,
                3,
                21,
                26,
                9,
                8,
                24,
                13,
                10,
                7,
                12,
                15,
                21,
                10,
                20,
                14,
                9,
                6,
                33,
                22,
                35,
                27,
                23,
                35,
                27,
                36,
                18,
                32,
                31,
                28,
                25,
                35,
                33,
                33,
                28,
                24,
                29,
                30,
                31,
                29,
                35,
                34,
                28,
                28,
                27,
                28,
                27,
                33,
                31,
                18,
                26,
                22,
                16,
                20,
                12,
                29,
                17,
                18,
                20,
                10,
                14,
                17,
                17,
                11,
                16,
                16,
                13,
                13,
                14,
                31,
                22,
                26,
                6,
                30,
                13,
                25,
                22,
                21,
                34,
                16,
                6,
                22,
                32,
                9,
                14,
                14,
                7,
                25,
                6,
                17,
                25,
                18,
                23,
                12,
                21,
                13,
                29,
                24,
                33,
                9,
                20,
                24,
                17,
                10,
                22,
                38,
                22,
                8,
                31,
                29,
                25,
                28,
                28,
                25,
                13,
                15,
                22,
                26,
                11,
                23,
                15,
                12,
                17,
                13,
                12,
                21,
                14,
                21,
                22,
                11,
                12,
                19,
                12,
                25,
                24,
                19,
                37,
                25,
                31,
                31,
                30,
                34,
                22,
                26,
                25,
                23,
                17,
                27,
                22,
                21,
                21,
                27,
                23,
                15,
                18,
                14,
                30,
                40,
                10,
                38,
                24,
                22,
                17,
                32,
                24,
                40,
                44,
                26,
                22,
                19,
                32,
                21,
                28,
                18,
                16,
                18,
                22,
                13,
                30,
                5,
                28,
                7,
                47,
                39,
                46,
                64,
                34,
                22,
                22,
                66,
                22,
                22,
                28,
                10,
                27,
                17,
                17,
                14,
                27,
                18,
                11,
                22,
                25,
                28,
                23,
                23,
                8,
                63,
                24,
                32,
                14,
                49,
                32,
                31,
                49,
                27,
                17,
                21,
                36,
                26,
                21,
                26,
                18,
                32,
                33,
                31,
                15,
                38,
                28,
                23,
                29,
                49,
                26,
                20,
                27,
                31,
                25,
                24,
                23,
                35,
                21,
                49,
                30,
                37,
                31,
                28,
                28,
                27,
                27,
                21,
                45,
                13,
                11,
                23,
                5,
                19,
                15,
                11,
                16,
                14,
                17,
                15,
                12,
                14,
                16,
                9,
                20,
                32,
                21,
                15,
                16,
                15,
                13,
                27,
                14,
                17,
                14,
                15,
                21,
                17,
                10,
                10,
                11,
                16,
                13,
                12,
                13,
                15,
                16,
                20,
                15,
                13,
                19,
                17,
                20,
                19,
                18,
                15,
                20,
                15,
                23,
                21,
                13,
                10,
                14,
                11,
                15,
                14,
                23,
                17,
                12,
                17,
                14,
                9,
                21,
                14,
                17,
                18,
                6,
                25,
                23,
                17,
                25,
                48,
                34,
                29,
                34,
                38,
                42,
                30,
                50,
                58,
                36,
                39,
                28,
                27,
                35,
                30,
                34,
                46,
                46,
                39,
                51,
                46,
                75,
                66,
                20,
                45,
                28,
                35,
                41,
                43,
                56,
                37,
                38,
                50,
                52,
                33,
                44,
                37,
                72,
                47,
                20,
                80,
                52,
                38,
                44,
                39,
                49,
                50,
                56,
                62,
                42,
                54,
                59,
                35,
                35,
                32,
                31,
                37,
                43,
                48,
                47,
                38,
                71,
                56,
                53,
                51,
                25,
                36,
                54,
                47,
                71,
                53,
                59,
                41,
                42,
                57,
                50,
                38,
                31,
                27,
                33,
                26,
                40,
                42,
                31,
                25,
                26,
                47,
                26,
                37,
                42,
                15,
                60,
                40,
                43,
                48,
                30,
                25,
                52,
                28,
                41,
                40,
                34,
                28,
                41,
                38,
                40,
                30,
                35,
                27,
                27,
                32,
                44,
                31,
                32,
                29,
                31,
                25,
                21,
                23,
                25,
                39,
                33,
                21,
                36,
                21,
                14,
                23,
                33,
                27,
                31,
                16,
                23,
                21,
                13,
                20,
                40,
                13,
                27,
                33,
                34,
                31,
                13,
                40,
                58,
                24,
                24,
                17,
                18,
                18,
                21,
                18,
                16,
                24,
                15,
                18,
                33,
                21,
                14,
                24,
                21,
                29,
                31,
                26,
                18,
                23,
                22,
                21,
                32,
                33,
                24,
                30,
                30,
                21,
                23,
                29,
                23,
                25,
                18,
                10,
                20,
                13,
                18,
                28,
                12,
                17,
                18,
                20,
                15,
                16,
                16,
                25,
                21,
                18,
                26,
                17,
                22,
                16,
                15,
                15,
                25,
                14,
                18,
                19,
                16,
                14,
                20,
                28,
                13,
                28,
                39,
                40,
                29,
                25,
                27,
                26,
                18,
                17,
                20,
                25,
                25,
                22,
                19,
                14,
                21,
                22,
                18,
                10,
                29,
                24,
                21,
                21,
                13,
                14,
                25,
                20,
                29,
                22,
                11,
                14,
                17,
                17,
                13,
                21,
                11,
                19,
                17,
                18,
                20,
                8,
                21,
                18,
                24,
                21,
                15,
                27,
                21
        };
    }
}
