import os


class FilesFinder:
    def find_files(self, path: str):
        files = []
        for r, d, f in os.walk(path):
            for file in f:
                files.append(os.path.join(r, file))

        return files
