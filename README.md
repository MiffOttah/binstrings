# MiffTheFox.BinString

Byte arrays are commonplace in .NET code for repersenting data that isn't just
text. But arrays aren't really easy to work with. There are no standard methods
for searching, concatenation, padding, or doing any other operation to a byte
array that you could easily do with a string.

This library aims to make working with binary data as easy as working with
strings by introducing the `BinString` type. A BinString is a string of bytes
that can be manipulated along the lines of a string of characters.

BinStrings also contain methods for converting binary data to and from integer
types (with endianness checks!), text strings, and strings of encoded data in
formats such as base32 and ascii85.

The library is available as a build for .NET Framework, .NET Standard, and .NET
Core, so it can be used in any sort of .NET environment.

The library is being rewritten for version 2.0 with new changes:

* New extensible BinaryTextEncoding system for adding new string encoding/decoding.
* Native .NET core support including Span and Index.
* License changed to MIT from GPL.

# Quick examples

## Comparing two files

    var file1 = new BinString(File.ReadAllBytes("file1.bin"));
    var file2 = new BinString(File.ReadAllBytes("file2.bin"));
    Console.WriteLine(file1 == file2 ? "Files are identical" : "Files are not identical");

## Concatenating binstrings

    var a = new BinString("Hello", Encoding.ASCII));
    var b = BinString.FromBytes("2c20");
    var c = new BinString("world%21", BinaryTextEncoding.UrlString);
    var message = a + b + c;
    Console.WriteLine(message.ToString(Encoding.ASCII));

## Base32 encoding

    var base32 = new MiffTheFox.BinaryTextEncodings.Base32();
    var data = new BinString("Hello", Encoding.ASCII);
    Console.WriteLine(data.ToString(base32));

## Extension methods for IO

    using (var fileStream = File.OpenRead(file)){
        var prefix = fileStream.ReadBinString(32);
    }

## Practical example: Determine if a file is a PNG

    static readonly BinString PNG_MAGIC_NUMBER = BinString.FromBytes("89 50 4e 47 0d 0a 1a 0a");
    public static bool IsPng(string file)
    {
        using (var fileStream = File.OpenRead(file)){
            var prefix = fileStream.ReadBinString(PNG_MAGIC_NUMBER.Length);
            return prefix == PNG_MAGIC_NUMBER;
        }
    }

## Practical example: Temporary file name

    public static string TemporaryFileName(string original){
        // The original string can't be trusted, because it might
        // contain invalid characters or names.
        var hash = MD5.Create().ComputeHash(new BinString(original, Encoding.UTF8));
        var timestamp = BinString.FromInt64(DateTime.Now.Ticks, IntegerEndianess.BigEndian);
        var base32 = new Base32(Base32.CHARSET_ZBASE32) { UsePadding = false };
        return (hash + timestamp).ToString(base32);
    }

# Full documentation

[Full documentation will be made available on Github Pages.](https://miffottah.github.io/binstrings)
