# Format strings

BinStrings are [formattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable).

The following format strings apply.

| Format String | Description | Example | Also provided by |
| ------------- | ----------- | ------- | ---------------- |
| `x` or `X`    | Hexadecimal encoding. Each byte is written as two characters of hex. If the format string is followed by a string, that string will be used as separator. Otherwise, there will be no separator. | `546573740d0a` | *(n/a)* |
| `s` or `S`    | Same as `x`, except the separator is a space. Since format strings cannot end in spaces, this can be used to produce space-separated hex. | `54 65 73 74 0d 0a` | *(n/a)* |
| `u` or `U`    | URL encoding. All bytes from 0x21 to 0x7e, except 0x2b (ASCII `%`) are repersented by their ASCII characters. All other bytes are represented with a `%`, followed by two hexadecimal characters. This is similar to the URL encoding specified by [RFC 1738](https://tools.ietf.org/html/rfc1738). | `Test%0d%0a` | [BinaryTextEncodings.UrlEncoding](../api/MiffTheFox.BinaryTextEncodings.UrlEncoding.html) |
| `e` or `E`    | Backslash encoding. All bytes from 0x21 to 0x7e, except 0x5c, 0x22, and 0x27 (ASCII `\\`, `"` and `'`) are repersented by their ASCII characters. 0x5c, is represented by two backslashes (`\\\\`). All other bytes are represented with a `\\x`, followed by two hexadecimal characters.  | `Test\x0d\x0a` | [BinaryTextEncodings.BackslashEscapeEncoding](../api/MiffTheFox.BinaryTextEncodings.BackslashEscapeEncoding.html) |
| `q` or `Q`    | Quoted-Printable encoding. | `Test=0d=0a` | [BinaryTextEncodings.QuotedPrintable](../api/MiffTheFox.BinaryTextEncodings.QuotedPrintable.html) |
| `64`          | Base64 encoding. | `VGVzdA0K` | [BinaryTextEncodings.Base64](../api/MiffTheFox.BinaryTextEncodings.Base64.html) |
| `85`          | ASCII-85 encoding. | `<~<+U,m%13~>` | [BinaryTextEncodings.Ascii85](../api/MiffTheFox.BinaryTextEncodings.Ascii85.html) |

With the `x`/`X`, `s`/`S`, `u`/`U`, `e`/`E`, and `q`/`Q` options, the case of
the character indicates the case of the hexadecimal representations.

A null or empty format string, the general format string `"G"`/`"g"`, or
calling the parameterless `ToString` method, defaults to `"x"`.
