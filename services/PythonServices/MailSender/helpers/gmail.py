import smtplib
import imaplib
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
from email.mime.base import MIMEBase
from email import encoders
import os
from configs.secrets import SecretsConfig
from datetime import datetime


class Gmail:
    secrets_config = SecretsConfig()

    def __create_payload(self, file):
        attach_file_name = file
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

        return self.secrets_config.title.replace("{0}", text_to_add)

    def __get_content(self, group):
        text_to_add = ""

        if not group == '':
            text_to_add = " " + group.for_translation

        return self.secrets_config.content.replace("{0}", text_to_add)

    def __create_message(self, item):
        message = MIMEMultipart()
        message['From'] = self.secrets_config.from_name
        message['To'] = self.secrets_config.gmail_user + "@gmail.com"
        message['Subject'] = self.__get_subject(item[0])
        message.attach(MIMEText(self.__get_content(item[0]), 'plain'))

        for file in item[1]:
            message.attach(self.__create_payload(file))

        return message.as_string()

    def send_emails(self, groups):
        with smtplib.SMTP_SSL('smtp.gmail.com', 465) as session:
            session.login(self.secrets_config.gmail_user, self.secrets_config.gmail_password)
            for item in groups.items():
                message = self.__create_message(item)
                session.sendmail(self.secrets_config.gmail_user + "@gmail.com",
                                 self.secrets_config.gmail_user + "@gmail.com",
                                 message)

                print('Mail Sent')

