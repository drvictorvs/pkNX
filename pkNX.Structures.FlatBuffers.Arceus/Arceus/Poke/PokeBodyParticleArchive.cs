using System;
using System.ComponentModel;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace pkNX.Structures.FlatBuffers.Arceus;

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class PokeBodyParticleArchive {
    public PokeBodyParticle AddEntry()
    {
        var entry = new PokeBodyParticle()
        {
            Field03 = new List<PokeBodyParticle_F03> {
              new(){
                Field02 = "",
                Field03 = "",
                Field04 = "",
                Field05 = "",
              },
            },
        };

        Table = Table.Append(entry).ToList();
        return entry;
    }
 }

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class PokeBodyParticle { }

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class PokeBodyParticle_F03 { }
