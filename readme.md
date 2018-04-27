### Brook-Dotnet

Reads and writes octet- and bitstreams.

For a Go version, check out [Brook-Go](https://github.com/Piot/brook-go).

It uses [log-dotnet](https://github.com/Piot/log-dotnet) for logging.

##### Usage

```csharp
octetWriter = new OctetWriter(128);
var bitStream = new OutBitStream(octetWriter);
bitStream.WriteBits(0xf, 4);
bitStream.Close();
```




