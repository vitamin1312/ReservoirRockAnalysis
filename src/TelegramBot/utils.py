import json
import cv2

def load_json(file_path):
    with open(file_path, 'r') as f:
        return json.load(f)
    
def show_image(image):
    if len(image.shape) == 3 and image.shape[2] == 3:
        image = image[:, :, ::-1]
    cv2.imshow('Image', image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()
