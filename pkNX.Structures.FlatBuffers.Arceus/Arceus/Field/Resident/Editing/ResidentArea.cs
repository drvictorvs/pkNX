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

    internal bool DoWrite { get; set; } = false;

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

    private void TryWrite<T>(string path, T obj) where T : class, IFlatBufferSerializable<T>
    {
        var index = resident.GetIndexFull(path);
        if (index == -1)
            throw new ArgumentOutOfRangeException(nameof(path));

        byte[] result = FlatBufferConverter.SerializeFrom(obj);
        resident[index] = result;
    }

    public void LoadInfo()
    {
        DoWrite = false;
        try{ Encounters   = TryRead<EncounterDataArchive      >(Settings.Encounters); }
        catch{throw new Exception("Encounters"); }
        finally { DoWrite = true; }

        try{ Locations    = TryRead<PlacementLocationArchive  >(Settings.Locations); }
        catch { throw new Exception("Locations"); }
        finally { DoWrite = DoWrite & true; }

        try{ Spawners     = TryRead<PlacementSpawnerArchive   >(Settings.Spawners); }
        catch { throw new Exception("Spawners"); }
        finally { DoWrite = DoWrite & true; }

        try{ Wormholes    = TryRead<PlacementSpawnerArchive   >(Settings.WormholeSpawners); }
        catch { throw new Exception("Wormholes"); }
        finally { DoWrite = DoWrite & true; }

        try{ LandItems    = TryRead<LandmarkItemSpawnTable    >(Settings.LandmarkItemSpawns); }
        catch { throw new Exception("LandItems"); }
        finally { DoWrite = DoWrite & true; }

        try{ LandMarks    = TryRead<LandmarkItemTable         >(Settings.LandmarkItems); }
        catch { throw new Exception("LandMarks"); }
        finally { DoWrite = DoWrite & true; }

        try{ Unown        = TryRead<PlacementUnnnTable        >(Settings.UnownSpawners); }
        catch { throw new Exception("Unown"); }
        finally { DoWrite = DoWrite & true; }

        try{ Mikaruge     = TryRead<PlacementMkrgTable        >(Settings.Mkrg); }
        catch { throw new Exception("Mikaruge"); }
        finally { DoWrite = DoWrite & true; }

        try { SearchItem   = TryRead<PlacementSearchItemTable  >(Settings.SearchItem); }
        catch { throw new Exception("SearchItem"); }
        finally { DoWrite = DoWrite & true; }
    }
    
    public void SaveInfo(string areaName)
    {
        if(!DoWrite || areaName.Contains("test") || areaName.Contains("processing"))
            return;
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
