using Sprint0.Interfaces;
using Sprint0.Items;

namespace Sprint0.Commands
{
    public class SelectItemCommand : ICommand
    {
        private readonly IItemSelection selection;
        private readonly ItemType itemType;

        public SelectItemCommand(IItemSelection selection, ItemType itemType)
        {
            this.selection = selection;
            this.itemType = itemType;
        }

        public void Execute()
        {
            selection.Select(itemType);
        }
    }
}
