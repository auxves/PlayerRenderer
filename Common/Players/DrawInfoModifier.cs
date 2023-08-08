using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using PlayerRenderer.Common;

namespace PlayerRenderer.Common.Players;

public class DrawInfoModifier : ModPlayer
{
    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (!PlayerRenderer.RenderingSpritesheet) return;

        var frame = PlayerRenderer.Frames[PlayerRenderer.FrameIndex];

        drawInfo.Position = Main.screenPosition + new Vector2(PlayerRenderer.WIDTH / 4, 8 + PlayerRenderer.FrameIndex * PlayerRenderer.HEIGHT);
        drawInfo.isSitting = frame is Sitting;
    }
}
