import cv2

def read_img(path, rgb=True):
    if rgb:
        img = cv2.imread(path)
        img = img[:, :, ::-1]
    else:
        img = cv2.imread(path, cv2.IMREAD_GRAYSCALE)
    return img