using Terraria;

namespace PlayerRenderer;

public abstract class Frame
{
	public virtual void Setup(Player player) {}
}

public sealed class PoseFrame : Frame
{
	public static readonly PoseFrame Default = new() { HeadFrame = 0, BodyFrame = 0, LegFrame = 0 };

	public int HeadFrame { get; init; }
	public int BodyFrame { get; init; }
	public int LegFrame { get; init; }

	public override void Setup(Player player)
	{
		player.headFrame.Y = player.headFrame.Height * HeadFrame;
		player.bodyFrame.Y = player.bodyFrame.Height * BodyFrame;
		player.legFrame.Y = player.legFrame.Height * LegFrame;
	}
}

public sealed class SittingFrame : Frame {}