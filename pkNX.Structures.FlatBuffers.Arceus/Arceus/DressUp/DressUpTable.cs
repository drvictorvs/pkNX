using System.ComponentModel;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace pkNX.Structures.FlatBuffers.Arceus;

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class DressUpTable {
    public DressUpEntry AddEntry()
    {
        var entry = new DressUpEntry(){
          EntryName = "",
          Field15 = "",
          Field20 = "",
          HairStyleName = "",
          FaceName = "",
          EyeBName = "",
          EyeWName = "",
          HeadwearName = "",
          TopName = "",
          BottomName = "",
          DressName = "",
          FootwearName = "",
          BagName = "",
          UnusedName = "",
          SlotName = "",
          PartIndex0 = "",
          PartIndex1 = "",
          PartIndex2 = "",
          PartIndex3 = "",
          PartIndex4 = "",
          ConfigMotionPath = "",
          DefaultMotionPath = "",
          DataPath = "",
          DefaultPartName = "",
          MotionPath = "",
          CategoryName = "",
          SubCategoryName = "",
        };

        Table = Table.Append(entry).ToList();
        return entry;
    }
 }

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class DressUpEntry { }
