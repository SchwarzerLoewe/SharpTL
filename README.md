# SharpTL

Portable class library allows to serialize/deserialize objects defined in [Type Language](http://core.telegram.org/mtproto/TL) schemas.

Binaries are available via NuGet package:
```powershell
PM> Install-Package SharpTL
```

## Usage ##

Declare a class with a `TLObject` attribute with unique number for an objects schema. Properties that should be serialized are marked with a `TLProperty` attribute with order number.

```csharp
[TLObject(0xA1B2C3D4)]
public class TestObject
{
    [TLProperty(1)]
    public bool TestBoolean { get; set; }

    [TLProperty(2)]
    public double TestDouble { get; set; }

    [TLProperty(3)]
    public int TestInt { get; set; }

    [TLProperty(4)]
    public List<int> TestIntVector { get; set; }

    [TLProperty(5)]
    public long TestLong { get; set; }

    [TLProperty(6)]
    public string TestString { get; set; }

    [TLProperty(7)]
    public List<IUser> TestUsersVector { get; set; }
}
```

The example `IUser` interface must be marked by a `TLType` attribute with types of derived classes which also must be marked by a `TLObject` attribute.

```csharp
[TLType(typeof (User), typeof (NoUser))]
public interface IUser
{
    int Id { get; set; }
}

[TLObject(0xD23C81A3)]
public class User : IUser
{
    [TLProperty(1)]
    public int Id { get; set; }

    [TLProperty(2)]
    public string FirstName { get; set; }

    [TLProperty(3)]
    public string LastName { get; set; }

    [TLProperty(4)]
    public byte[] Key { get; set; }
}

[TLObject(0xC67599D1)]
public class NoUser : IUser
{
    [TLProperty(1)]
    public int Id { get; set; }
}
```

#### Serializing:
```csharp
var obj = new TestObject();
// obj properties initializing.
byte[] objBytes = TLRig.Default.Serialize(obj);
```

#### Deserializing:

```csharp
byte[] objBytes;
// ...
var obj = TLRig.Default.Deserialize<TestObject>(objBytes);
```

## TL-schema compiler
It is possible to automatically convert a TL-schema (json/tl) to C# object model using the **SharpTL.Compiler.CLI** tool.

Usage example:
```powershell
SharpTL.Compiler.CLI.exe compile -t json -s telegram.json -ns SharpTelegram.Schema -mn Telegram -impl
```
This command produces 2 files:
- `SharpTelegram.Schema.cs` schema types and methods interfaces.
- `SharpTelegram.Schema.MethodsImpl.cs` schema methods implementation (generated only with `-impl` arg). In order to compile this file, [SharpMTProto] library must be referenced.

## Performance
There is a special performance measurement tool, named [SerializersRace](https://github.com/inTagger/SerializersRace).

#### Current standings for 9000000 serialization iterations

| Serializer        | Version   | Bytes |  Prep. | Warmup |  Elapsed |  Rate |
|:------------------|:----------|------:|-------:|-------:|---------:|------:|
| [protobuf-net]    | 2.0.0.668 |    60 | 146 ms |  14 ms |  5440 ms | x1,00 |
| [SharpTL]         | 0.7.2     |    88 |  21 ms |  51 ms | 13547 ms | x2,49 |


## Change log

#### SharpTL 0.8

- Added `TLObjectWithCustomSerializerAttribute`.
- Improved compiler:
  - Added support for several new built-in types.
  - Added 'MethodsInterfaceName' arg to the compiler.
  - Compiler template: type interfaces now 'partial's.
- Breaking changes:
  - `TLSerializerBase` now has only one constructor and it accepts `constructorNumber`.
  - `ITLSingleConstructorSerializer` now has settable `ConstructorNumber` property.
  - Removed ability to set custom serializer type in `TLObjectAttribute`, use `TLObjectWithCustomSerializerAttribute` instead and to override constructor number from a custom serializer, use the attribute together with `TLObjectAttribute`.

#### SharpTL 0.7.2

- Improved performance of `TLVectorSerializer`.
- Added `PrepareSerializer<T>()` method to the `TLRig`.
- Change behavior of getting serializer from `TLRig`. Now result can be null instead of throwing an exception.

#### SharpTL 0.7.1

- Significantly increased serialization performance of `TLCustomObjectSerializer` (up to 5x faster than in SharpTL 7.0).
- Added dependency for the 'Dynamitey' package.

#### SharpTL 0.7

- Added support of the `Object` Pseudotype.
- Added generic `TLSerializer<T>`.
- Added `CustomSerializerType` support for `TLObject`.
- TLRig: added serialization/deserialization to/from `TLStreamer`.
- Added «Durov mode» to `TLBytesSerializer`. In «Durov mode» `Bytes` is an alias for `String` type hence both serializers have the same constructor numbers.

#### SharpTL 0.6

- Added support for bare vector serialization in the `TLSchemaCompiler`.
- Added `Read()` method with items serialization mode override for `TLVerctorSerializer`.

#### SharpTL 0.5

- Fixed problem with zero length on `TLStreamer.WriteRandomData(int length)`.
- Changed `TLStreamer` API names conventions (e.g. `ReadUInt` -> `ReadUInt32`).

#### SharpTL 0.4.1

- Removed `CRC32` (now in the `BigMath` lib).
- Minor changes to the `TLStreamer`.

#### SharpTL 0.4

- Improved `TLStreamer`.
- Added support of serialization mode override to some methods of the `TLRig`.
- Renamed `TLStreamer.WriteAllBytes()` to `TLStreamer.Write()`.

#### SharpTL 0.3

- Added a `leaveOpen` (underlying stream) parameter to `TLStreamer`.
- Added `TLBitConverter`.
- Implemented serializers for `Int128` and `Int256`.
- `Int128` and `Int256` base types moved to `BigMath` repo.

#### SharpTL 0.2

- Added base types `Int128`, `Int256`, and `TLBytesSerializer`.
- Added `SharpTL.Compiler`.

#### SharpTL 0.1

- Implemented serializers for base TL types and custom objects.


[protobuf-net]: https://github.com/mgravell/protobuf-net
[SharpTL]: https://github.com/Taggersoft/SharpTL
[SharpMTProto]: https://github.com/Taggersoft/SharpMTProto
