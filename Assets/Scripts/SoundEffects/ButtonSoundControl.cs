using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace SoundEffects
{
    public class ButtonSoundControl : SoundControl
    {
        [SerializeField] private List<Button> _buttons = new();

        private  void Start()
        {
            foreach (var button in _buttons)
            {
                button.onClick.AddListener(() => PlaySound(SoundEffectType.SelectButton));
            }
        }
    }
}