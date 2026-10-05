namespace TeamUno.Mario.Interfaces
{
    internal interface IDemoControls : IItemSelection
    {
        void PreviousBlock();
        void NextBlock();
        void PreviousEnemy();
        void NextEnemy();
        void CycleItem(int direction);
    }
}
