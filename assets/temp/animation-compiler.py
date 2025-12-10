from PIL import Image
import os

# Get the path to all of the animation frames
frame_path = "./frames/"
paths = os.listdir(frame_path)

# Get all of the image
images = []
for path in paths:

	# Ignore anything that isn't a png
	if not path.endswith(".png"): continue
	if path.endswith("animation.png"): continue
	
	# Load in the image
	images.append(Image.open(frame_path + path))

# Get how many frames
frame_count = len(images)

# Make the image for the actual texture
frame_width = images[0].width
frame_height = images[0].height
animation = Image.new("RGBA", (frame_count * frame_width, frame_height))

# Paste all of the image onto the
# new animation image thingy yk
for i in range(frame_count):
	animation.paste(images[i], (frame_width * i, 0))

# Save the image
animation.save(frame_path + "animation.png")

# Yap sesh
print(f"Exported animation ({frame_count} frames)\nUse a spacing offset thingy of {frame_width}.\nIt's in {frame_path} btw")