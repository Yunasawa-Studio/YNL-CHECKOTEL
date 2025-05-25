using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine;

namespace YNL.Checkotel
{
    public static partial class Function
    {
        public static async UniTask<Texture2D[]> GetRoomImageAsync(this HotelUnit unit)
        {
            var urls = unit.Rooms?
                .Where(i => Main.Database.Rooms[i].Description != null && !string.IsNullOrEmpty(Main.Database.Rooms[i].Description.ImageURL))
                .Select(i => Main.Database.Rooms[i].Description.ImageURL).ToArray();
            var tasks = new List<UniTask<Texture2D>>();

            foreach (string url in urls)
            {
                tasks.Add(DownloadImageAsync(url));
            }

            return await UniTask.WhenAll(tasks);

            async UniTask<Texture2D> DownloadImageAsync(string url)
            {
                using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
                {
                    await uwr.SendWebRequest();

                    if (uwr.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"Failed to download image: {url}, Error: {uwr.error}");
                        return null;
                    }

                    return DownloadHandlerTexture.GetContent(uwr);
                }
            }
        }

        public static async UniTask<string> GetRawDatabaseAsync(this string url)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }
        }
    }
}