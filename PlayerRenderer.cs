using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PlayerRenderer;

public class PlayerRenderer : Mod
{
	public const int WIDTH = 44;
	public const int HEIGHT = 54;
	public const int FRAME_COUNT = 21;

	public const int HEAD_SIZE = 36;

	public static bool RenderingSpritesheet { get; private set; } = false;
	public static int FrameIndex { get; private set; } = 0;

	public static string SavePath { get; } = Path.Combine(Main.SavePath, "Sprites");

	public static readonly Frame[] Frames =
	{
		Pose.Default,
		new Pose(5, 6, 5),
		new Pose(6),
		new Pose(7),
		new Pose(8),
		new Pose(9),
		new Pose(10),
		new Pose(11),
		new Pose(12),
		new Pose(13),
		new Pose(14),
		new Pose(15),
		new Pose(16),
		new Pose(17),
		new Pose(18),
		new Pose(19),
		Pose.Default,
		Pose.Default,
		new Sitting(),
		Pose.Default,
		new Blinking(),
	};

	public static void Render(Player player, string path)
	{
		RenderingSpritesheet = true;

		using var stream = new RenderStream(Main.spriteBatch.GraphicsDevice, WIDTH, HEIGHT * FRAME_COUNT) { OutputPath = path };

		for (FrameIndex = 0; FrameIndex < FRAME_COUNT; FrameIndex++)
		{
			var frame = Frames[FrameIndex];
			frame.Setup(player);
			Main.PlayerRenderer.DrawPlayers(Main.Camera, new[] { player });
			frame.Cleanup(player);
		}

		RenderingSpritesheet = false;
	}

	public static void RenderHead(Player player, string path)
	{
		using var stream = new RenderStream(Main.spriteBatch.GraphicsDevice, HEAD_SIZE, HEAD_SIZE) { OutputPath = path };

		Main.spriteBatch.Begin();
		Main.PlayerRenderer.DrawPlayerHead(Main.Camera, player, new Vector2(stream.Width / 2 - 4, stream.Height / 2 - 4));
		Main.spriteBatch.End();
	}
}

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

public class RenderStream : IDisposable
{
	public string OutputPath { get; init; }

	public RenderTarget2D Target { get; private init; }
	public int Width => Target.Width;
	public int Height => Target.Height;

	public RenderStream(GraphicsDevice device, int width, int height)
	{
		Target = new RenderTarget2D(device, width, height);

		device.SetRenderTarget(Target);
		device.Clear(Color.Transparent);
	}

	public void Dispose()
	{
		using var stream = File.Create(OutputPath);

		Target.SaveAsPng(stream, Width, Height);
		Target.Dispose();

		GC.SuppressFinalize(this);
	}
}

public class RenderCommand : ModCommand
{
	public override string Command => "render";
	public override string Usage => "/render <name>";
	public override string Description => "Renders the current player to an NPC Spritesheet. Zoom must be 100% and good lighting is recommended";

	public override CommandType Type => CommandType.Chat;

	public override void Action(CommandCaller caller, string input, string[] args)
	{
		if (Main.GameZoomTarget != 1f)
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

		var name = args[0];

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