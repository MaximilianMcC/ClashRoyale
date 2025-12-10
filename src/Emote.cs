using Raylib_cs;

class Emote
{
	private Animation animation;
	private Sound sound;
	public bool Playing { get; private set; } = false;

	public Emote(string animationPath, string audioPath, int emoteWidth)
	{
		// Load the animation
		animation = new Animation(animationPath, emoteWidth, 10.0f, true);

		// Load the sound
		sound = AssetManager.LoadSound(audioPath);
	}

	public void Play()
	{
		// Can't play if we're already playing
		if (Playing) return;
		Playing = true;

		animation.Restart();
		Raylib.PlaySound(sound);
	}

	public void Render()
	{
		if (Playing == false) return;

		// Once the sound has finished playing then end the emote
		if (Playing == true && Raylib.IsSoundPlaying(sound) == false)
		{
			Playing = false;
		}

		animation.Animate();
		Raylib.DrawTexture(animation.GetFrame(), 0, 0, Color.White);
	}

	~Emote()
	{
		animation.Unload();
		Raylib.UnloadSound(sound);
	}
}