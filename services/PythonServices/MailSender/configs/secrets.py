import json


class SecretsConfig:
    dropbox_token: str
    gmail_user: str
    gmail_password: str
    title: str
    content: str
    from_name: str

    def __init__(self):
        with open("secrets.json", encoding='utf-8') as json_file:
            data = json.load(json_file)
            self.__dict__ = data
