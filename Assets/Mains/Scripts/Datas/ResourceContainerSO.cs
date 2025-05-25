using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.Addons;

namespace YNL.Checkotel
{
    [CreateAssetMenu(fileName = "ResourceContainerSO", menuName = "YNL - Checkotel/ResourceContainerSO")]
    public class ResourceContainerSO : ScriptableObject
    {
        public SerializableDictionary<string, Texture2D> Icons = new();
        public SerializableDictionary<string, StyleSheet> Styles = new();
    }
}