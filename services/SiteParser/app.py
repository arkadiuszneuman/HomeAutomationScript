import ioc_container
from create_backup_job.create_backup_job import CreateBackupJob
import schedule
import logging

container = ioc_container.IocContainer()


def execute_backup_job():
    create_backup_job: CreateBackupJob = container.create_backup_job()
    logger: logging.Logger = container.logger()
    logger.info("Executing job")
    create_backup_job.execute()

execute_backup_job()

schedule.every().day.at("22:11").do(execute_backup_job)
schedule.every().day.at("22:14").do(execute_backup_job)

# while True:
#     schedule.run_pending()
#     time.sleep(1)
