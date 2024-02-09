using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using pkNX.Game;
using pkNX.Structures;

namespace pkNX.WinForms;

public sealed partial class GenericEditor<T> : Form where T : class
{
    private string[] Names = [];
    private DataCache<T> Cache;
    public bool Modified { get; set; }
    private string[] GetNames(Func<T, int, string> nameSelector)
    {
        string[] Names = Cache.LoadAll().Select(nameSelector).ToArray();
        for (int i = 0; i < Names.Length; i++)
            Names[i] = $"{Names[i]} [{i}]";
        return Names;
    }

    [Obsolete("Use the constructor overload with callbacks instead")]
    public GenericEditor(DataCache<T> Cache, string[] names, string title, Action? randomizeCallback = null, Action? addEntryCallback = null, Action<int>? removeEntryCallback = null, bool canSave = true)
        : this(_ => Cache, (_, i) => names[i], title, _ => randomizeCallback?.Invoke(), addEntryCallback, removeEntryCallback, canSave)
    { }

    public GenericEditor(Func<GenericEditor<T>, DataCache<T>> loadCache, Func<T, int, string> nameSelector, string title, Action<IEnumerable<T>>? randomizeCallback = null, Action? addEntryCallback = null, Action<int>? removeEntryCallback = null, bool canSave = true)
    {
        InitializeComponent();
        Text = title;

        Cache = loadCache(this);
        Names = GetNames(nameSelector);
        CB_EntryName.Items.AddRange(Names);
        CB_EntryName.SelectedIndex = 0;

        if (!canSave)
            B_Save.Enabled = false;

        if (randomizeCallback != null)
        {
            B_Rand.Visible = true;
            B_Rand.Click += (_, __) =>
            {
                randomizeCallback(Cache.LoadAll());
                LoadIndex(0);
                System.Media.SystemSounds.Asterisk.Play();
            };
        }

        if (addEntryCallback != null)
        {
            B_AddEntry.Visible = true;
            B_AddEntry.Click += (_, __) =>
            {
                addEntryCallback();
                Modified = true;

                // Reload editor
                Cache = loadCache(this);
                Names = GetNames(nameSelector);
                
                CB_EntryName.Items.Clear();
                CB_EntryName.Items.AddRange(Names);

                System.Media.SystemSounds.Asterisk.Play();
            };
        }
        if (removeEntryCallback != null)
        {
            B_RemoveEntry.Visible = true;
            B_RemoveEntry.Click += (_, __) =>
            {
                DialogResult confirmResult = MessageBox.Show($"Are you sure you want to remove entry {CB_EntryName.SelectedIndex}?", "Confirm Delete", MessageBoxButtons.YesNo);

                if (confirmResult == DialogResult.Yes)
                {
                    removeEntryCallback(CB_EntryName.SelectedIndex);
                    Modified = true;

                    Cache = loadCache(this);
                    Names = GetNames(nameSelector);

                    CB_EntryName.Items.Clear();
                    CB_EntryName.Items.AddRange(Names);

                    System.Media.SystemSounds.Asterisk.Play();
                }
            };
        }
    }

    private void CB_EntryName_SelectedIndexChanged(object sender, EventArgs e)
    {
        var index = CB_EntryName.SelectedIndex;
        LoadIndex(index);
    }

    private void LoadIndex(int index) => Grid.SelectedObject = Cache[index];

    private void B_Save_Click(object sender, EventArgs e)
    {
        LoadIndex(0);
        Modified = true;
        Close();
    }

    private void B_Dump_Click(object sender, EventArgs e)
    {
        var arr = Cache.LoadAll();
        var result = TableUtil.GetNamedTypeTable(arr, Names, Text.Split(' ')[0]);
        Clipboard.SetText(result);
        System.Media.SystemSounds.Asterisk.Play();
        MessageBox.Show($"Dumped {Names.Length} entries to clipboard.");
    }
}
