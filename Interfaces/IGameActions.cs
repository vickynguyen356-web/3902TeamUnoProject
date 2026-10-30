namespace TeamUno.Mario.Interfaces
{
    internal interface IGameActions
    {
        bool CanControlPlayer { get; }
        void TogglePause();
        void Quit();
        void Reset();
        void LoadArea(string areaId, string spawnId = "start");
    }
}
