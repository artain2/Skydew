using UnityEngine;
using UnityEngine.UI;

namespace _SkyDew.Scripts.Common
{
    public class UIGhost : MonoBehaviour
    {
        public Image iconImage;

        public Image IconImage => iconImage;
        public RectTransform IconRT => iconImage.transform as RectTransform;
    }
}