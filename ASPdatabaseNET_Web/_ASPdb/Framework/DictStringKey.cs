using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdb.Framework
{
    //----------------------------------------------------------------------------------------------------////
    public class DictStringKey<V> where V : class
    {
        public Dictionary<string, V> TheDictionary;

        //----------------------------------------------------------------------------------------------------
        public DictStringKey()
        {
            this.TheDictionary = new Dictionary<string, V>();
        }


        //----------------------------------------------------------------------------------------------------
        public bool Insert(string key_AnyCase, V value)
        {
            string key = key_AnyCase.Trim().ToLower();

            if (!this.TheDictionary.ContainsKey(key))
            {
                this.TheDictionary.Add(key, value);
                return true;
            }
            else return false;
        }
        //----------------------------------------------------------------------------------------------------
        public bool Update(string key_AnyCase, V value)
        {
            string key = key_AnyCase.Trim().ToLower();

            if (this.TheDictionary.ContainsKey(key))
            {
                this.TheDictionary[key] = value;
                return true;
            }
            else return false;
        }
        //----------------------------------------------------------------------------------------------------
        public string InsertOrUpdate(string key_AnyCase, V value)
        {
            string key = key_AnyCase.Trim().ToLower();

            if (!this.TheDictionary.ContainsKey(key))
            {
                this.TheDictionary.Add(key, value);
                return "Insert";
            }
            else
            {
                this.TheDictionary[key] = value;
                return "Update";
            }
        }

        //----------------------------------------------------------------------------------------------------
        public bool ContainsKey(string key_AnyCase)
        {
            string key = key_AnyCase.Trim().ToLower();
            return this.TheDictionary.ContainsKey(key);
        }
        //----------------------------------------------------------------------------------------------------
        public bool DoesNot_ContainKey(string key_AnyCase)
        {
            string key = key_AnyCase.Trim().ToLower();
            return !this.TheDictionary.ContainsKey(key);
        }

        //----------------------------------------------------------------------------------------------------
        public V Get(string key_AnyCase)
        {
            string key = key_AnyCase.Trim().ToLower();
            if (this.TheDictionary.ContainsKey(key))
                return this.TheDictionary[key];
            else
                return null;
        }



    }
}