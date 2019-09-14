from helpers.files_finder import FilesFinder
from helpers.gmail import Gmail
import locale

locale.setlocale(locale.LC_ALL, '')
files_finder = FilesFinder()
files = files_finder.find_files("C:\\Users\\arkad\\Arek\\Dropbox\\Faktury")
print(files)
gmail = Gmail()
gmail.send_email(files)
