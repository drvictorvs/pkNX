using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using pkNX.Structures.FlatBuffers.Arceus;

namespace pkNX.WinForms.Controls;

public partial class EncounterTableEditor8a : UserControl
{
    public IList<EncounterTable> Tables = Array.Empty<EncounterTable>();

    public EncounterTableEditor8a() => InitializeComponent();
    private static string[] BadAreas => ["test", "processing"];

    public void LoadTable(IList<EncounterTable> table, string path)
    {
        Tables = table;
        if (table.Count == 0)
        {
            Visible = false;
            return;
        }

        Visible = true;
        L_ConfigName.Text = path;

        var items = table.Select(z => new ComboItem<EncounterTable>(z.ToString().Replace("\"", ""), z)).ToArray();
        CB_Encounters.DisplayMember = nameof(ComboItem<EncounterTable>.Text);
        CB_Encounters.ValueMember = nameof(ComboItem<EncounterTable>.Value);
        CB_Encounters.DataSource = new BindingSource(items, null);

        CB_Encounters.SelectedIndex = 0;
    }

    private class ComboItem<T>(string text, T value)
    {
        public string Text { get; } = text;
        public T Value { get; } = value;
    }

    private void CB_Encounters_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (CB_Encounters.SelectedValue is not EncounterTable enc)
            throw new ArgumentException(nameof(CB_Encounters.SelectedValue));
        PG_Encounters.SelectedObject = enc;
    }

    private void B_NoShinyLocks_Click(object sender, EventArgs e)
    {
        int ctr = 0;
        foreach (var table in Tables)
        {
            foreach (var slot in table.Table)
            {
                if (slot.ShinyLock != ShinyType.Never) {
                    ctr++;
                    continue;
                }
                slot.ShinyLock = ShinyType.Random;
                ctr++;
            }
        }

        WinFormsUtil.Alert($"Removed {ctr} locked slots.");
        CB_Encounters_SelectedIndexChanged(sender, e);
    }

    private void PG_Encounters_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
    {
        if (e.NewSelection == null)
            return;
        var obj = e.NewSelection.Value;
        bool enable = obj is EncounterSlot;
        B_CloneTableEntry.Enabled = enable;
        B_ConfigureAsAlpha.Enabled = enable;
        B_RemoveCondition.Enabled = enable;
    }

    private void B_CloneTableEntry_Click(object sender, EventArgs e)
    {
        var obj = PG_Encounters.SelectedGridItem.Value;
        if (obj is not EncounterSlot slotToClone)
            return;
        var encounterTable = (EncounterTable)PG_Encounters.SelectedObject;
        encounterTable.Table = encounterTable.Table.Concat(new[] { new EncounterSlot(slotToClone) }).ToArray();
        PG_Encounters.Refresh();
    }

    private void B_ConfigureAsAlpha_Click(object sender, EventArgs e)
    {
        var obj = PG_Encounters.SelectedGridItem.Value;
        if (obj is EncounterSlot slotToEdit)
        {
            slotToEdit.BaseProbability = 1;
            slotToEdit.Field09 = true;
            slotToEdit.Field10 = true;
            slotToEdit.Oybn.Field02 = true;
            slotToEdit.Oybn.Field03 = true;
            slotToEdit.Oybn.Oybn1 = true;
            slotToEdit.NumPerfectIvs = 3;

            PG_Encounters.Refresh();
        }
    }

    private void B_RemoveCondition_Click(object sender, EventArgs e)
    {
        var obj = PG_Encounters.SelectedGridItem.Value;
        if (obj is EncounterSlot slotToEdit)
        {
            slotToEdit.Eligibility.ConditionID = Condition.None;
            slotToEdit.Eligibility.ConditionTypeID = ConditionType.None;

            PG_Encounters.Refresh();
        }
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_IncreaseLevels_Click(object sender, EventArgs e)
    {
            foreach(EncounterTable table in Tables)
            {
                table.MinLevel = (int)(table.MinLevel * 1.1);
                table.MaxLevel = (int)(table.MaxLevel * 1.1);
            }
        System.Media.SystemSounds.Asterisk.Play();
    }
}
