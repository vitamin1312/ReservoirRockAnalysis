import onnxruntime as ort


class ONNXModel:
    def __init__(self, path_to_model):

        self.path_to_model = path_to_model
        self._session = ort.InferenceSession(path_to_model, providers=['CUDAExecutionProvider'])
        print(self._session)
