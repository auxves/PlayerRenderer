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
	const int WIDTH = 44;
	const int HEIGHT = 54;
	const int FRAME_COUNT = 21;

	public static bool RenderingSpritesheet { get; private set; } = false;
	public static int FrameIndex { get; private set; } = 0;

	public static string SavePath { get; } = Path.Combine(Main.SavePath, "Sprites");

	public static Frame[] Frames { get; } =
	{
		PoseFrame.Default,
		new PoseFrame { HeadFrame = 5, BodyFrame = 6, LegFrame = 5 },
		new PoseFrame { HeadFrame = 6, BodyFrame = 6, LegFrame = 6 },
		new PoseFrame { HeadFrame = 7, BodyFrame = 7, LegFrame = 7 },
		new PoseFrame { HeadFrame = 8, BodyFrame = 8, LegFrame = 8 },
		new PoseFrame { HeadFrame = 9, BodyFrame = 9, LegFrame = 9 },
		new PoseFrame { HeadFrame = 10, BodyFrame = 10, LegFrame = 10 },
		new PoseFrame { HeadFrame = 11, BodyFrame = 11, LegFrame = 11 },
		new PoseFrame { HeadFrame = 12, BodyFrame = 12, LegFrame = 12 },
		new PoseFrame { HeadFrame = 13, BodyFrame = 13, LegFrame = 13 },
		new PoseFrame { HeadFrame = 14, BodyFrame = 14, LegFrame = 14 },
		new PoseFrame { HeadFrame = 15, BodyFrame = 15, LegFrame = 15 },
		new PoseFrame { HeadFrame = 16, BodyFrame = 16, LegFrame = 16 },
		new PoseFrame { HeadFrame = 17, BodyFrame = 17, LegFrame = 17 },
		new PoseFrame { HeadFrame = 18, BodyFrame = 18, LegFrame = 18 },
		new PoseFrame { HeadFrame = 19, BodyFrame = 19, LegFrame = 19 },
		PoseFrame.Default,
		PoseFrame.Default,
		new SittingFrame(),
		PoseFrame.Default,
		PoseFrame.Default,
	};

	public static void Render(Player player, string name)
	{
		RenderingSpritesheet = true;

		using var stream = new RenderStream(Main.spriteBatch.GraphicsDevice, WIDTH, HEIGHT * FRAME_COUNT) { OutputName = $"{name}.png" };

		for (FrameIndex = 0; FrameIndex < FRAME_COUNT; FrameIndex++)
		{
			Frames[FrameIndex].Setup(player);
			Main.PlayerRenderer.DrawPlayers(Main.Camera, new[] { player });
		}

		RenderingSpritesheet = false;
	}

	public static void RenderHead(Player player, string name)
	{
		using var stream = new RenderStream(Main.spriteBatch.GraphicsDevice, 36, 36) { OutputName = $"{name}_Head.png" };

		Main.spriteBatch.Begin(
			(SpriteSortMode)1,
			BlendState.AlphaBlend,
			Main.Camera.Sampler,
			DepthStencilState.None,
			Main.Camera.Rasterizer,
			null,
			Main.Camera.GameViewMatrix.TransformationMatrix
		);

		Main.PlayerRenderer.DrawPlayerHead(Main.Camera, player, new Vector2(stream.Width / 2 - 4, stream.Height / 2 - 4));

		Main.spriteBatch.End();
	}
}

public class PositionMover : ModPlayer
{
	public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
	{
		if (!PlayerRenderer.RenderingSpritesheet) return;

		drawInfo.Position = Main.screenPosition + new Vector2(11, 8 + PlayerRenderer.FrameIndex * 54);
		drawInfo.isSitting = PlayerRenderer.Frames[PlayerRenderer.FrameIndex] is SittingFrame;
	}
}

public class RenderStream : IDisposable
{
	public string OutputName { get; init; }

	public RenderTarget2D Target { get; init; }
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
		Directory.CreateDirectory(PlayerRenderer.SavePath);

		var path = Path.Combine(PlayerRenderer.SavePath, OutputName);
		using var stream = File.Create(path);

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
			caller.Reply($"Game Zoom must be 100% for the sprites to render properly", Color.Red);
			caller.Reply($"If the slider doesn't go down to 100%, disable Forced Zoom in the tModLoader Settings", Color.LightSalmon);
			return;
		}

		if (args.Length == 0)
		{
			caller.Reply("Must provide <name> to the render command", Color.Red);
			return;
		}

		var name = args[0];

		PlayerRenderer.Render(caller.Player, name);
		PlayerRenderer.RenderHead(caller.Player, name);

		caller.Reply($"Spritesheet saved to {Path.Combine(PlayerRenderer.SavePath, $"{name}.png")}", Color.LightBlue);
		caller.Reply($"Head saved to {Path.Combine(PlayerRenderer.SavePath, $"{name}_Head.png")}", Color.LightBlue);
		caller.Reply("The Spritesheet should be a drop-in replacement for a Town NPC Sprite, but you should probably crop the Head sprite manually", Color.LightSalmon);
	}
}