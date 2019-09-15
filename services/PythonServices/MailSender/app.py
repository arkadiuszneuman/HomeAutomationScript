from helpers.files_finder import FilesFinder
from helpers.gmail import Gmail
import locale
from helpers.groups_creator import GroupsCreator

locale.setlocale(locale.LC_ALL, '')
files_finder = FilesFinder()
files = files_finder.find_files("C:\\Users\\arkad\\Arek\\Dropbox\\Faktury")

groups_creator = GroupsCreator()
groups = groups_creator.get_groups(files)

gmail = Gmail()
gmail.send_emails(groups)
