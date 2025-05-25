import PIL.Image
import onnxruntime as ort
import numpy as np
import cv2
import PIL
import tempfile


class ONNXModel:
    def __init__(self, path_to_model, size=2048, n_parts_y=4, n_parts_x=6):
        self.path_to_model = path_to_model
        self._session = ort.InferenceSession(path_to_model, providers=['CUDAExecutionProvider'])
        self.size = size
        self.n_parts_y = n_parts_y
        self.n_parts_x = n_parts_x

    def predict(self, image):
        return self._session.run(['output'], {'input': image[None, ...]})
    
    @staticmethod
    def read_img(path):
        return np.array(PIL.Image.open(path).convert('RGB'))

    @staticmethod
    def get_color(clls):
        if clls == 0:
            return [0, 0, 0]
        elif clls == 1:
            return [0, 255, 0]
        elif clls == 2:
            return [255, 0, 0]
        elif clls == 3:
            return [255, 255, 0]
        
    def get_coordinates(self, shape):
        y_step = int(shape[0] / self.n_parts_y)
        x_step = int(shape[1] / self.n_parts_x)

        return  [((y_step*j, x_step*i), (j, i))
                                                for j in range(1, self.n_parts_y, 2)
                                                    for i in range(1, self.n_parts_x, 2)]
        
    def get_image_mask(self, mask):
        s = mask.shape
        return np.array([self.get_color(pixel) for row in mask for pixel in row]).reshape(s + (3,))
    
    def get_image_corners(self, i, j, x_step, y_step):

        if j == 1:
            min_y, max_y = 0, self.size

        else:
            min_y, max_y = 4*y_step-self.size, 4*y_step

        if i == 1:
            min_x, max_x = 0, self.size
        elif i == 3:
            delta = self.size - 2*x_step
            min_x, max_x = 2*x_step-delta//2, 4*x_step+delta//2
        else:
            min_x, max_x = 6*x_step-self.size, 6*x_step

        return min_x, min_y, max_x, max_y
    
    def get_mask_corners(self, i, j, x_step, y_step):

        if j == 1:
            min_y, max_y = 0, 2*y_step

        else:
            min_y, max_y = self.size - 2*y_step, self.size

        if i == 1:
            min_x, max_x = 0, 2*x_step
        elif i == 3:
            middle = self.size//2
            min_x, max_x = middle - x_step, middle + x_step
        else:
            min_x, max_x = self.size - 2*x_step, self.size

        return min_x, min_y, max_x, max_y
    
    def predict_mask(self, image):

        y_step = int(image.shape[1] / self.n_parts_y)
        x_step = int(image.shape[2] / self.n_parts_x)

        part_paths = []

        num = 0
        flag = True
        coordinates = self.get_coordinates(image.shape)

        for (y, x), (j, i) in coordinates:
            min_x, min_y, max_x, max_y = self.get_image_corners(i, j, x_step, y_step)
            image_part = image[:, min_y:max_y, min_x:max_x]
            mask = self.predict(image_part)[0][0]
            mask = mask.argmax(0)
            image_mask = self.get_image_mask(mask)

            with tempfile.NamedTemporaryFile(delete=False, suffix='.png') as temp_file:
                temp_path = temp_file.name
                flag = cv2.imwrite(temp_path, image_mask[:, :, ::-1]) and flag
                part_paths.append(temp_path)
                num += 1

        assert flag

        return part_paths
    
    def predict_full_mask(self, image):

        init_size = (image.shape[2], image.shape[1])
        image = np.transpose(image, (1, 2, 0))
        image = cv2.resize(image, (self.size, self.size))
        image = np.transpose(image, (2, 0, 1))
        mask = self.predict(image)[0][0]
        mask = mask.argmax(0)
        mask = self.get_image_mask(mask).astype(np.uint8)
        mask = cv2.resize(mask, init_size)
        
        return mask

    def assemble_mask(self, orig_shape, parts_path):
        y_step = int(orig_shape[1] / self.n_parts_y)
        x_step = int(orig_shape[2] / self.n_parts_x)

        parts = list()
        coordinates = self.get_coordinates(orig_shape)

        for path, ((y, x), (j, i)) in zip(parts_path, coordinates):

            min_x, min_y, max_x, max_y = self.get_mask_corners(i, j, x_step, y_step)

            part = self.read_img(path)
            crop_part = part[min_y:max_y, min_x:max_x]
            parts.append(crop_part)

        upper_part = np.concatenate(parts[:3], axis=1)
        lower_part = np.concatenate(parts[3:], axis=1)

        full_image_mask = np.concatenate([upper_part, lower_part], axis=0)

        return full_image_mask
    
    def preprocess(self, image):
        image = np.array(image)
        cut = True if (image.shape[0] > self.size and image.shape[1] > self.size) else False
        image = np.transpose(image, (2, 0, 1))
        return (image / 255).astype(np.float32), cut
    
    def predict_image(self, image):
        transp_image, cut = self.preprocess(image)

        if cut:
            parts_path = self.predict_mask(transp_image)
            mask = self.assemble_mask(transp_image.shape, parts_path)
        else:
            mask = self.predict_full_mask(transp_image)
            
        not_mask = np.array(~np.any(mask, axis=2))
        image_without_pores = image * np.stack([not_mask] * 3, axis=2)
        image_wiht_pores = image_without_pores + mask

        return image_wiht_pores
