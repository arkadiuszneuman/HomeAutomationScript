from dependency_injector import containers, providers
from create_backup_job.create_backup_job import CreateBackupJob
from create_backup_job.create_backup_job import DropboxSender
from create_backup_job.create_backup_job import ZipBackupCreator
import logging
import sys


class CustomFormatter(logging.Formatter):
    """Logging Formatter to add colors and count warning / errors"""

    grey = "\x1b[38;21m"
    yellow = "\x1b[33;21m"
    red = "\x1b[31;21m"
    bold_red = "\x1b[31;1m"
    reset = "\x1b[0m"
    format = "%(asctime)s %(name)s [%(levelname)s] %(message)s"

    FORMATS = {
        logging.DEBUG: grey + format + reset,
        logging.INFO: grey + format + reset,
        logging.WARNING: yellow + format + reset,
        logging.ERROR: red + format + reset,
        logging.CRITICAL: bold_red + format + reset
    }

    def format(self, record):
        log_fmt = self.FORMATS.get(record.levelno)
        formatter = logging.Formatter(log_fmt)
        return formatter.format(record)


class IocContainer(containers.DeclarativeContainer):
    __root = logging.getLogger("BackupCreator")
    __root.setLevel(logging.DEBUG)

    __handler = logging.StreamHandler(sys.stdout)
    __handler.setLevel(logging.DEBUG)
    __handler.setFormatter(CustomFormatter())
    __root.addHandler(__handler)
    logger = providers.Object(__root)

    dropbox_sender = providers.Factory(DropboxSender,
                                       logger=logger)
    zip_backup_creator = providers.Factory(ZipBackupCreator,
                                           logger=logger)
    create_backup_job = providers.Factory(CreateBackupJob,
                                          backup_creator=zip_backup_creator,
                                          dropbox_sender=dropbox_sender,
                                          logger=logger)
