from helpers.files_finder import FilesFinder
from helpers.gmail import Gmail
import locale
from helpers.groups_creator import GroupsCreator
from helpers.dropbox_files import DropboxFiles
from configs.cache import Cache

locale.setlocale(locale.LC_ALL, '')
# cache = Cache()
# cache.add_saved_file("test.aa")
# cache.save()

with DropboxFiles() as dropbox_files:
    dropbox_files.download_files()

    files_finder = FilesFinder()
    files = files_finder.find_files("C:\\Users\\arkad\\Arek\\Dropbox\\Faktury")

    groups_creator = GroupsCreator()
    groups = groups_creator.get_groups(files)

    gmail = Gmail()
    gmail.send_emails(groups)
