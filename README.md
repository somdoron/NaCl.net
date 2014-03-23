# NaCl.net

NaCl.net (pronounced "salt dotnet") is C# implementation of [NaCl](http://nacl.cr.yp.to/) by Daniel J. Bernstein.

NaCl (pronounced "salt") is a new easy-to-use high-speed software library for network communication, encryption, decryption, signatures, etc. 
NaCl's goal is to provide all of the core operations needed to build higher-level cryptographic tools.

Of course, other libraries already exist for these core operations. NaCl advances the state of the art by improving security, by improving usability, and by improving speed.

## Why .Net Port?

First a portable version on NaCl that can run on every machine already exist and called [libsodium](https://github.com/jedisct1/libsodium), NaCl.net is actually a port of this library.

Also a .net binding (wrapper for the c project) exist for this project https://github.com/adamcaudill/libsodium-net.

So why a .net Port?  From my experience using unmanaged libraries in managed projects is not recommended and for me it always ending bad with an access denied or memory exception. 
This is why I created [NetMQ](httsp://github.com/zeromq/netmq) (.net port of ZeroMQ) and this is why I'm creating NaCl.net. 

But why to port NaCL? [Zeromq 4.0](http://zeromq.org/) introduce new security framework which is using NaCl (actually it is using libsodium) and porting NaCl to .net it's the first stage of porting version 4 of zeromq to .net.

## Where to download?

You can download and compile, it straightforward. Soon it will also be available at nuget.

## How to use?

Soon.

## Contributing?

Any pull requests are welcomed, currently only the feature required by NetMQ are ported, so good starting point is to port the other features. Also pull requests of documentation and examples are highly important.

## License
NaCl.net is using MPLv2, you can read more at the [FAQ](http://www.mozilla.org/MPL/2.0/FAQ.html) file.
