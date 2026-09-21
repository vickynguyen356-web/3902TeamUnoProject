using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprint0.Interfaces
{
    public interface ISpriteState
    {
        Rectangle Bounds { get; }
        bool IsSmall { get; }
        PlayerForm Form { get; }
        EntityAnimationState AnimationState { get; }
        SpriteEffects FacingDirection { get; }
    }
}
