using Terraria;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;

namespace PlayerRenderer.Common.Commands;

public class RenderCommand : ModCommand
{
    public override string Command => "render";
    public override string Usage => "/render <name>";
    public override string Description => "Renders the current player to an NPC Spritesheet. Zoom must be 100% and good lighting is recommended";

    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (Main.GameZoomTarget != 1f || Main.ForcedMinimumZoom != 1f)
        {
            caller.Reply("Game Zoom must be 100% for the sprites to render properly", Color.OrangeRed);
            caller.Reply("If the slider doesn't go down to 100%, disable Forced Zoom in the tModLoader Settings", Color.LightSalmon);
            return;
        }

        if (args.Length == 0)
        {
            caller.Reply("Must provide <name> to the render command", Color.OrangeRed);
            return;
        }

        var name = string.Join(" ", args);

        Directory.CreateDirectory(PlayerRenderer.SavePath);

        var spritesheetPath = Path.Combine(PlayerRenderer.SavePath, $"{name}.png");
        var headPath = Path.Combine(PlayerRenderer.SavePath, $"{name}_Head.png");

        PlayerRenderer.Render(caller.Player, spritesheetPath);
        PlayerRenderer.RenderHead(caller.Player, headPath);

        caller.Reply($"Spritesheet saved to {spritesheetPath}", Color.LightBlue);
        caller.Reply($"Head saved to {headPath}", Color.LightBlue);
        caller.Reply("The Spritesheet should be a drop-in replacement for a Town NPC Sprite, but you should probably crop the Head sprite manually", Color.LightSalmon);
    }
}