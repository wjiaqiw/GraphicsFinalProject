using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace DynamicPortals
{
    [ExecuteInEditMode]
    public class PortalManager : MonoBehaviour
    {
        static PortalManager _instance;
        public static PortalManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    PortalManager portalManager = Resources.Load<PortalManager>("Prefabs/PortalManager");
                    if (portalManager != null)
                    {
                        Instantiate(portalManager).name = "PortalManager";
                        return portalManager;
                    }
                    else
                    {
                        Debug.LogError("No PortalManager found in the scene and automatic instantiation failed. Please add one manually");
                        EditorApplication.ExitPlaymode();
                    }
                }
                return _instance;
            }
        }

        [SerializeField] Player _player;
        public Player Player
        {
            get
            {
                if (_player == null) _player = FindObjectOfType<Player>();
                return _player;
            }
        }

        void Awake()
        {
            if (_instance != null && _instance != this) DestroyImmediate(gameObject);
            else _instance = this;
        }
    }
}
