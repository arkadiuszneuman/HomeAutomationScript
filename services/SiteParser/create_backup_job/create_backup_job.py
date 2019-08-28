from .zip_backup_creator import ZipBackupCreator
from .dropbox_sender import DropboxSender
from logging import Logger


class CreateBackupJob:
    def __init__(self, backup_creator: ZipBackupCreator, dropbox_sender: DropboxSender, logger: Logger):
        self.__backup_creator = backup_creator
        self.__dropbox_sender = dropbox_sender
        self.__logger = logger

    def execute(self):
        self.__logger.info("Backup job started")
        try:
            backup_path = self.__backup_creator.create_backup()
            self.__dropbox_sender.backup_to_dropbox(backup_path)
        except Exception as error:
            self.__logger.error("Error in backup creation: " + str(error))
