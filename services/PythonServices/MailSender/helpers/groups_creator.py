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
            last_dir = self.__get_last_dir(os.path.dirname(f.full_path))

            if last_dir not in dictionary:
                dictionary[last_dir] = []

            dictionary[last_dir].append(f)

        groups_dictionary = {}
        for key in dictionary:
            added = False
            for group in config.groups:
                if group.dir_name == key:
                    groups_dictionary[group] = dictionary[key]
                    added = True
            if not added:
                if '' not in dictionary:
                    groups_dictionary[''] = dictionary[key]
                else:
                    groups_dictionary[''].append(dictionary[key])

        return groups_dictionary

