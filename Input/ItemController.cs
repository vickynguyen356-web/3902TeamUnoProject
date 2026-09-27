using Microsoft.Xna.Framework.Input;
using Sprint0.Commands;
using Sprint0.Interfaces;
using Sprint0.Items;

namespace Sprint0.Input
{
    public class ItemController : IController
    {
        private readonly ICommand resetItemsCommand;
        private readonly ICommand previousItemCommand;
        private readonly ICommand nextItemCommand;
        private readonly ICommand selectMushroomCommand;
        private readonly ICommand selectFireFlowerCommand;
        private readonly ICommand selectCoinCommand;
        private readonly ICommand selectStarCommand;
        private readonly ICommand selectOneUpCommand;
        private readonly ICommand selectBlockCoinCommand;
        private KeyboardState previousKeyState;
        private KeyboardState currentKeyState;

        public ItemController(ItemDemo items)
        {
            IItemSelection selection = items.Selection;
            resetItemsCommand = new ResetItemsCommand(items);
            previousItemCommand = new CycleItemCommand(items, -1);
            nextItemCommand = new CycleItemCommand(items, 1);
            selectMushroomCommand = new SelectItemCommand(selection, ItemType.Mushroom);
            selectFireFlowerCommand = new SelectItemCommand(selection, ItemType.FireFlower);
            selectCoinCommand = new SelectItemCommand(selection, ItemType.Coin);
            selectStarCommand = new SelectItemCommand(selection, ItemType.Star);
            selectOneUpCommand = new SelectItemCommand(selection, ItemType.OneUpMushroom);
            selectBlockCoinCommand = new SelectItemCommand(selection, ItemType.BlockCoin);
        }

        public void Update()
        {
            HandleInput(Keyboard.GetState());
        }

        public void HandleInput(KeyboardState keyboardState)
        {
            currentKeyState = keyboardState;
            HandleCyclingKeys();
            HandleSelectionKeys();
            HandleCoinKeys();
            previousKeyState = currentKeyState;
        }

        private void HandleCyclingKeys()
        {
            if (WasPressed(Keys.R))
            {
                resetItemsCommand.Execute();
            }

            if (WasPressed(Keys.U))
            {
                previousItemCommand.Execute();
            }

            if (WasPressed(Keys.I))
            {
                nextItemCommand.Execute();
            }
        }

        private void HandleSelectionKeys()
        {
            if (WasPressed(Keys.D1) || WasPressed(Keys.NumPad1))
            {
                selectMushroomCommand.Execute();
            }
            if (WasPressed(Keys.D2) || WasPressed(Keys.NumPad2))
            {
                selectFireFlowerCommand.Execute();
            }
            if (WasPressed(Keys.D4) || WasPressed(Keys.NumPad4))
            {
                selectStarCommand.Execute();
            }
            if (WasPressed(Keys.D5) || WasPressed(Keys.NumPad5))
            {
                selectOneUpCommand.Execute();
            }
        }

        private void HandleCoinKeys()
        {
            if (WasPressed(Keys.D3) || WasPressed(Keys.NumPad3))
            {
                selectCoinCommand.Execute();
            }
            if (WasPressed(Keys.D6) || WasPressed(Keys.NumPad6))
            {
                selectBlockCoinCommand.Execute();
            }
        }

        private bool WasPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && previousKeyState.IsKeyUp(key);
        }
    }
}
