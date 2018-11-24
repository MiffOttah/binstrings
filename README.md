# MiffTheFox.BinString

Byte arrays are commonplace in .NET code for repersenting data that isn't just
text. But arrays aren't really easy to work with. There are no standard methods
for searching, concatenation, padding, or doing any other operation to a byte
array that you could easily do with a string.

This library aims to make working with binary data as easy as working with
strings by introducing the `BinString` type. A BinString is a string of bytes
that can be manipulated along the lines of a string of characters.

# Usage

Create a BinString

    byte[] hello = Encoding.UTF8.GetBytes("Hello, world!");
    BinString helloBin = new BinString(hello);
    // or
    var helloBin = new BinString("Hello, world!", Encoding.UTF8);

Use a BinString

    var sha = SHA1.Create();
    var result = new BinString(sha.ComputeHash(helloBin));

Format a BinString for display

    Console.WriteLine("The hash is: {0:x}", result);

Concatenation

    var a = new BinString("Hello", Encoding.ASCII));
    var b = BinString.FromBytes("2c20");
    var c = BinString.FromUrlString("world%21");
    var message = a + b + c;
    Console.WriteLine(message.ToString(Encoding.ASCII));

Equality testing

    var a = BinString.FromTextString("ABC", Encoding.ASCII);
    var b = BinString.FromBytes("414243");
    if (a == b) Console.WriteLine("They match!");

Known-endianess conversion from longer types.

	uint color = 0xff8000ff;
	var colorBin = BinString.FromUInt32(color, IntegerEndianess.BigEndian);
	byte a = color[0], r = color[1], g = color[2], b = color[3];
	return Color.FromArgb(a, r, g, b);


[More documentation is available on the wiki.](https://github.com/MiffOttah/binstrings/wiki)

# Installation

[You can install the package through NuGet with this link](https://www.nuget.org/packages/MiffTheFox.BinString/) or by typing the following command into the Pacakage Manage Console:

    Install-Package MiffTheFox.BinString

Have fun!~
