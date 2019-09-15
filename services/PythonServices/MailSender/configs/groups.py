import json


class Group:
    def __init__(self, data):
        self.__dict__ = data


class GroupsConfig:
    groups = []

    def __init__(self):
        with open("groups.json", encoding='utf-8') as json_file:
            data = json.load(json_file)
            for group in data["groups"]:
                self.groups.append(Group(group))
