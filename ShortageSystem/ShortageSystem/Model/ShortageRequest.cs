using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShortageSystem.Model.Enums;

namespace ShortageSystem.Model
{
    internal class ShortageRequest
    {
        private int _priority;
        public string Title { get; set; }
        public string Name { get; set; }
        public Room? Room { get; set; }
        public Category? Category { get; set; }

        public int Priority
        {
            get { return _priority;}
            set
            {
                if (value < 1 || value > 10)
                {
                    throw new ArgumentOutOfRangeException(value.ToString());
                }
                _priority = value;
            }
        }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
