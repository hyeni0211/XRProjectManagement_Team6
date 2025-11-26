using System;
using TwentyOz.VivenSDK.Scripts.Editor.Util;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.VMap
{
    [Serializable]
    public class VivenMapObject
    {
        [SerializeField] private SerializableGuid guid;
        [SerializeField] private GameObject prefab;
        
        private Twoz.Viven.Interactions.VObject vObject;

        public string Key
        {
            get => guid.Get().ToString();
            set
            {
                Guid.TryParse(value, out var result);
                if (result == Guid.Empty)
                {
                    Debug.LogError("Invalid Guid");
                    return;
                }
                guid = new SerializableGuid(result);
            }
        }

        public Twoz.Viven.Interactions.VObject VObject => vObject;

        public GameObject Prefab
        {
            get => prefab;
            set
            {
                var vobject = prefab.GetComponent<Twoz.Viven.Interactions.VObject>();
                if (!vobject)
                {
                    Debug.LogError("Prefab must have VObject component");
                    return;
                }
                prefab = value;
                vObject = vobject;
            }
        }
    }
}