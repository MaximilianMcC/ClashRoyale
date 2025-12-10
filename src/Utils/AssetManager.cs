using System.Reflection;
using Raylib_cs;

// TODO: Maybe just don't do any of this and just load the whole asset
// TODO: Directory into a temporary directory somewhere hidden 
class AssetManager
{
	public static Dictionary<string, Texture2D> Textures = [];

	private static readonly string Namespace = "clash_royale";

	private static byte[] GetAssetBytes(string assetPath, out string extension)
	{
		// Get the current assembly and namespace
		// so we can extract the assets and whatnot
		Assembly assembly = Assembly.GetExecutingAssembly();

    	// Clean and format the asset path for embedded resources
    	assetPath = assetPath.TrimStart('.', '/', '\\').Replace("/", ".").Replace("\\", ".");
    	string path = $"{Namespace}.{assetPath}";
    	extension = Path.GetExtension(assetPath);

		// Get the stream containing the assets data
		using (Stream stream = assembly.GetManifestResourceStream(path))
		{
			// Check for if there is a stream or not
			if (stream == null)
			{
				throw new Exception("Could not find embedded asset at " + path);
			}

			// Get the stream as a byte array
			byte[] bytes;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				// Extract the bytes
				stream.CopyTo(memoryStream);

				// Give them back
				bytes = memoryStream.ToArray();
				return bytes;
			}
		}

	}

	private static string GetAssetAsTempFile(string assetPath, bool useOriginalName = false)
	{
		// Get the asset byte array and extension
		byte[] bytes = GetAssetBytes(assetPath, out string extension);

		// Make a new temporary file, and rename it to
		// have the correct file extension
		string tempFilepath;
		if (useOriginalName)
		{
			// Make a temporary file in the temp directory with the
			// same filename as the original file we're tryna get
			tempFilepath = Path.Join(Path.GetTempPath(), Path.GetFileName(assetPath));
		}
		else
		{
			// Make a new temp file, and change the extension
			// to match the file that we're tryna open/use
			tempFilepath = Path.GetTempFileName();
			tempFilepath = Path.ChangeExtension(tempFilepath, extension);
		}

		// Write all of the bytes to the file
		File.WriteAllBytes(tempFilepath, bytes);

		// Return the path of the temporary file
		return tempFilepath;
	}

	private static string CreateAssetAsTempFileInDirectory(string assetPath, string directoryPath)
	{
		// Get the asset byte array and extension
		byte[] bytes = GetAssetBytes(assetPath, out string extension);

		// Get the path where the temp file
		// will be created
		string path = Path.Join(directoryPath, Path.GetFileName(assetPath));

		// Create the file at the path
		File.WriteAllBytes(path, bytes);

		// Give them back the path to the file
		// that they just created
		return path;
	}

	public static Image LoadImage(string path)
	{
		// Get the asset byte array and extension
		byte[] bytes = GetAssetBytes(path, out string extension);

		// Load the image from the byte array
		Image image = Raylib.LoadImageFromMemory(extension, bytes);

		// Give back the loaded image
		return image;
	}

	public static Texture2D LoadTexture(string path)
	{
		// Load the texture as an image
		// then convert it to a texture
		Image image = LoadImage(path);
		Texture2D texture = Raylib.LoadTextureFromImage(image);

		// Unload the image since we no longer need it
		Raylib.UnloadImage(image);

		// Give back the loaded texture
		return texture;
	}

	public static Sound LoadSound(string path)
	{
		// Get the asset byte array and extension
		byte[] bytes = GetAssetBytes(path, out string extension);

		// Load the wave data from the raw bytes then use
		// that to load the sound
		Wave wave = Raylib.LoadWaveFromMemory(extension, bytes);
		Sound sound = Raylib.LoadSoundFromWave(wave);

		// Unload the wave data because we don't need it anymore
		Raylib.UnloadWave(wave);

		// Give back the sound
		return sound;
	}

	public static Music LoadMusic(string path)
	{
		// Get the asset byte array and extension
		byte[] bytes = GetAssetBytes(path, out string extension);

		// Load the music from the data
		Music music = Raylib.LoadMusicStreamFromMemory(extension, bytes);

		// Give back the sound
		return music;
	}

	public static Font LoadFont(string path)
	{
		// Get the asset byte array and extension
		byte[] bytes = GetAssetBytes(path, out string extension);

    	// ASCII for characters are from 32 to 126
    	int[] fontChars = Enumerable.Range(32, 126).ToArray(); 

		// Load the font from the raw bytes
		Font font = Raylib.LoadFontFromMemory(extension, bytes, 32, fontChars, fontChars.Length);

		// Give back the font
		return font;
	}

	public static Model LoadGlbModel(string path)
	{
		// Get the path to both the model, and the material file
		string modelPath = GetAssetAsTempFile(path);

		// Actually load the model
		Model model = Raylib.LoadModel(modelPath);

		// Get rid of the temporary file we just made
		File.Delete(modelPath);

		// Give back the loaded model
		return model;
	}

	public static void UnloadEverything()
	{
		foreach (Texture2D texture in Textures.Values)
		{
			Raylib.UnloadTexture(texture);
		}
	}

	//! Debug
	public static void PrintEmbeddedAssets()
	{
		// Get all of the assets that are embedded rn
		Assembly assembly = Assembly.GetExecutingAssembly();
		string[] assets = assembly.GetManifestResourceNames();
		
		// Print them all
		Console.WriteLine("All embedded assets:");
		foreach (string asset in assets) Console.WriteLine("- " + asset);
		Console.WriteLine();
	}
}