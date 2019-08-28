import json


class Secrets:
    dropbox_token = ""

    def __init__(self):
        with open("secrets.json") as json_file:
            data = json.load(json_file)
            self.__dict__ = data;
