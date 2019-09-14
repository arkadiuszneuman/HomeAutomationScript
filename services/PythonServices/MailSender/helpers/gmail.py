import smtplib
import imaplib
from configs.secrets import SecretsConfig


class Gmail:
    secrets_config = SecretsConfig()
    sent_from = secrets_config.gmail_user
    to = [secrets_config.gmail_user + "@gmail.com"]
    subject = 'OMG Super Important Message'
    body = 'Hey, what\'s up?\n\n- You'

    email_text = """\
    From: %s
    To: %s
    Subject: %s
    
    %s
    """ % (sent_from, ", ".join(to), subject, body)

    def send_email(self):
        server_ssl = smtplib.SMTP_SSL('smtp.gmail.com', 465)
        server_ssl.ehlo()
        server_ssl.login(self.secrets_config.gmail_user, self.secrets_config.gmail_password)
        server_ssl.sendmail(self.secrets_config.gmail_user, self.to, self.email_text)
        server_ssl.close()

        SMTP_SERVER = "imap.gmail.com"
        SMTP_PORT = 993

        mail = imaplib.IMAP4_SSL(SMTP_SERVER, SMTP_PORT)
        mail.login(self.secrets_config.gmail_user, self.secrets_config.gmail_password)
        mail.select('inbox')

        print("Email sent!")
