from helpers.gmail import Gmail
import locale
from helpers.groups_creator import GroupsCreator
from helpers.dropbox_files import DropboxFiles
from helpers.logger import Logger

locale.setlocale(locale.LC_ALL, '')

with DropboxFiles() as dropbox_files:
    downloaded_files = list(dropbox_files.download_files())
    if len(downloaded_files) == 0:
        Logger().get_logger().info("No files to send")
    else:
        groups_creator = GroupsCreator()
        groups = groups_creator.get_groups(downloaded_files)

        gmail = Gmail()
        gmail.send_emails(groups)
