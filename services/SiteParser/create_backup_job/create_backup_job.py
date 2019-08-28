from .zip_backup_creator import ZipBackupCreator
from .dropbox_sender import DropboxSender


class CreateBackupJob:
    def __init__(self):
        self.__backup_creator = ZipBackupCreator()
        self.__dropbox_sender = DropboxSender()

    def execute(self):
        print("Backup job started")
        try:
            backup_path = self.__backup_creator.create_backup()
            self.__dropbox_sender.backup_to_dropbox(backup_path)
        except Exception as error:
            print("Error in backup creation: " + str(error))
