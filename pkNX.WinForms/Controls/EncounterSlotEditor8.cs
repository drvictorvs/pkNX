using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using pkNX.Game;
using pkNX.Structures.FlatBuffers.Arceus;

namespace pkNX.WinForms.Controls;

public partial class EncounterSlotEditor8a : UserControl
{
    public IList<EncounterSlot> Tables = Array.Empty<EncounterSlot>();

    public EncounterSlotEditor8a() => InitializeComponent();

    public void LoadTable(IList<EncounterTable> table, string path)
    {
        if (table.Count == 0)
        {
            Visible = false;
            return;
        }

        Visible = true;
        L_ConfigName.Text = path;

        Tables = table.SelectMany(table => table.Table.Where(slot => slot != null)).ToArray();
        var items = Tables.Select(slot => new ComboItem<EncounterSlot>($"{slot.Species} ({slot.Form})", slot)).ToArray();
        CB_EncounterSlots.DisplayMember = nameof(ComboItem<EncounterSlot>.Text);
        CB_EncounterSlots.ValueMember = nameof(ComboItem<EncounterSlot>.Value);
        CB_EncounterSlots.DataSource = new BindingSource(items, null);

        CB_EncounterSlots.SelectedIndex = 0;
    }

    private class ComboItem<T>(string text, T value)
    {
        public string Text {get; } = text;
        public T Value { get; } = value;
    }

    private void CB_EncounterSlots_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (CB_EncounterSlots.SelectedValue is not EncounterSlot enc)
            throw new ArgumentException(nameof(CB_EncounterSlots.SelectedValue));
        PG_EncounterSlots.SelectedObject = enc;
    }
}
