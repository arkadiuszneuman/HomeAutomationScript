import json


class Group:
    def __init__(self, data):
        self.__dict__ = data


class SettingsConfig:
    class __SettingsConfig:
        groups = []
        dropbox_invoices_dir: str

        def __init__(self):
            with open("settings.json", encoding='utf-8') as json_file:
                data = json.load(json_file)
                self.dropbox_invoices_dir = data["dropbox_invoices_dir"]
                for group in data["groups"]:
                    self.groups.append(Group(group))

    __instance: __SettingsConfig = None

    def __init__(self):
        if not SettingsConfig.__instance:
            SettingsConfig.__instance = SettingsConfig.__SettingsConfig()

    def get(self):
        return self.__instance
