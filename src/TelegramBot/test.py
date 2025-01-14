from utils import  load_json
from neural_network import ONNXModel

path_to_model = load_json('config.json')['path_to_model']
model = ONNXModel(path_to_model)

path_to_image = r"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\Кондурчинская\Kondur3_122842_1.jpg"
