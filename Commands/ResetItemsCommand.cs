using Sprint0.Interfaces;
using Sprint0.Items;

namespace Sprint0.Commands
{
    public class ResetItemsCommand : ICommand
    {
        private readonly ItemDemo items;

        public ResetItemsCommand(ItemDemo items)
        {
            this.items = items;
        }

        public void Execute()
        {
            items.Reset();
        }
    }
}
