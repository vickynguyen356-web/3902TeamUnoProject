using Sprint0.Items;

namespace Sprint0.Interfaces
{
    public interface IItemSelection
    {
        ItemType SelectedItem { get; }
        void Select(ItemType itemType);
    }
}
