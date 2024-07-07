using System;
using System.Collections.Generic;
using UnityEngine;

namespace ED.Additional.Collections
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private TKey[] _keys;
        [SerializeField] private TValue[] _values;

        public SerializableDictionary() : base() { }

        public SerializableDictionary(IDictionary<TKey, TValue> dict) : base(dict.Count)
        {
            foreach (var p in dict) this[p.Key] = p.Value;
        }

        public void CopyFrom(IDictionary<TKey, TValue> dict)
        {
            Clear();

            foreach (var kvp in dict) this[kvp.Key] = kvp.Value;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_keys != null && _values != null && _keys.Length == _values.Length)
            {
                Clear();
                var n = _keys.Length;

                for (var i = 0; i < n; ++i) this[_keys[i]] = _values[i];

                _keys = null;
                _values = null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            var n = Count;
            _keys = new TKey[n];
            _values = new TValue[n];

            var i = 0;

            foreach (var kvp in this)
            {
                _keys[i] = kvp.Key;
                _values[i] = kvp.Value;
                ++i;
            }
        }
    }
}