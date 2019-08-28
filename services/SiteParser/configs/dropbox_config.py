import json


class DropboxConfig:
    max_files: int
    timeout_seconds: int

    def __init__(self):
        with open("config.json") as json_file:
            data = json.load(json_file)
            self.__dict__ = data["dropbox"]
