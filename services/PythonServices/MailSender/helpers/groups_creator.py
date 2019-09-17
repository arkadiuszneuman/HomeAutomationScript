import os
from configs.settings import SettingsConfig

class Group:
    name: str
    config_group: str


class GroupsCreator:
    def __get_last_dir(self, path):
        return os.path.basename(os.path.normpath(path))

    def get_groups(self, files: []):
        config = SettingsConfig().get()

        dictionary = {}
        # for
        for f in files:
            last_dir = self.__get_last_dir(os.path.dirname(f)).strip("C:\\Users\\arkad\\Arek\\Dropbox\\Faktury\\09")

            if last_dir not in dictionary:
                dictionary[last_dir] = []

            dictionary[last_dir].append(f)

        groups_dictionary = {}
        for group in config.groups:
            if group.dir_name in dictionary:
                groups_dictionary[group] = dictionary[group.dir_name]

        if '' in dictionary:
            groups_dictionary[""] = dictionary['']

        return groups_dictionary

