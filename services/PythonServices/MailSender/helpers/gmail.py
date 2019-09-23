from configs.cache import Cache
import smtplib
import imaplib
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
from email.mime.base import MIMEBase
from email import encoders
import os
from configs.secrets import SecretsConfig
from datetime import datetime
from .logger import Logger


class Gmail:

    def __init__(self):
        self.__logger = Logger().get_logger()
        self.__secrets_config = SecretsConfig()
        self.__cache = Cache()

    def __create_payload(self, file):
        attach_file_name = file.full_path
        self.__cache.add_sent_file(file.path_display)
        with open(attach_file_name, 'rb') as attach_file:
            payload = MIMEBase('application', 'octate-stream')
            payload.set_payload(attach_file.read())
            encoders.encode_base64(payload)
            file_name = os.path.basename(attach_file_name)
            payload.add_header('Content-Disposition', "attachment; filename= %s" % file_name)
            return payload

    def __get_subject(self, group):
        text_to_add = ""

        if not group == '':
            text_to_add = " " + group.translation
            text_to_add += " " + datetime.now().strftime('%B')

        return self.__secrets_config.title.replace("{0}", text_to_add)

    def __get_content(self, group):
        text_to_add = ""

        if not group == '':
            text_to_add = " " + group.for_translation

        return self.__secrets_config.content.replace("{0}", text_to_add)

    def __create_message(self, item):
        message = MIMEMultipart()
        message['From'] = self.__secrets_config.from_name
        message['To'] = ", ".join(self.__secrets_config.mail_to)
        message['Subject'] = self.__get_subject(item[0])
        message.attach(MIMEText(self.__get_content(item[0]), 'plain'))

        for file in item[1]:
            message.attach(self.__create_payload(file))

        return message.as_string()

    def send_emails(self, groups):
        with smtplib.SMTP_SSL('smtp.gmail.com', 465) as session:
            session.login(self.__secrets_config.gmail_user, self.__secrets_config.gmail_password)
            for item in groups.items():
                message = self.__create_message(item)
                session.sendmail(self.__secrets_config.gmail_user + "@gmail.com",
                                 self.__secrets_config.gmail_user + "@gmail.com",
                                 message)

                self.__cache.save()

                group = item[0]
                if not group == '':
                    self.__logger.info("Group mails " + item[0].dir_name + " sent")
                else:
                    self.__logger.info("Noname group mails sent")
