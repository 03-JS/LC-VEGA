using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace LC_VEGA
{
    internal class PlayerInput : LcInputActions
    {
        [InputAction("<Keyboard>/x", Name = "Manual Listening")]
        public InputAction Toggle { get; set; }
    }
}
