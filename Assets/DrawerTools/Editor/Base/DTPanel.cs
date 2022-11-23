using System;
using UnityEngine;

namespace DrawerTools
{
    /// <summary>
    /// Base panel class
    /// Realize: some <see cref="IDTPanel"/> behaviours
    /// </summary>
    public abstract class DTPanel : DTDrawable, IDTPanel
    {
        public class DefaultPanelBehaviour<T> where T : DTDrawable, IDTPanel
        {
            private readonly T _target;

            protected readonly Scroll Scroll = new Scroll();

            public IDTPanel Parent { get; set; }
            public bool DrawScrollInExpand { get; set; } = false;
            public bool DrawName { get; set; } = true;


            public DefaultPanelBehaviour(T target, IDTPanel parent, Action atParentSizeChanged)
            {
                _target = target;
                _target.Sizer.ExpandsHeight = true;
                Parent = parent;
                Scroll.SetSizer(_target.Sizer);
                Parent.OnSizeChange += atParentSizeChanged;
                _target.OnBeforeDraw += BeforeDraw;
                _target.OnAfterDraw += AfterDraw;
            }

            public Vector2 GetFixedSize(float? x = null, float? y = null)
            {
                if (!x.HasValue && !_target.Sizer.ExpandsWidth)
                    x = _target.Sizer.Width;
                if (!y.HasValue && !_target.Sizer.ExpandsHeight)
                    y = _target.Sizer.Width;
                if (x.HasValue && y.HasValue)
                    return new Vector2(x.Value, y.Value);
                return Parent.GetFixedSize(x, y);
            }

            protected void BeforeDraw()
            {
                if (_target.Name != "" && DrawName)
                    DT.Label(_target.Name);
                if (!_target.Sizer.ExpandsHeight || DrawScrollInExpand)
                    Scroll.Begin();
            }

            protected void AfterDraw()
            {
                if (!_target.Sizer.ExpandsHeight || DrawScrollInExpand)
                    Scroll.End();
            }
        }

        protected readonly DefaultPanelBehaviour<DTPanel> panelBehaviour;

        public DTPanel(IDTPanel parent) : base()
        {
            panelBehaviour = new DefaultPanelBehaviour<DTPanel>(this, parent, AtSizeChanged);
        }

        public IDTPanel Parent => panelBehaviour.Parent;

        public Vector2 GetFixedSize(float? x = null, float? y = null) => panelBehaviour.GetFixedSize(x, y);

        public DTPanel SetExpandable(bool expandable)
        {
            panelBehaviour.DrawScrollInExpand = expandable;
            return this;
        }
    }
}