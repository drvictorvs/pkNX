using System.Buffers;
using System.Diagnostics;
using FlatSharp;
using pkNX.Containers;

namespace pkNX.Structures.FlatBuffers.Arceus;

// Not a FlatBuffer; wraps the fields into a single object.
public sealed class ResidentArea(GFPack resident, AreaSettings settings, AreaSettingsTable? table = null)
{
    public AreaSettings Settings = settings;
    public AreaSettingsTable Table => table = null!;
    public string AreaName => Settings.Name;
    public string FriendlyAreaName => Settings.FriendlyAreaName;

    // Encount
    public EncounterDataArchive? Encounters { get; private set; } = null!;

    // Placement
    public PlacementLocationArchive? Locations { get; private set; } = null!;
    public PlacementSpawnerArchive? Spawners { get; private set; } = null!;
    public PlacementSpawnerArchive? Wormholes { get; private set; } = null!;
    public LandmarkItemSpawnTable? LandItems { get; private set; } = null!;
    public LandmarkItemTable? LandMarks { get; private set; } = null!;
    public PlacementUnnnTable? Unown { get; private set; } = null!;
    public PlacementMkrgTable? Mikaruge { get; private set; } = null!;
    public PlacementSearchItemTable? SearchItem { get; private set; } = null!;

    private T? TryRead<T>(string path) where T : class, IFlatBufferSerializable<T>
    {
        var index = resident.GetIndexFull(path);
        if (index == -1)
            return null;

        var data = resident[index];
        return T.GreedyMutableSerializer.Parse(data);
    }

private void TryWrite<T>(string path, T? obj) where T : class, IFlatBufferSerializable<T>
    {
        if(obj is null)
            return;
        var index = resident.GetIndexFull(path);
        if (index == -1)
            return;

        byte[] result = FlatBufferConverter.SerializeFrom(obj);
        resident[index] = result;
    }

    public void LoadInfo()
    {
        Encounters   = TryRead<EncounterDataArchive      >(Settings.Encounters); 
        Locations    = TryRead<PlacementLocationArchive  >(Settings.Locations); 
        Spawners     = TryRead<PlacementSpawnerArchive   >(Settings.Spawners); 
        Wormholes    = TryRead<PlacementSpawnerArchive   >(Settings.WormholeSpawners); 
        LandItems    = TryRead<LandmarkItemSpawnTable    >(Settings.LandmarkItemSpawns); 
        LandMarks    = TryRead<LandmarkItemTable         >(Settings.LandmarkItems); 
        Unown        = TryRead<PlacementUnnnTable        >(Settings.UnownSpawners); 
        Mikaruge     = TryRead<PlacementMkrgTable        >(Settings.Mkrg); 
        SearchItem   = TryRead<PlacementSearchItemTable  >(Settings.SearchItem);
    }
    
    public void SaveInfo(string areaName)
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
        Debug.WriteLine($"Writing {Table} to bin/field/resident/AreaSettings.bin");
        TryWrite("bin/field/resident/AreaSettings.bin", Table);
    }
}
