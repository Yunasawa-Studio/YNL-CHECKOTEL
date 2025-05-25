using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using YNL.Utilities.Extensions;
namespace YNL.Checkotel
{
    public class DataManager : MonoBehaviour
    {
        public static string HotelImageURL;
        public static string RoomImageURL;

        public bool EnableInitializeDatabase = true;

        private DatabaseContainerSO _database => Main.Database;

        [SerializeField] private string _hotelDatabaseURL;
        [SerializeField] private string _roomDatabaseURL;
        [SerializeField] private string _feedbackDatabaseURL;
        [SerializeField] private string _configDatabaseURL;

        private string _savingPath => $"{Application.persistentDataPath}/Save.ysf";

        private void Awake()
        {
            Marker.OnRuntimeSavingRequested += SaveRuntimeData;
        }

        private void OnDestroy()
        {
            Marker.OnRuntimeSavingRequested -= SaveRuntimeData;
        }

        private void Start()
        {
            Main.Runtime.Reset();

            LoadSavedData();

            InitializeDatabases().Forget();
        }

        public void SaveRuntimeData()
        {
            SaveNewtonJson(Main.Runtime.Data, _savingPath);
        }

        private async UniTaskVoid InitializeDatabases()
        {
            if (EnableInitializeDatabase)
            {
                await InitializeConfigDatabase();
                await InitializeHotelDatabase();
                await InitializeRoomDatabase();
                await InitializeFeedbackDatabase();
            }

            await UniTask.Delay(100);

            InitializeBaseData();

            Marker.OnDatabaseSerializationDone?.Invoke();
            Main.IsSystemStarted = true;
        }

        public void LoadSavedData()
        {
            Main.Runtime.Data = MJson.LoadNewtonJson<RuntimeData>(_savingPath, null, ResaveData);

            if (Main.Runtime.Data == null) ResaveData(null);

            void ResaveData(string message)
            {
                Main.Runtime.Data = new();
                SaveNewtonJson(Main.Runtime.Data, _savingPath);
            }
        }

        private async UniTask InitializeConfigDatabase()
        {
            var content = await _configDatabaseURL.GetRawDatabaseAsync();

            string[] lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            HotelImageURL = lines[0];
            RoomImageURL = lines[1];
        }

        private async UniTask InitializeHotelDatabase()
        {
            var content = await _hotelDatabaseURL.GetRawDatabaseAsync();

            _database.Hotels.Clear();

            string[] lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            for (int l = 1; l < lines.Length; l++)
            {
                var fields = lines[l].SplitCSV();

                if (string.IsNullOrEmpty(fields[1])) break;

                _database.SerializeHotelDatabase(fields);
            }
        }

        private async UniTask InitializeRoomDatabase()
        {
            var content = await _roomDatabaseURL.GetRawDatabaseAsync();

            _database.Rooms.Clear();

            string[] lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            for (int l = 1; l < lines.Length; l++)
            {
                var fields = lines[l].SplitCSV();

                if (string.IsNullOrEmpty(fields[1])) break;

                _database.SerializeRoomDatabase(fields);
            }
        }

        private async UniTask InitializeFeedbackDatabase()
        {
            var content = await _feedbackDatabaseURL.GetRawDatabaseAsync();

            _database.Feedbacks.Clear();

            string[] lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            for (int l = 1; l < lines.Length; l++)
            {
                var fields = lines[l].SplitCSV();

                if (string.IsNullOrEmpty(fields[1])) break;

                _database.SerializeFeedbackDatabase(fields);
            }

            foreach (var feedback in Main.Database.Feedbacks)
            {
                MDebug.Log($"Feedback: {feedback.Value.Cleanliness.ToString()} - {feedback.Value.Facilities.ToString()} - {feedback.Value.Service.ToString()}: {feedback.Value.AverageRating.ToString()}");
            }
        }

        private void InitializeBaseData()
        {
            foreach (var id in _database.Hotels.Keys)
            {
                id.GetHighestPriceAllType();
                id.GetLowestPriceAllType();
            }
        }

        public static bool SaveNewtonJson<T>(T data, string path, Action saveDone = null)
        {
            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path)) { } // Properly closes the file
                Debug.LogWarning("Target json file doesn't exist! Created a new file.");
            }

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            // Open file with proper read/write permissions
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(json);
            }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            return true;
        }
    }
}