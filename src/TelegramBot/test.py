import PIL.Image
import PIL
import numpy as np
import pathlib

from utils import  load_json, show_image
from neural_network import ONNXModel

path_to_model = load_json('config.json')['path_to_model']
model = ONNXModel(path_to_model)

path_to_image = pathlib.Path(r"C:\Users\Viktor\Downloads\photo_2025-01-15_16-06-17.jpg")
image = np.array(PIL.Image.open(path_to_image))


image_mask = model.predict_image(image)
print(image_mask)

show_image(image_mask)
