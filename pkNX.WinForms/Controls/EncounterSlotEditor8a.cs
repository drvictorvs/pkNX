// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Windows.Forms;
// using pkNX.Structures.FlatBuffers.Arceus;

// namespace pkNX.WinForms.Controls;

// public partial class EncounterSlotEditor8a : UserControl
// {
//     public IList<EncounterSlot> Tables = Array.Empty<EncounterSlot>();

//     public EncounterSlotEditor8a() => InitializeComponent();

//     public void LoadTable(IList<EncounterSlot> table, string path)
//     {
//         Tables = table;
//         if (table.Count == 0)
//         {
//             Visible = false;
//             return;
//         }

//         Visible = true;
//         L_ConfigName.Text = path;

//         var items = table.Select(z => new ComboItem(z.NameSummary.Replace("\"", ""), z)).ToArray();
//         CB_EncounterSlots.DisplayMember = nameof(ComboItem.Text);
//         CB_EncounterSlots.ValueMember = nameof(ComboItem.Value);
//         CB_EncounterSlots.DataSource = new BindingSource(items, null);

//         CB_EncounterSlots.SelectedIndex = 0;
//     }

//     private class ComboItem
//     {
//         public ComboItem(string text, EncounterSlot value)
//         {
//             Text = text;
//             Value = value;
//         }

//         public string Text { get; }
//         public EncounterSlot Value { get; }
//     }

//     private void CB_EncounterSlots_SelectedIndexChanged(object sender, EventArgs e)
//     {
//         if (CB_EncounterSlots.SelectedValue is not EncounterSlot enc)
//             throw new ArgumentException(nameof(CB_EncounterSlots.SelectedValue));
//         PG_EncounterSlots.SelectedObject = enc;
//     }
// }
