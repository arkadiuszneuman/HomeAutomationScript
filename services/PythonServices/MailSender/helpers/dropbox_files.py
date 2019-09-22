import dropbox
import os
import tempfile
import shutil
from configs.settings import SettingsConfig
from configs.secrets import SecretsConfig
from .logger import Logger


class DropboxFiles:
    def __init__(self):
        self.__dbx = dropbox.Dropbox(SecretsConfig().dropbox_token)
        self.__temp_dir = os.path.join(tempfile.gettempdir(), "mail_sender")
        self.__logger = Logger().get_logger()

    def download_files(self):
        self.remove_temp_dir()
        self.__logger.info("Downloading files from " + SettingsConfig().get().dropbox_invoices_dir)

        files = self.__get_files(SettingsConfig().get().dropbox_invoices_dir)

        for file in files:
            output = os.path.join(self.__temp_dir, file.path_display[1:])
            if not os.path.exists(os.path.dirname(output)):
                os.makedirs(os.path.dirname(output))

            self.__logger.info("Downloading file " + file.path_display + " to " + output)
            self.__dbx.files_download_to_file(output, file.path_display)

    def remove_temp_dir(self):
        if os.path.isdir(self.__temp_dir):
            self.__logger.info("Removing temp dir " + self.__temp_dir)
            try:
                shutil.rmtree(self.__temp_dir)
            except:
                self.__logger.warning("Error while deleting directory")

    def __exit__(self, type, value, traceback):
        self.remove_temp_dir()

    def __enter__(self):
        return self

    def __get_files(self, dir):
        files = []
        response = self.__dbx.files_list_folder("/" + dir)
        for file in response.entries:
            if self.__is_file(file):
                files.append(file)
            elif self.__is_folder(file):
                files.extend(self.__get_files(file.path_display))

        return files

    def __is_file(self, dropbox_meta):
        return isinstance(dropbox_meta, dropbox.dropbox.files.FileMetadata)

    def __is_folder(self, dropbox_meta):
        return isinstance(dropbox_meta, dropbox.dropbox.files.FolderMetadata)