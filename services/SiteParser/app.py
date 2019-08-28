from create_backup_job.create_backup_job import CreateBackupJob
import schedule
import time


def execute_backup_job():
    print("Executing job")
    create_backup_job = CreateBackupJob()
    create_backup_job.execute()


schedule.every().day.at("22:13").do(execute_backup_job)
schedule.every().day.at("22:14").do(execute_backup_job)

while True:
    schedule.run_pending()
    time.sleep(1)
