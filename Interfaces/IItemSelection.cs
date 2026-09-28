using TeamUno.Mario.Items;

namespace TeamUno.Mario.Interfaces
{
    public interface IItemSelection
    {
        ItemType SelectedItem
        {
            get;
        }

        void Select(ItemType itemType);
    }
}
