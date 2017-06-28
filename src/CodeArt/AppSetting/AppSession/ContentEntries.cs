﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.AppSetting
{
    /// <summary>
    /// 回话内容条目
    /// </summary>
    public sealed class ContentEntries
    {
        public const string Name = "ContentEntries";

        private static Dictionary<string, object> _data = new Dictionary<string, object>();

        public bool Initialized
        {
            get;
            private set;
        }

        public void Initializ()
        {
            this.Initialized = true;
        }


        public ContentEntries()
        {

        }

        public object Get(string name)
        {
            if (_data.TryGetValue(name, out var value))
            {
                return value;
            }
            return null;
        }

        public void Set(string name, object value)
        {
            _data[name] = value;
        }

        public bool Contains(string name)
        {
            return _data.ContainsKey(name);
        }

        public void Clear()
        {
            _data.Clear();
            this.Initialized = false;
        }
    }
}
