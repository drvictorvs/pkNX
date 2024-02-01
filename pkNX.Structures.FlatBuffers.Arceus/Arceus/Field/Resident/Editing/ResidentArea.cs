using System.Buffers;
using System.Diagnostics;
using FlatSharp;
using pkNX.Containers;

namespace pkNX.Structures.FlatBuffers.Arceus;

// Not a FlatBuffer; wraps the fields into a single object.
public sealed class ResidentArea(GFPack resident, AreaSettings settings, AreaSettingsTable? table = null)
{
    public AreaSettings Settings = settings;
    public AreaSettingsTable? Table = table;
    public string AreaName => Settings.Name;
    public string FriendlyAreaName => Settings.FriendlyAreaName;

    // Encount
    public EncounterDataArchive Encounters { get; private set; } = null!;

    // Placement
    public PlacementLocationArchive Locations { get; private set; } = null!;
    public PlacementSpawnerArchive Spawners { get; private set; } = null!;
    public PlacementSpawnerArchive Wormholes { get; private set; } = null!;
    public LandmarkItemSpawnTable LandItems { get; private set; } = null!;
    public LandmarkItemTable LandMarks { get; private set; } = null!;
    public PlacementUnnnTable Unown { get; private set; } = null!;
    public PlacementMkrgTable Mikaruge { get; private set; } = null!;
    public PlacementSearchItemTable SearchItem { get; private set; } = null!;

    private T TryRead<T>(string path) where T : class, IFlatBufferSerializable<T>
    {
        var index = resident.GetIndexFull(path);
        if (index == -1)
            throw new ArgumentOutOfRangeException(nameof(path));

        var data = resident[index];
        return T.GreedyMutableSerializer.Parse(data);
    }

    private static byte[] Write<T>(T obj) where T : class, IFlatBufferSerializable<T>
    {
        var pool = ArrayPool<byte>.Shared;
        var serializer = obj.Serializer;
        var data = pool.Rent(serializer.GetMaxSize(obj));
        var len = serializer.Write(data, obj);
        var result = data.AsSpan(0, len).ToArray();
        pool.Return(data);
        return result;
    }

    private void TryWrite<T>(string path, T obj) where T : class, IFlatBufferSerializable<T>
    {
        var index = resident.GetIndexFull(path);
        if (index == -1)
            return;

        byte[] result = Write(obj);
        resident[index] = result;
    }

    public void LoadInfo()
    {
        // Load encount 
        try {
        Encounters   = TryRead<EncounterDataArchive      >(Settings.Encounters);
        Locations    = TryRead<PlacementLocationArchive  >(Settings.Locations);
        Spawners     = TryRead<PlacementSpawnerArchive   >(Settings.Spawners);
        Wormholes    = TryRead<PlacementSpawnerArchive   >(Settings.WormholeSpawners);
        LandItems    = TryRead<LandmarkItemSpawnTable    >(Settings.LandmarkItemSpawns);
        LandMarks    = TryRead<LandmarkItemTable         >(Settings.LandmarkItems);
        Unown        = TryRead<PlacementUnnnTable        >(Settings.UnownSpawners);
        Mikaruge     = TryRead<PlacementMkrgTable        >(Settings.Mkrg);
        SearchItem   = TryRead<PlacementSearchItemTable  >(Settings.SearchItem);
        } catch {
            return;
        }
    }
    
    public void SaveInfo()
    {
        Debug.WriteLine($"Writing {Encounters} to {Settings.Encounters}");
        TryWrite(Settings.Encounters, Encounters);
        Debug.WriteLine($"Writing {Locations} to {Settings.Locations}");
        TryWrite(Settings.Locations, Locations);
        Debug.WriteLine($"Writing {Spawners} to {Settings.Spawners}");
        TryWrite(Settings.Spawners, Spawners);
        Debug.WriteLine($"Writing {Wormholes} to {Settings.WormholeSpawners}");
        TryWrite(Settings.WormholeSpawners, Wormholes);
        Debug.WriteLine($"Writing {LandItems} to {Settings.LandmarkItemSpawns}");
        TryWrite(Settings.LandmarkItemSpawns, LandItems);
        Debug.WriteLine($"Writing {LandMarks} to {Settings.LandmarkItems}");
        TryWrite(Settings.LandmarkItems, LandMarks);
        Debug.WriteLine($"Writing {Unown} to {Settings.UnownSpawners}");
        TryWrite(Settings.UnownSpawners, Unown);
        Debug.WriteLine($"Writing {Mikaruge} to {Settings.Mkrg}");
        TryWrite(Settings.Mkrg, Mikaruge);
        Debug.WriteLine($"Writing {SearchItem} to {Settings.SearchItem}");
        TryWrite(Settings.SearchItem, SearchItem);
        if(Table != null){
            Debug.WriteLine($"Writing {Table} to {Settings.SearchItem}");
            TryWrite("bin/field/resident/AreaSettings.bin", Table);
        }
    }
}
