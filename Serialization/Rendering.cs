using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVX.Serialization
{
     public struct Written
    {
        public STRONGS Strongs; // UInt64 accessible as UInt16[]
        public BCVW BCVWc;   // UInt32 accessible as Byte[]
        public UInt16 WordKey;
        public UInt16 pnPOS12;
        public UInt32 POS32;
        public UInt16 Lemma;
        public byte Punctuation;
        public byte Transition;
    }

    public class ChapterRendering
    {
        public string BookName;
        public string BookAbbreviation3;
        public string BookAbbreviation4;
        public byte BookNumber;
        public byte ChapterNumber;
        public Dictionary<byte, VerseRendering> Verses;
        public static ChapterRendering CreateFromYaml(string yaml)
        {
            try
            {
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                return deserializer.Deserialize<ChapterRendering>(yaml);
            }
            catch (Exception e)
            {
                ;
            }
            return null;
        }
        public static ChapterRendering CreateFromYaml(TextReader yaml)
        {
            try
            {
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                return deserializer.Deserialize<ChapterRendering>(yaml);
            }
            catch (Exception e)
            {
                ;
            }
            return null;
        }
    }
    public class VerseRendering
    {
        public BCVW Coordinates;
        public WordRendering[] Words;

        public VerseRendering(BCVW coordinates)
        {
            this.Coordinates = coordinates;
            this.Words = new WordRendering[coordinates.WC];
        }
        public static VerseRendering CreateFromYaml(string yaml)
        {
            try
            {
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                return deserializer.Deserialize<VerseRendering>(yaml);
            }
            catch (Exception e)
            {
                ;
            }
            return null;
        }
        public static VerseRendering CreateFromYaml(TextReader yaml)
        {
            try
            {
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                return deserializer.Deserialize<VerseRendering>(yaml);
            }
            catch (Exception e)
            {
                ;
            }
            return null;
        }
    }
    public class WordRendering
    {
        public UInt32 WordKey;
        public BCVW Coordinates;
        public PNPOS PnPos;
        public string NuPos;
        public string Text;   // KJV
        public string Modern; // AVX
        public bool Modernized { get => !this.Text.Equals(this.Modern, StringComparison.InvariantCultureIgnoreCase); }
        public byte Punctuation;
        public Dictionary<UInt32, string> Triggers;       // <highlight-id, feature-match-string>

        public WordRendering()
        {
            this.WordKey = 0;
            this.Coordinates = new BCVW();
            this.Text = string.Empty;
            this.Modern = string.Empty;
            this.Punctuation = 0;
            this.Triggers = new Dictionary<UInt32, string>();
            this.PnPos = new PNPOS();
            this.NuPos = string.Empty;
        }
    }
}

