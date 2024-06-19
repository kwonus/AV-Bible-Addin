using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVX
{
    public class BibleResult : List<BookResult>
    {
        public BibleResult() : base()
        {
            ;
        }
        public byte BookCnt
        {
            get
            {
                return (byte)this.Count;
            }
        }
        public UInt16 ChapterCnt
        {
            get
            {
                UInt16 cnt = 0;
                foreach (var bk in this)
                    cnt += bk.ChapterCnt;
                return cnt;
            }
        }
        public UInt16 VerseCnt
        {
            get
            {
                UInt16 cnt = 0;
                foreach (var bk in this)
                    cnt += bk.VerseCnt;
                return cnt;
            }
        }
    }
    public class BookResult : List<ChapterResult>
    {
        public BookResult(byte b) : base()
        {
            this.B = b;
        }
        public byte B { get; private set; }
        public BookInfo Info
        {
            get
            {
                if (this.B >= 1 && this.B <= 66)
                    return BookInfo.GetBook(this.B);
                return null;
            }
        }

        public UInt16 ChapterCnt
        {
            get
            {
                return (UInt16)this.Count;
            }
        }
        public UInt16 VerseCnt
        {
            get
            {
                UInt16 cnt = 0;
                foreach (var ch in this)
                    cnt += ch.VerseCnt;
                return cnt;
            }
        }
    }
    public class ChapterResult : List<VerseResult>
    {
        public ChapterResult(byte c) : base()
        {
            this.C = c;
        }
        public byte C { get; private set; }

        public UInt16 VerseCnt
        {
            get
            {
                return (UInt16)this.Count;
            }
        }
    }
    public class VerseResult
    {
        public VerseResult(byte v) : base()
        {
            this.V = v;
        }
        public byte V { get; private set; }
    }
}
