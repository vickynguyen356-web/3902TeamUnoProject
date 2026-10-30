using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class LoadAreaCommand : ICommand
    {
        private readonly IGameActions _gameActions;
        private readonly string _areaId;
        private readonly string _spawnId;

        public LoadAreaCommand(IGameActions gameActions, string areaId, string spawnId = "start")
        {
            _gameActions = gameActions;
            _areaId = areaId;
            _spawnId = spawnId;
        }

        public void Execute()
        {
            _gameActions.LoadArea(_areaId, _spawnId);
        }
    }
}
