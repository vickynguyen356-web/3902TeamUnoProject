using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class ItemSelection : IItemSelection
    {
        private ItemType selectedItem = ItemType.Mushroom;

        public ItemType SelectedItem
        {
            get { return selectedItem; }
        }

        public void Select(ItemType itemType)
        {
            selectedItem = itemType;
        }

        public void Reset()
        {
            Select(ItemType.Mushroom);
        }
    }
}
