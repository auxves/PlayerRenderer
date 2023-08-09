# PlayerRenderer

Player Renderer lets you render your player to a Town NPC Spritesheet.

Video tutorial with examples: https://youtu.be/MzhXoQlCsTM

## Generating The Sprite

1. Drip out your character
2. Zoom out to 100% (if the slider doesn't go down to 100%, disable Forced Zoom in the tModLoader Settings)
3. Ensure proper lighting (this will affect the resulting sprites) - Daylight recommended
4. Face to the left (not required, but NPC sprites are expected to face left)
5. Type `/render <name>` in the chat, substituting <name> with your desired name

> The sprites will be saved in the `Sprites` directory. next to `Players` and `Worlds`

## Usage In Mod

Both sprites should be placed alongisde your NPC.cs file.

```cs
[AutoloadHead]
public class ExamplePerson : ModNPC
    public override void SetStaticDefaults()
        // These lines
        Main.npcFrameCount[Type] = 25;
        NPCID.Sets.ExtraFramesCount[Type] = 9;
    }
}
```

A more complete example of a Town NPC implementation can be found in the tModLoader ExampleMod.

## Supports

- Plain Character
- Armors
- Vanities
- Accessories
