using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WeHearIt
{
    public class ExistReaderWriter
    {
        public string FileName { get; set; } = "exist.txt";

        private List<string> _db;

        public ExistReaderWriter()
        {
            try
            {
                _db = File.ReadAllLines(this.FileName).ToList();
            }
            catch
            {
                _db = new List<string>();
            }
            
        }
        public int GetCount()
        {
            return this._db.Count();
        }
        public bool HaveThis(string @findMe)
        {
            return this._db.Contains(findMe);
        }
        public bool Add(string @newValue,bool save=true)
        {
            try
            {
                this._db.Add(newValue);
                if(save)
                this.Save();

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public void Save()
        {
            File.WriteAllLines(this.FileName, _db);
        }
    }
}
