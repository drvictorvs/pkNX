using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using pkNX.Containers;
using pkNX.Game;
using pkNX.Randomization;
using pkNX.Structures;
using pkNX.Structures.FlatBuffers;
using pkNX.Structures.FlatBuffers.Arceus;
using static pkNX.Structures.Species;
using Util = pkNX.Randomization.Util;

namespace pkNX.WinForms.Subforms;

public partial class AreaEditor8a : Form
{
    private readonly GameManagerPLA ROM;
    private readonly GFPack Resident;
    private readonly AreaSettingsTable Settings;

    private readonly string[] AreaNames;

    private ResidentArea Area;
    private int AreaIndex;
    private readonly bool Loading;
    private static string[] BadAreas => ["test", "processing"];

    public AreaEditor8a(GameManagerPLA rom)
    {
        ROM = rom;

        Resident = (GFPack)ROM.GetFile(GameFile.Resident);
        var bin_settings = Resident.GetDataFullPath("bin/field/resident/AreaSettings.bin");
        Settings = FlatBufferConverter.DeserializeFrom<AreaSettingsTable>(bin_settings);

        AreaNames = Settings.Table.Select(z => z.Name).ToArray();

        const string startingArea = "ha_area00";
        (AreaIndex, Area) = LoadAreaByName(startingArea);

        InitializeComponent();

        PG_RandSettings.SelectedObject = EditUtil.Settings.Species;

        Loading = true;
        CB_Area.Items.AddRange(Settings.Table.Select(z => z.FriendlyAreaName).ToArray());
        CB_Area.SelectedIndex = AreaIndex;
        LoadArea();
        Loading = false;
    }

    private (int index, ResidentArea area) LoadAreaByName(string name)
    {
        var index = Array.IndexOf(AreaNames, name);
        var area = new ResidentArea(Resident, Settings.Find(name), Settings);
        area.LoadInfo();
        return (index, area);
    }

    private void CB_Map_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Loading)
            return;
        SaveArea();
        (AreaIndex, Area) = LoadAreaByName(AreaNames[CB_Area.SelectedIndex]);
        LoadArea();
    }

    private void B_Randomize_Click(object sender, EventArgs e)
    {
        SaveArea();
        RandomizeArea(Area, (SpeciesSettings)PG_RandSettings.SelectedObject);
        LoadArea();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_AllowRiding_Click(object sender, EventArgs e)
    {
        foreach (string AreaName in AreaNames.Where(z => !BadAreas.Any(z.Contains)).ToArray())
        {
            SaveArea();
            (AreaIndex, Area) = LoadAreaByName(AreaName);
            LoadArea();
            Area.Settings.CanRide = true;
        }
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_ChangeProperty_Click(object sender, EventArgs e)
    {
        Form Dialog = new()
        {
            Text = "Enter the property name you want to change:",
            Width = 400,
            Height = 200,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen
        };
        Label L_PropName = new()
        {
            Text = "Property name:",
            Left = 10,
            Top = 10,
            Width = 100
        };
        Label L_PropValue = new()
        {
            Text = "Property value:",
            Left = 10,
            Top = 50,
            Width = 100
        };
        TextBox TB_PropName = new()
        {
            Left = 120,
            Top = 10,
            Width = 250
        };
        TextBox TB_PropValue = new()
        {
            Left = 120,
            Top = 50,
            Width = 250
        };
        Button ButtonOk = new()
        {
            Text = "OK",
            Left = 200,
            Top = 100,
            Width = 80,
            Height = 30,
            DialogResult = DialogResult.OK
        };
        Button ButtonCancel = new()
        {
            Text = "Cancel",
            Left = 290,
            Top = 100,
            Width = 80,
            Height = 30,
            DialogResult = DialogResult.Cancel
        };
        Dialog.Controls.Add(L_PropName);
        Dialog.Controls.Add(TB_PropName);
        Dialog.Controls.Add(L_PropValue);
        Dialog.Controls.Add(TB_PropValue);
        Dialog.Controls.Add(ButtonOk);
        Dialog.Controls.Add(ButtonCancel);
        Dialog.AcceptButton = ButtonOk;
        bool failure = true;
        while (failure) {
            var result = Dialog.ShowDialog();
            if(result == DialogResult.Cancel)
                break;
            string propName = TB_PropName.Text;
            string propValue = TB_PropValue.Text;

            PropertyInfo? propInfo = typeof(AreaSettings).GetProperty(propName);
            var propType = propInfo!.PropertyType;
            object? newValue;
            try { newValue = Convert.ChangeType(propValue, propType); } catch { WinFormsUtil.Alert($"{propValue} is not a valid value for {propName}."); continue; }
                if (propInfo != null && newValue != null)
                {
                    foreach (string AreaName in AreaNames.Where(z => !BadAreas.Any(z.Contains)).ToArray()){
                        SaveArea();
                        (AreaIndex, Area) = LoadAreaByName(AreaName);
                        LoadArea();
                        propInfo.SetValue(Area.Settings, newValue);
                    }
                    failure = false;
                } else {
                    Dialog.Text = $"Property '{propName}' non-existent or the value input is not valid for its type. Try again or cancel the operation.";
                }
        }
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void RandomizeArea(ResidentArea area, SpeciesSettings settings)
    {
        var pt = ROM.Data.PersonalData;
        var rand = new SpeciesRandomizer(ROM.Info, pt);

        var hasForm = new HashSet<int>();
        var banned = new HashSet<int>();
        foreach (var pi in pt.Table.Cast<IPersonalMisc_SWSH>())
        {
            if (pi.IsPresentInGame)
            {
                banned.Remove(pi.DexIndexNational);
                hasForm.Add(pi.DexIndexNational);
            }
            else if (!hasForm.Contains(pi.DexIndexNational))
            {
                banned.Add(pi.DexIndexNational);
            }
        }

        settings.Legends = false; // Legendary encounter slot conditions require you to not have captured the Legendary in order to encounter them; ban altogether.
        rand.Initialize(settings, [.. banned]);

        var formRand = pt.Table.Cast<IPersonalMisc_SWSH>()
            .Where(z => z.IsPresentInGame && !(Legal.BattleExclusiveForms.Contains(z.DexIndexNational) || Legal.BattleFusions.Contains(z.DexIndexNational)))
            .GroupBy(z => z.DexIndexNational)
            .ToDictionary(z => z.Key, z => z.ToList());

        var encounters = area.Encounters;
        foreach (var table in encounters.Table)
        {
            foreach (var enc in table.Table)
            {
                if (enc.ShinyLock is not ShinyType.Random)
                    continue;

                // to progress the story in Cobalt Coastlands, you are required to show Iscan a Dusclops; ensure one can be captured
                if (enc.Species is (int)Dusclops && table.TableID is 7663383561364099763)
                    continue;

                var spec = rand.GetRandomSpecies(enc.Species);
                enc.Species = spec;
                enc.Form = GetRandomForm(spec);
                enc.ClearMoves();
            }
        }
        int GetRandomForm(int spec)
        {
            if (!formRand.TryGetValue((ushort)spec, out var entries))
                return 0;
            var count = entries.Count;

            return (Species)spec switch
            {
                Growlithe or Arcanine or Voltorb or Electrode or Typhlosion or Qwilfish or Samurott or Lilligant or Zorua or Zoroark or Braviary or Sliggoo or Goodra or Avalugg or Decidueye => 1,
                Basculin => 2,
                Kleavor => 0,
                _ => Util.Random.Next(0, count),
            };
        }
    }

    private void LoadArea()
    {
        Debug.WriteLine($"Loading Area {Area.AreaName} [#{AreaIndex}]");
        if (Area.Settings is not null)
            PG_AreaSettings.SelectedObject = Area.Settings;
        if (Area.Encounters is null) {
            Edit_EncounterSettings.Visible = false;
            Edit_EncounterSlots.Visible = false;
        }
        else {
            Edit_EncounterSettings.LoadTable(Area.Encounters.Table, Area.Settings.Encounters);
            Edit_EncounterSlots.LoadTable(Area.Encounters.Table, Area.Settings.Encounters, ROM);
        }

        if (Area.Spawners is null)
            Edit_RegularSpawners.Visible = false;
        else
            Edit_RegularSpawners.LoadTable(Area.Spawners.Table, Area.Settings.Spawners);

        if (Area.Wormholes is null)
            Edit_WormholeSpawners.Visible = false;
        else
            Edit_WormholeSpawners.LoadTable(Area.Wormholes.Table, Area.Settings.WormholeSpawners);
            
        if (Area.LandItems is null)
            Edit_LandmarkSpawns.Visible = false;
        else
            Edit_LandmarkSpawns.LoadTable(Area.LandItems.Table, Area.Settings.LandmarkItemSpawns);
    }

    private void SaveArea()
    {
        if(!BadAreas.Any(Area.AreaName.Contains)){
            Debug.WriteLine($"Saving Area {Area.AreaName}");
            Area.SaveInfo(Area.AreaName);}
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Save = true;
        Close();
    }

    private bool Save;

    private void AreaEditor8a_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (Save){
            SaveArea();
        }
        else {
            Resident.CancelEdits();
        }
    }
}
