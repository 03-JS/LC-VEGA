using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LC_VEGA
{
    internal class PlayerInput : LcInputActions
    {
        [InputAction(KeyboardControl.X, Name = "Manual Listening")]
        public InputAction Activation { get; set; }
    }
}
