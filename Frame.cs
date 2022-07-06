using Terraria;

namespace PlayerRenderer;

public abstract record Frame
{
	public virtual void Setup(Player player) { }
	public virtual void Cleanup(Player player) { }

	public sealed record Pose(int HeadFrame, int BodyFrame, int LegFrame) : Frame
	{
		public Pose(int Frame) : this(Frame, Frame, Frame) { }

		public static readonly Pose Default = new(0);

		public override void Setup(Player player)
		{
			player.headFrame.Y = player.headFrame.Height * HeadFrame;
			player.bodyFrame.Y = player.bodyFrame.Height * BodyFrame;
			player.legFrame.Y = player.legFrame.Height * LegFrame;
		}
	}

	public sealed record Sitting : Frame { }

	public sealed record Blinking : Frame
	{
		public override void Setup(Player player)
		{
			Pose.Default.Setup(player);
			player.eyeHelper.BlinkBecausePlayerGotHurt();
			player.eyeHelper.Update(player);
		}

		public override void Cleanup(Player player)
		{
			for (int i = 0; i < 20; i++)
			{
				player.eyeHelper.Update(player);
			}
		}
	}
}
