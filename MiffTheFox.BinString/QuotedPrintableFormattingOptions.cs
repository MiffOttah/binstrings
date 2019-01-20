using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    /// <summary>
    /// Specified options for Quoted-Printable encoding of a BinString.
    /// </summary>
    [Flags]
    public enum QuotedPrintableFormattingOptions
    {
        /// <summary>
        /// The default options. Hexadecimal digits are upper case, newlines in the text are encoded,
        /// lines greater then 75 characters in length are broken up, and the newline characater is
        /// the system default.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Hexadecimal digits are lower case, instead of upper case.
        /// </summary>
        LowerCaseHex = 0b1,

        /// <summary>
        /// Newlines in the input text are preserved, rater than encoded
        /// </summary>
        KeepNewlines = 0b10,

        /// <summary>
        /// Newlines are not added automatically to keep output lines under 75 characters in length (as specified by QP)
        /// </summary>
        DontEnforceLineLengthLimit = 0b100,

        /// <summary>
        /// Uses CR as a newline. If no newline flags are set, defaults to the system's default.
        /// </summary>
        UseCr = 0b1000,

        /// <summary>
        /// Uses LF as a newline. If no newline flags are set, defaults to the system's default.
        /// </summary>
        UseLf = 0b10000,

        /// <summary>
        /// Uses CR+LF as a newline. If no newline flags are set, defaults to the system's default.
        /// </summary>
        UseCrLf = UseCr | UseLf
    }
}
