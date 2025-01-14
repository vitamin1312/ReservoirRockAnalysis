import json

def load_json(file_path):
    import json
    with open(file_path, 'r') as f:
        return json.load(f)