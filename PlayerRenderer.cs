using Terraria;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;
using PlayerRenderer.Common;
using Microsoft.Xna.Framework.Graphics;

namespace PlayerRenderer;

public class PlayerRenderer : Mod
{
	public const int WIDTH = 44;
	public const int HEIGHT = 54;

	public const int HEAD_SIZE = 44;

	public static bool RenderingSpritesheet { get; private set; } = false;
	public static int FrameIndex { get; private set; } = 0;

	public static readonly string SavePath = Path.Combine(Main.SavePath, "Sprites");

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

		using var interceptor = new SpriteInterceptor(WIDTH, HEIGHT * Frames.Length) { OutputPath = path };

		for (FrameIndex = 0; FrameIndex < Frames.Length; FrameIndex++)
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
        using var interceptor = new SpriteInterceptor(HEAD_SIZE, HEAD_SIZE) { OutputPath = path };

        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		Main.PlayerRenderer.DrawPlayerHead(Main.Camera, player, new Vector2(interceptor.Width / 2 - 4, interceptor.Height / 2 - 4));
		Main.spriteBatch.End();
	}
}
