using Sprint0.Interfaces;
using Sprint0.Items;

namespace Sprint0.Commands
{
    public class CycleItemCommand : ICommand
    {
        private readonly ItemDemo items;
        private readonly int direction;

        public CycleItemCommand(ItemDemo items, int direction)
        {
            this.items = items;
            this.direction = direction;
        }

        public void Execute()
        {
            items.Cycle(direction);
        }
    }
}
