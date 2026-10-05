using TeamUno.Mario.Items;

namespace TeamUno.Mario.Interfaces
{
    internal interface IItemSelection
    {
        ItemType SelectedItem
        {
            get;
        }

        void Select(ItemType itemType);
    }
}
