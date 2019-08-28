import json


class PathsConfig:
    temp_backup_path = ""
    dirs_to_backup = []

    def __init__(self):
        with open("config.json") as json_file:
            data = json.load(json_file)
            self.__dict__ = data["paths"]
