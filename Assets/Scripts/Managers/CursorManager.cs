using UnityEngine;

namespace Managers
{
    public class CursorManager : MonoBehaviour
    {
        [SerializeField] private Texture2D cursorTexture;

        void Start()
        {
            Cursor.SetCursor(cursorTexture, Vector3.zero, CursorMode.ForceSoftware);
        }
    }
}