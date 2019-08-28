import os
from os import path
from distutils.dir_util import copy_tree
import datetime
import zipfile
from configs.paths_config import PathsConfig
import shutil


class ZipBackupCreator:
    paths_config = PathsConfig()

    def create_backup(self):
        print("Starting to create zip")

        backup_dir = self.__get_backup_dir()
        self.__copy_dirs_to_backup(backup_dir)
        zip_file_path = self.__get_zip_file_path(backup_dir)
        self.__create_zip_file(backup_dir, zip_file_path)

        return zip_file_path

    @staticmethod
    def __get_full_path(pth):
        return path.normpath(path.join(os.getcwd(), pth))

    def __get_backup_dir(self):
        backup_dir = self.__get_full_path(self.paths_config.temp_backup_path)
        if path.exists(backup_dir):
            shutil.rmtree(backup_dir)

        return backup_dir

    def __copy_dirs_to_backup(self, backup_dir):
        for directory in self.paths_config.dirs_to_backup:
            source_dir = self.__get_full_path(directory)
            if not path.exists(source_dir):
                print("Dir {0} does not exist".format(source_dir))

            dir_name = path.basename(source_dir)
            destination_dir = path.join(backup_dir, "create", dir_name)

            self.__perform_deep_copy(source_dir, destination_dir)

            print("Copied {source} to {dest}".format(source=source_dir, dest=destination_dir))

    @staticmethod
    def __create_dir(pth):
        os.makedirs(pth, exist_ok=True)

    def __perform_deep_copy(self, source_dir, destination_dir):
        self.__create_dir(destination_dir)
        copy_tree(source_dir, destination_dir)

    @staticmethod
    def __get_zip_file_path(backup_dir):
        date_string = datetime.datetime.now().strftime("%Y-%m-%d-%H%M%S")
        backup_file_name = "Win_backup_{0}.zip".format(date_string)
        zip_file_path = path.join(backup_dir, backup_file_name)
        if os.path.exists(zip_file_path):
            os.remove(zip_file_path)

        return zip_file_path

    @staticmethod
    def __create_zip_file(backup_dir, zip_file_path):
        with zipfile.ZipFile(zip_file_path, 'w', zipfile.ZIP_DEFLATED) as zip:
            for root, dirs, files in os.walk(path.join(backup_dir, "create")):
                for file in files:
                    zip.write(path.join(root, file))

        file_size_b = path.getsize(zip_file_path)
        file_size_kb = file_size_b / 1024
        file_size_mb = file_size_kb / 1024
        print("Created zip: {zipFilePath}. File size: {fileSizeMB:.3f}"
              .format(zipFilePath=zip_file_path, fileSizeMB=round(file_size_mb, 3)))
