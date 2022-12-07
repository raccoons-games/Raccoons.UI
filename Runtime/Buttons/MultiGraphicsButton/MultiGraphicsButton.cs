using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raccoons.UI.Buttons
{
    [RequireComponent(typeof(MultiTargetGraphics))]
    public class MultiGraphicsButton : Button
    {
        public MultiTargetGraphics _subTargetGraphics;
        protected override void Awake()
        {
            base.Awake();
            _subTargetGraphics = GetComponent<MultiTargetGraphics>();
        }
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            //get the graphics, if it could not get the graphics, return here
            base.DoStateTransition(state, instant);
            if (transition == Transition.ColorTint)
            {
                var targetColor =
                    state == SelectionState.Disabled ? colors.disabledColor :
                    state == SelectionState.Highlighted ? colors.highlightedColor :
                    state == SelectionState.Normal ? colors.normalColor :
                    state == SelectionState.Pressed ? colors.pressedColor :
                    state == SelectionState.Selected ? colors.selectedColor : Color.white;

                _subTargetGraphics.TargetGraphics?.ForEach(x => x.CrossFadeColor(targetColor, instant ? 0 : colors.fadeDuration, true, true));
            }
        }
    }
}
