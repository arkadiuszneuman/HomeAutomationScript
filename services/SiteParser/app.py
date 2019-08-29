import ioc_container
from create_backup_job.create_backup_job import CreateBackupJob
import logging
from flask import Flask
from flask import jsonify

container = ioc_container.IocContainer()

app = Flask(__name__)
app.config.update(
    JSON_AS_ASCII=False
)


@app.route('/createbackup')
def index():
    try:
        create_backup_job: CreateBackupJob = container.create_backup_job()
        logger: logging.Logger = container.logger()
        logger.info("Executing job")
        file_name = create_backup_job.execute()
    except Exception as error:
        return jsonify(
            success=False,
            message=str(error)
        )

    return jsonify(
        success="True",
        message=file_name
    )


if __name__ == '__main__':
    app.run(debug=True)
