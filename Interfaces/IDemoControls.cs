namespace TeamUno.Mario.Interfaces
{
    public interface IDemoControls : IItemSelection
    {
        void PreviousBlock();
        void NextBlock();
        void PreviousEnemy();
        void NextEnemy();
        void CycleItem(int direction);
    }
}
