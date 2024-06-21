using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVX.Serialization
{
    public class DataStream
    {
        public BCVW Coordinates;
        public UInt32 WordKey;
        public PNPOS PnPos;
        public string NuPos;
        public string Text;   // KJV
        public string Modern; // AVX
        public bool Modernized;
        public byte Punctuation;
        public Dictionary<UInt32, string> Triggers;

        public DataStream()
        {
            this.WordKey = 0;
            this.Coordinates = new BCVW();
            this.NuPos = null;
            this.Text = null;   // KJV
            this.Modern = null; // AVX
            this.Punctuation = 0;
            this.PnPos = null;
            this.Triggers = null;
        }
        public static (DataStream[] verses, Exception exception) CreateFromYaml(string yaml)
        {
            try
            {
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                return (deserializer.Deserialize<DataStream[]>(yaml), null);
            }
            catch (Exception ex)
            {
                return (new DataStream[0], ex);
            }
        }
        public static (DataStream[] verses, Exception exception) CreateFromYaml(TextReader yaml, bool singleton)
        {
            try
            {
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                return (deserializer.Deserialize<DataStream[]>(yaml), null);
            }
            catch (Exception ex)
            {
                return (new DataStream[0], ex);
            }
        }
    }
}

