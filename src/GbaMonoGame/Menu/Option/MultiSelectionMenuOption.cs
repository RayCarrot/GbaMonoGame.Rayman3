using System;
using System.Linq;

namespace GbaMonoGame;

public class MultiSelectionMenuOption<T> : MenuOption
{
    public MultiSelectionMenuOption(string name, Item[] items, Func<Item[], T> getData, Action<T> setData, Func<T, string> getCustomName)
        : base(name)
    {
        Items = items;
        GetData = getData;
        SetData = setData;
        GetCustomName = getCustomName;
    }

    private Func<Item[], T> GetData { get; }
    private Action<T> SetData { get; }
    private Func<T, string> GetCustomName { get; }

    private string[] DisplayNames { get; set; }
    private T CustomData { get; set; }
    private bool HasCustom { get; set; }

    private Item[] Items { get; }

    public int OriginalSelectedIndex { get; set; }
    public int SelectedIndex { get; set; }

    public bool IsModified => SelectedIndex != OriginalSelectedIndex;

    public T GetSelectedData()
    {
        if (HasCustom)
        {
            if (SelectedIndex == 0)
                return CustomData;
            else
                return Items[SelectedIndex - 1].Data;
        }
        else
        {
            return Items[SelectedIndex].Data;
        }
    }

    public override void Init()
    {
        T selectedData = GetData(Items);
        int index = Array.FindIndex(Items, x => (x.Data == null && selectedData == null) || x.Data?.Equals(selectedData) == true);

        if (index == -1)
        {
            CustomData = selectedData;
            SelectedIndex = 0;
            HasCustom = true;
            string name = GetCustomName(selectedData);
            DisplayNames = Items.Select(x => x.DisplayName).Prepend(name == null ? "Custom" : $"Custom ({name})").ToArray();
        }
        else
        {
            SelectedIndex = index;
            HasCustom = false;
            DisplayNames = Items.Select(x => x.DisplayName).ToArray();
        }

        OriginalSelectedIndex = SelectedIndex;
    }

    public override void Apply()
    {
        SetData(GetSelectedData());
        Engine.SaveConfig();
        OriginalSelectedIndex = SelectedIndex;
    }

    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1, 0.9f);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Left);

        menu.Text(Name);
        SelectedIndex = menu.Selection(DisplayNames, SelectedIndex);

        if (IsModified)
            Apply();
    }

    public class Item(string displayName, T data)
    {
        public string DisplayName { get; } = displayName;
        public T Data { get; } = data;
    }
}