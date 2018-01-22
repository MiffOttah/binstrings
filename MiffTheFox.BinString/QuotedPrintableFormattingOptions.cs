using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    [Flags]
    public enum QuotedPrintableFormattingOptions
    {
        Default = 0,
        LowerCaseHex = 0b1,
        KeepNewlines = 0b10,
        DontEnforceLineLengthLimit = 0b100
    }
}
