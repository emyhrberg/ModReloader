using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace ModHelper.UI.SpawnerElements
{
    //TOsDO: wow that's a lot of redundant this.
    public class Grid : UIElement
    {
        public delegate bool ElementSearchMethod(UIElement element);

        private class UIInnerList : UIElement
        {
            public override bool ContainsPoint(Vector2 point)
            {
                return true;
            }

            protected override void DrawChildren(SpriteBatch spriteBatch)
            {
                Vector2 position = Parent.GetDimensions().Position();
                Vector2 dimensions = new Vector2(Parent.GetDimensions().Width, Parent.GetDimensions().Height);
                foreach (UIElement current in Elements)
                {
                    Vector2 position2 = current.GetDimensions().Position();
                    Vector2 dimensions2 = new Vector2(current.GetDimensions().Width, current.GetDimensions().Height);
                    if (Collision.CheckAABBvAABBCollision(position, dimensions, position2, dimensions2))
                    {
                        current.Draw(spriteBatch);
                    }
                }
            }
        }

        public List<UIElement> _items = new List<UIElement>();
        protected UIScrollbar _scrollbar;
        internal UIElement _innerList = new UIInnerList();
        private float _innerListHeight;
        public float ListPadding = 5f;

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        // ** added
        public Action<List<UIElement>> ManualSortMethod;

        // tosdo, vertical/horizontal orientation, left to right, etc?
        public Grid()
        {
            _innerList.OverflowHidden = false;
            _innerList.Width.Set(0f, 1f);
            _innerList.Height.Set(0f, 1f);
            OverflowHidden = true;
            Append(_innerList);
        }

        public float GetTotalHeight()
        {
            return _innerListHeight;
        }

        public void Goto(UIGrid.ElementSearchMethod searchMethod, bool center = false)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (searchMethod(_items[i]))
                {
                    _scrollbar.ViewPosition = _items[i].Top.Pixels;
                    if (center)
                    {
                        _scrollbar.ViewPosition = _items[i].Top.Pixels - GetInnerDimensions().Height / 2 + _items[i].GetOuterDimensions().Height / 2;
                    }
                    return;
                }
            }
        }

        public virtual void Add(UIElement item)
        {
            _items.Add(item);
            _innerList.Append(item);
            UpdateOrder();
            _innerList.Recalculate();
        }

        public virtual void AddRange(IEnumerable<UIElement> items)
        {
            _items.AddRange(items);
            foreach (var item in items)
            {
                _innerList.Append(item);
            }

            UpdateOrder();
            _innerList.Recalculate();
        }

        public virtual bool Remove(UIElement item)
        {
            _innerList.RemoveChild(item);
            UpdateOrder();
            return _items.Remove(item);
        }

        public virtual void Clear()
        {
            _innerList.RemoveAllChildren();
            _items.Clear();
        }

        public override void Recalculate()
        {
            base.Recalculate();
            UpdateScrollbar();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
                PlayerInput.LockVanillaMouseScroll("ModLoader/UIList");
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            if (_scrollbar != null)
            {
                _scrollbar.ViewPosition -= evt.ScrollWheelValue;
            }
        }

        public override void RecalculateChildren()
        {
            float availableWidth = GetInnerDimensions().Width;
            base.RecalculateChildren();
            float top = 0f;
            float left = 0f;
            float maxRowHeight = 0f;
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var outerDimensions = item.GetOuterDimensions();
                if (left + outerDimensions.Width > availableWidth && left > 0)
                {
                    top += maxRowHeight + ListPadding;
                    left = 0;
                    maxRowHeight = 0;
                }
                maxRowHeight = Math.Max(maxRowHeight, outerDimensions.Height);
                item.Left.Set(left, 0f);
                left += outerDimensions.Width + ListPadding;
                item.Top.Set(top, 0f);
            }
            _innerListHeight = top + maxRowHeight;
        }

        private void UpdateScrollbar()
        {
            if (_scrollbar == null)
            {
                return;
            }
            _scrollbar.SetView(GetInnerDimensions().Height, _innerListHeight);
        }

        public void SetScrollbar(UIScrollbar scrollbar)
        {
            _scrollbar = scrollbar;
            UpdateScrollbar();
        }

        public void UpdateOrder()
        {
            if (ManualSortMethod != null)
            {
                // Log.Info("Manual sort with " + _items.Count + " items");
                ManualSortMethod(_items);
            }
            else
            {
                _items.Sort(SortMethod);
            }
            UpdateScrollbar();
        }

        public int SortMethod(UIElement item1, UIElement item2)
        {
            return item1.CompareTo(item2);
        }

        public override List<SnapPoint> GetSnapPoints()
        {
            List<SnapPoint> list = new List<SnapPoint>();
            if (GetSnapPoint(out SnapPoint item))
            {
                list.Add(item);
            }
            foreach (UIElement current in _items)
            {
                list.AddRange(current.GetSnapPoints());
            }
            return list;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (_scrollbar != null)
            {
                _innerList.Top.Set(-_scrollbar.GetValue(), 0f);
            }
        }
    }

    class NestedUIGrid : UIGrid
    {
        public NestedUIGrid()
        {
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            if (_scrollbar != null)
            {
                float oldpos = _scrollbar.ViewPosition;
                _scrollbar.ViewPosition -= evt.ScrollWheelValue;
                if (oldpos == _scrollbar.ViewPosition)
                {
                    base.ScrollWheel(evt);
                }
            }
            else
            {
                base.ScrollWheel(evt);
            }
        }
    }
}
