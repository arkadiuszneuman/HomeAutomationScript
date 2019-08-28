from dependency_injector import containers, providers
from create_backup_job.create_backup_job import CreateBackupJob
from create_backup_job.create_backup_job import DropboxSender
from create_backup_job.create_backup_job import ZipBackupCreator


class IocContainer(containers.DeclarativeContainer):
    dropbox_sender = providers.Factory(DropboxSender)
    zip_backup_creator = providers.Factory(ZipBackupCreator)
    create_backup_job = providers.Factory(CreateBackupJob,
                                          backup_creator=zip_backup_creator,
                                          dropbox_sender=dropbox_sender)
