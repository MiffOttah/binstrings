# MiffTheFox.BinString

Byte arrays are commonplace in .NET code for repersenting data that isn't just
text. But arrays aren't really easy to work with. There are no standard methods
for searching, concatenation, padding, or doing any other operation to a byte
array that you could easily do with a string.

This library aims to make working with binary data as easy as working with
strings by introducing the BinString type. A BinString is a string of bytes
that can be manipulated along the lines of a string of characters.

BinStrings also contain methods for converting binary data to and from integer
types (with endianness checks!), text strings, and strings of encoded data in
formats such as base32 and ascii85.

The library is available as a build for .NET Framework, .NET Standard, and
.NET Core, so it can be used in any sort of .NET environment.

<!-- TODO: Links to NuGet packages. -->
