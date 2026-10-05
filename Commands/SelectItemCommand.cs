using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Items;

namespace TeamUno.Mario.Commands
{
    internal class SelectItemCommand : ICommand
    {
        private readonly IItemSelection _selection;
        private readonly ItemType _itemType;

        public SelectItemCommand(IItemSelection selection, ItemType itemType)
        {
            _selection = selection;
            _itemType = itemType;
        }

        public void Execute()
        {
            _selection.Select(_itemType);
        }
    }
}
