import logging
import sys
from cmreslogging.handlers import CMRESHandler


class Logger:
    __is_initialized = False

    class __CustomFormatter(logging.Formatter):
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

    def __init__(self):
        self.__root = logging.getLogger("BackupCreator")
        if Logger.__is_initialized is False:
            Logger.__is_initialized = True
            self.__root.setLevel(logging.DEBUG)

            self.__handler = logging.StreamHandler(sys.stdout)
            self.__handler.setLevel(logging.DEBUG)
            self.__handler.setFormatter(self.__CustomFormatter())
            self.__root.addHandler(self.__handler)

            self.__handler2 = CMRESHandler(hosts=[{'host': 'localhost', 'port': 9200}],
                                           auth_type=CMRESHandler.AuthType.NO_AUTH,
                                           es_index_name="logstash_mailsender-{0:yyyy.MM.dd}")
            self.__handler2.setLevel(logging.DEBUG)
            self.__root.addHandler(self.__handler2)

    def get_logger(self):
        return self.__root
