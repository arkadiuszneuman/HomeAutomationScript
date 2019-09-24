import json
import os


class Cache:
    class __Cache:
        def __init__(self):
            self.files_send = []
            if os.path.isfile("cache.json"):
                with open("cache.json", encoding='utf-8') as json_file:
                    self.__dict__ = json.load(json_file)

        def save_to_json(self):
            with open('cache.json', 'w') as outfile:
                json.dump(self.__dict__, outfile, indent=4)

    __instance: __Cache = None

    def __init__(self):
        if not Cache.__instance:
            Cache.__instance = Cache.__Cache()

    def save(self):
        self.__instance.save_to_json()

    def add_sent_file(self, file_path: str):
        if file_path not in self.__instance.files_send:
            self.__instance.files_send.append(file_path)

    def get_send_files(self):
        return self.__instance.files_send

