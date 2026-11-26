using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal static class LevelItemManager
    {
        public static void Update(
            GameTime gameTime,
            LevelDefinition definition,
            IList<IItem> items)
        {
            for (int index = items.Count - 1; index >= 0; index--)
            {
                IItem item = items[index];
                if (!item.IsExpired)
                {
                    item.Update(gameTime);
                }

                if (LevelBounds.IsOutsideLevel(item.Bounds, definition))
                {
                    item.Expire();
                }

                if (item.IsExpired)
                {
                    items.RemoveAt(index);
                }
            }
        }
    }
}
