using System.ComponentModel;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace pkNX.Structures.FlatBuffers.Arceus;

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class EvolutionTable {
    

    public EvolutionSet AddEntry()
    {
        var entry = new EvolutionSet();
        Table.Add(entry);
        return entry;
    }

    public void RemoveEntry(int entryIndex)
    {
        if(entryIndex > 0 && entryIndex < Table.Count){
            var entry = Table[entryIndex];
            Table.Remove(entry);
        } else {
            throw new ArgumentOutOfRangeException(nameof(entryIndex));
        }
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class EvolutionEntry {
    public override string ToString() => $"{Method} - {Argument} - {Species}{(Form == 0 ? "" : $"-{Form}")} - {Level}";
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class EvolutionSet
{
    public EvolutionEntry? Entry01
    {
        get { 
            if (Table!.Count != 1)
                throw new ArgumentException($"Invalid {nameof(Table)}"); 
            if (Table[0] is not null)
                return Table[0];
            else
                throw new ArgumentException($"Invalid {nameof(Table)}");
        }
        set { 
            if (Table!.Count != 1) 
                throw new ArgumentException($"Invalid {nameof(Table)}"); 
            if (Table[0] is EvolutionEntry)
                Table[0] = value; 
            }
    }

    public EvolutionEntry? Entry02
    {
        get
        {
            if (Table!.Count != 1)
                throw new ArgumentException($"Invalid {nameof(Table)}");
            if (Table[0] is not null)
                return Table[1];
            else
                throw new ArgumentException($"Invalid {nameof(Table)}");
        }
        set
        {
            if (Table!.Count != 1)
                throw new ArgumentException($"Invalid {nameof(Table)}");
            if (Table[1] is EvolutionEntry)
                Table[1] = value;
        }
    }

    public byte[] Write()
    {
        if (Table is null || Table.Count == 0)
            return Array.Empty<byte>();

        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        foreach (var evo in Table)
        {
            // ReSharper disable RedundantCast
            bw.Write((ushort)GetMainlineMethod(evo.Method));
            bw.Write((ushort)evo.Argument);
            bw.Write((ushort)evo.Species);
            bw.Write((sbyte)evo.Form);
            bw.Write((byte)evo.Level);
            // ReSharper restore RedundantCast
        }
        return ms.ToArray();

        // Remap so all games have the same method values.
        static ushort GetMainlineMethod(ushort evoMethod) => evoMethod switch
        {
            50 => (ushort)EvolutionType.UseItem, // Ursaluna
            51 => (ushort)EvolutionType.UseMoveAgileStyle, // Wyrdeer
            52 => (ushort)EvolutionType.UseMoveStrongStyle, // Overqwil
            53 => (ushort)EvolutionType.LevelUpRecoilDamageMale, // Basculegion-0
            54 => (ushort)EvolutionType.LevelUpRecoilDamageFemale, // Basculegion-1
            _ => evoMethod,
        };
    }
}


[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class EvolutionEntry01 : EvolutionEntry
{

}