using System;
using Microsoft.Xna.Framework.Input;
using TeamUno.Mario.Commands;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Items;

namespace TeamUno.Mario.Input
{
    internal class DemoController : IController
    {
        private readonly KeyboardInput _input;
        private readonly ICommand _previousBlockCommand;
        private readonly ICommand _nextBlockCommand;
        private readonly ICommand _previousEnemyCommand;
        private readonly ICommand _nextEnemyCommand;
        private readonly ICommand _previousItemCommand;
        private readonly ICommand _nextItemCommand;
        private readonly ICommand _selectMushroomCommand;
        private readonly ICommand _selectFireFlowerCommand;
        private readonly ICommand _selectFloatingCoinCommand;
        private readonly ICommand _selectStarCommand;
        private readonly ICommand _selectOneUpCommand;
        private readonly ICommand _selectBlockCoinCommand;

        public DemoController(IDemoControls demoControls, KeyboardInput input)
        {
            ArgumentNullException.ThrowIfNull(demoControls);

            ArgumentNullException.ThrowIfNull(input);

            _input = input;
            _previousBlockCommand = new PreviousBlockCommand(demoControls);
            _nextBlockCommand = new NextBlockCommand(demoControls);
            _previousEnemyCommand = new PreviousEnemyCommand(demoControls);
            _nextEnemyCommand = new NextEnemyCommand(demoControls);
            _previousItemCommand = new CycleItemCommand(demoControls, -1);
            _nextItemCommand = new CycleItemCommand(demoControls, 1);
            _selectMushroomCommand = new SelectItemCommand(demoControls, ItemType.Mushroom);
            _selectFireFlowerCommand = new SelectItemCommand(demoControls, ItemType.FireFlower);
            _selectFloatingCoinCommand = new SelectItemCommand(demoControls, ItemType.FloatingCoin);
            _selectStarCommand = new SelectItemCommand(demoControls, ItemType.Star);
            _selectOneUpCommand = new SelectItemCommand(demoControls, ItemType.OneUpMushroom);
            _selectBlockCoinCommand = new SelectItemCommand(demoControls, ItemType.BlockCoin);
        }

        public void Update()
        {
            HandleBlockAndEnemyKeys();
            HandleCyclingKeys();
            HandleSelectionKeys();
            HandleCoinKeys();
        }

        private void HandleBlockAndEnemyKeys()
        {
            if (_input.WasPressed(Keys.T))
            {
                _previousBlockCommand.Execute();
            }

            if (_input.WasPressed(Keys.Y))
            {
                _nextBlockCommand.Execute();
            }

            if (_input.WasPressed(Keys.O))
            {
                _previousEnemyCommand.Execute();
            }

            if (_input.WasPressed(Keys.P))
            {
                _nextEnemyCommand.Execute();
            }
        }

        private void HandleCyclingKeys()
        {
            if (_input.WasPressed(Keys.U))
            {
                _previousItemCommand.Execute();
            }

            if (_input.WasPressed(Keys.I))
            {
                _nextItemCommand.Execute();
            }
        }

        private void HandleSelectionKeys()
        {
            if (_input.WasPressed(Keys.D1) || _input.WasPressed(Keys.NumPad1))
            {
                _selectMushroomCommand.Execute();
            }

            if (_input.WasPressed(Keys.D2) || _input.WasPressed(Keys.NumPad2))
            {
                _selectFireFlowerCommand.Execute();
            }

            if (_input.WasPressed(Keys.D4) || _input.WasPressed(Keys.NumPad4))
            {
                _selectStarCommand.Execute();
            }

            if (_input.WasPressed(Keys.D5) || _input.WasPressed(Keys.NumPad5))
            {
                _selectOneUpCommand.Execute();
            }
        }

        private void HandleCoinKeys()
        {
            if (_input.WasPressed(Keys.D3) || _input.WasPressed(Keys.NumPad3))
            {
                _selectFloatingCoinCommand.Execute();
            }

            if (_input.WasPressed(Keys.D6) || _input.WasPressed(Keys.NumPad6))
            {
                _selectBlockCoinCommand.Execute();
            }
        }
    }
}
