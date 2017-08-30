# MiffTheFox.BinString

This is a .NET library for manipluating sequences of `System.Byte` as easily as a typical string (a sequence of `System.Char`).

# Usage

Create a BinString

    byte[] hello = Encoding.UTF8.GetBytes("Hello, world!");
    BinString helloBin = new BinString(hello);
    // or
    var helloBin = BinString.FromTextString("Hello, world!", Encoding.UTF8);

Use a BinString

    var sha = SHA1.Create();
    var result = new BinString(sha.ComputeHash(helloBin));

Format a BinString for display

    Console.WriteLine("The hash is: {0:x}", result);

Build a BinString with a BinStringBuilder

    var builder = new BinStringBuilder();
    builder.Append(BinString.FromTextString("Hello", Encoding.ASCII));
    builder.Append(0x2c);
    builder.Append(0x20);
    builder.Append(Encoding.ASCII.GetBytes("world!"));

    var myString = builder.ToBinString();
    Console.WriteLine(myString.ToString(Encoding.ASCII));

I still need to wirte more expansive docs, but `BinStringTests.cs` and the XML comments in the source files should give examples of usage.

# Installation

Ok, so this is my first NuGet package so I hope it works.

[You can install the package through NuGet with this link](https://www.nuget.org/packages/MiffTheFox.BinString/) or by typing the following command into the Pacakage Manage Console:

    Install-Package MiffTheFox.BinString -Version 1.0.0

And I don't use .NET core but if you do this installation command might work:

    dotnet add package MiffTheFox.BinString --version 1.0.0

Have fun! \^_\^
