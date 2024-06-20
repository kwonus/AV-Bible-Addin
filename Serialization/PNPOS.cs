using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVX.Serialization
{
    public enum NUMBER
    {
        Unknown = 0,
        Singular = 1,
        Plural = 2,
    };
    public enum GENDER
    {
        Neuter = -1,
        Unknown = 0,
        Male = 1,
        Female = 2,
    };
    public enum CASE
    {
        Unknown = 0,
        Genitive = 1,
        Nominative = 2,
        Objective = 3,
        Reflexive = 4,
    };
    public class PNPOS
    {
        public byte person;
        public NUMBER number;
        public GENDER gender;
        public CASE case_;
        public string pos;

        public PNPOS()
        {
            this.person = 0;
            this.number = NUMBER.Unknown;
            this.gender = GENDER.Unknown;
            this.case_ = CASE.Unknown;
            this.pos = string.Empty;
        }
    }

}
