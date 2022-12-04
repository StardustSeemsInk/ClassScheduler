using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskTopPlayer
{
    internal class Events
    {
        public delegate void OnSetRootPathHandler();

        public static event OnSetRootPathHandler? OnSetRootPath;

        public static void InitEvents()
        {
            OnSetRootPath += () => { };
        }

        public static void Invoke(string name)
        {
            switch (name)
            {
                case nameof(OnSetRootPath):
                    OnSetRootPath?.Invoke();
                    break;
            }
        }
    }
}
