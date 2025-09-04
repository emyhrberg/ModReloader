using System;
using Terraria;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace ModReloader.Common.Configs.ConfigElements
{
    public abstract class IntPathOptionElement : RangeElement
    {
        public override int NumberTicks
        {
            get => GetCount();
        }

        public override float TickIncrement
        {
            get
            {
                int count = NumberTicks;
                if (count <= 1) return 1f;
                return 1f / (count - 1);
            }
        }

        protected override float Proportion
        {
            get
            {
                int count = NumberTicks;
                int max = count - 1;
                if (max <= 0) return 0f;
                int index = ReadIndex();
                if (index < 0) index = 0;
                if (index > max) index = max;
                return (float)index / max;
            }
            set
            {
                int count = NumberTicks;
                int max = count - 1;
                if (max < 0) max = 0;
                int newIndex = (int)Math.Round(value * Math.Max(1, max));
                if (newIndex < 0) newIndex = 0;
                if (newIndex > max) newIndex = max;
                WriteIndex(newIndex);
                Interface.modConfig.SetPendingChanges();
            }
        }

        public override void OnBind()
        {
            base.OnBind();
            TextDisplayFunction = () =>
            {
                string header = Label ?? MemberInfo.Name;
                //return header + ": " + ResolveName(ReadIndex());
                return header;
            };
        }

        protected abstract int GetCount();

        protected abstract string ResolveName(int index);

        protected abstract string IDToPath(int index);

        protected abstract int PathToID(string path);

        private int ReadIndex()
        {
            object raw = MemberInfo.GetValue(Item);
            int index = 0;
            if (raw is string path) index = PathToID(path);
            if (index >= 0) return index;
            return 0;
        }

        private void WriteIndex(int i)
        {
            if (!MemberInfo.CanWrite) return;
            MemberInfo.SetValue(Item, IDToPath(i));
        }
    }
}
