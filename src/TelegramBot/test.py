import PIL.Image
import cv2
import PIL
import numpy as np
import pathlib

from utils import  load_json, show_image
from neural_network import ONNXModel

path_to_model = load_json('config.json')['path_to_model']
model = ONNXModel(path_to_model, size=2048 + 1024)

path_to_image = pathlib.Path("C:\\Users\\Viktor\\Documents\\IT\\ReservoirRockAnalysis\\data\\Кондурчинская\\Kondur3_122842_1.jpg")
image = np.array(PIL.Image.open(path_to_image))
image = np.transpose(image, (2, 0, 1))

image_mask = model.predict_image(image)

show_image(image_mask)
