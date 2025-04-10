import cv2

def read_img(path, rgb=True):
    if rgb:
        img = cv2.imread(path)
        img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    else:
        img = cv2.imread(path, cv2.IMREAD_GRAYSCALE)
    return img