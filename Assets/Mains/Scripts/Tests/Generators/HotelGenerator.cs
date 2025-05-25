using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace YNL.Checkotel
{
    public class HotelGenerator : MonoBehaviour
    {
        [SerializeField] private string _url;
        [SerializeField] private DatabaseContainerSO _database;

        [Button]
        public async void Generate()
        {
            var content = await _url.GetRawDatabaseAsync();

            _database.Hotels.Clear();

            string[] lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            for (int l = 1; l < lines.Length; l++)
            {
                _database.SerializeHotelDatabase(lines[l].SplitCSV());
            }
        }
    }
}