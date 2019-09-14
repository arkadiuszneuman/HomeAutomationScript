import json


class SecretsConfig:
    gmail_user: str
    gmail_password: str

    def __init__(self):
        with open("secrets.json") as json_file:
            data = json.load(json_file)
            self.__dict__ = data
