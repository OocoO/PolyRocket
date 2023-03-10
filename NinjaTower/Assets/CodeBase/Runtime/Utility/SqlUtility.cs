using System.Data.SQLite;

namespace Carotaa.Code
{
    public static class SqlUtility
    {
        private const string FileName = "/GameData.db";

        private static string _fileName;

        public static void OnRuntimeInit()
        {
            _fileName = FileUtility.PersistentDataPath + FileName;
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_fileName);
        }
    }
}