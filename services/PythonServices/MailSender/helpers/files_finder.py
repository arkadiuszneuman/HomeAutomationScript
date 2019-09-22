import os
from configs.cache import Cache


class FilesFinder:
    def find_files(self, path: str):
        sent_files = Cache().get_send_files()
        files = []
        for r, d, f in os.walk(path):
            for file in f:
                full_path = os.path.join(r, file)
                if full_path not in sent_files:
                    files.append(full_path)

        return files
