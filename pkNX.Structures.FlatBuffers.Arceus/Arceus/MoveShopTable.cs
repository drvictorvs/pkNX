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
public partial class MoveShopTable {
    public MoveShopIndex AddEntry()
    {
        var entry = new MoveShopIndex();

        Table = Table.Append(entry).ToList();
        return entry;
    }
    public void RemoveEntry(int entryIndex)
    {
        var entry = Table[entryIndex];
        Table.Remove(entry);
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class MoveShopIndex { }
