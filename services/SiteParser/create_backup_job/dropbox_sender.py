from logging import Logger

from configs.secrets import Secrets
from configs.dropbox_config import DropboxConfig
import dropbox
import os


class DropboxSender:
    def __init__(self, logger: Logger, secrets_config: Secrets, dropbox_config: DropboxConfig):
        self.__logger = logger
        self.__dropbox_config = dropbox_config
        self.__dbx = dropbox.Dropbox(secrets_config.dropbox_token)

    def backup_to_dropbox(self, file_path):
        file_name = self.__backup_file(file_path)
        self.__remove_old_backups()
        return file_name

    def __backup_file(self, file_path):
        file_name = "/" + os.path.basename(file_path)
        self.__logger.info("Uploading file {file} to Dropbox".format(file=file_name))
        with open(file_path, "rb") as file:
            self.__dbx.files_upload(file.read(), file_name)

        self.__logger.info("Uploaded file {file} to Dropbox".format(file=file_name))
        return file_name

    def __remove_old_backups(self):
        entries = self.__dbx.files_list_folder('').entries
        files_count_to_get = len(entries) - self.__dropbox_config.max_files
        if files_count_to_get > 0:
            files_to_delete = sorted(entries, key=lambda x: x.name)[:files_count_to_get]
            for file in files_to_delete:
                self.__dbx.files_delete_v2("/" + file.name)
                self.__logger.info("Removed old backup file: {removed_backup_file_name}".format(removed_backup_file_name=file.name))
