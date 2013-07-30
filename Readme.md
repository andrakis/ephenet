Ephenet - Ephemeral Network
---------------------------

Provides a virtual network interface and network layer.

Currently supports:
 * Terminals - low level terminals (think the RJ45 connector)
 * Hubs - consisting of multiple Terminals, and forwarding received
   traffic to each of its other Terminals.

Todo:
 * Code was written in a hurry, sparsely commented. Fix.
 * Unit tests.
 * MonoDevelop compatiblity.
 * A network protocol (like UDP or TCP) so that apps can start to make
   use of the virtual network.

A while away:
 * A network interface driver to allow real TCP/IP traffic over the
   virtual network (Windows.)
 * The above for Linux/BSD hosts.
 * The above for virtualization packages


Requirements
------------

* Visual Studio 2010 or 2012 (Express editions are fine)
* .NET 4.0
* [GenSharp](https://github.com/andrakis/gensharp) (included as a submodule)

Note: the project aims to be compatible with MonoDevelop, however this
compatibility will be in a future release.


Usage
-----

Please initialize the submodule(s):
```
$ git submodule update --init
```

From time to time you may also need to update the submodule(s):
```
$ git submodule update
```

See Ephenet.Sample/Sample.cs for a preliminary example.
Once there is a network protocol, there will be more samples, as well
as unit tests.


By
--
Julian "Andrakis" Thatcher <julian@noblesamurai.com>


License - MIT License
---------------------
Copyright (C) 2013 Julian Thatcher

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
