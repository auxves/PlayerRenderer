using Terraria;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PlayerRenderer.Common;

public class SpriteInterceptor : IDisposable
{
    public string OutputPath { get; init; }

    public RenderTarget2D Target { get; private init; }
    public int Width => Target.Width;
    public int Height => Target.Height;

    public SpriteInterceptor(int width, int height)
    {
        var device = Main.spriteBatch.GraphicsDevice;

        Target = new RenderTarget2D(device, width, height);

        device.SetRenderTarget(Target);
        device.Clear(Color.Transparent);
    }

    public void Dispose()
    {
        using var stream = File.Create(OutputPath);

        Target.SaveAsPng(stream, Width, Height);
        Target.Dispose();
    }
}
