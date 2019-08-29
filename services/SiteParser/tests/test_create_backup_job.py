import unittest
import unittest.mock
from create_backup_job.create_backup_job import CreateBackupJob
from create_backup_job.dropbox_sender import DropboxSender


class TestSum(unittest.TestCase):
    def test_list_int(self):
        zip_backup_creator_mock = unittest.mock.Mock()
        dropbox_sender_mock: DropboxSender = unittest.mock.Mock()
        sut = CreateBackupJob(backup_creator=zip_backup_creator_mock,
                              dropbox_sender=dropbox_sender_mock,
                              logger=unittest.mock.Mock())

        sut.execute()

        zip_backup_creator_mock.create_backup.assert_called_once()
        dropbox_sender_mock.backup_to_dropbox.assert_called_once()


if __name__ == '__main__':
    unittest.main()
