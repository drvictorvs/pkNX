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
public partial class HaShopTable {
    public HaShopItem AddEntry()
    {
        var entry = new HaShopItem();

        Table = Table.Append(entry).ToList();
        return entry;
    }  
 }

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class HaShopItem { }
