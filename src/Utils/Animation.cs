using System.Numerics;
using Raylib_cs;

class Animation
{
	private Texture2D[] frames;

	private double fps;
	private int totalFrames;
	
	private double timeOfLastFrame;
	private int currentFrame;

	public bool JustCompletedALoop { get; private set; }
	public bool Loop = true;

	// John frame (inventor of animations)
	//? Animations go on the x btw
	public Animation(string path, int frameWidth, float fps, bool shouldLoop = true)
	{
		// Load in the actual animation image
		Image animation = AssetManager.LoadImage(path);

		// Set frame information stuff
		// TODO: Don't use `this.`	
		//? forgot why the 1 thingy but idk
		this.fps = 1d / fps;
		Loop = shouldLoop;
		totalFrames = animation.Width / frameWidth;
		frames = new Texture2D[totalFrames];
		
		// Loop through every frame, and draw
		// it onto a render texture then bake
		// it onto a normal texture
		for (int i = 0; i < totalFrames; i++)
		{
			// Get the section of the image
			// that we wanna use for the frame
			// then get the frame from that
			Rectangle source = new Rectangle(frameWidth * i, 0, frameWidth, animation.Height);
			Image frameImage = Raylib.ImageFromImage(animation, source);

			// Turn the image into a texture
			Texture2D frame = Raylib.LoadTextureFromImage(frameImage);
			frames[i] = frame;

			// Be a tidy kiwi
			Raylib.UnloadImage(frameImage);
		}

		// Get rid of the animation image
		// since its now been fully baked
		Raylib.UnloadImage(animation);
	}

	// Move onto the next frame if needed
	public void Animate()
	{
		// Check for if we wanna loop it
		if (JustCompletedALoop && Loop == false) return;

		// Get the current time and the elapsed time
		double currentTime = Raylib.GetTime();
		double elapsedTime = currentTime - timeOfLastFrame;

		// Check for if we're due for the next frame
		if (elapsedTime >= fps)
		{
			currentFrame++;
			timeOfLastFrame = currentTime;
		}

		// Loop the animation
		//? -1 because we start counting at 0
		if (currentFrame > (totalFrames - 1))
		{
			JustCompletedALoop = true;
			currentFrame = 0;
		}
		else JustCompletedALoop = false;
	}

	// Get the current frame of the animation
	// TODO: Fancy as debug thing that shows frame index
	public Texture2D GetFrame()
	{
		return frames[currentFrame];
	}

	public void Restart()
	{
		currentFrame = 0;
		JustCompletedALoop = true;
	}

	public void Unload()
	{
		// Get rid of all the frames
		for (int i = 0; i < totalFrames; i++)
		{
			// Unload the current frame
			Raylib.UnloadTexture(frames[i]);
		}
	}
}