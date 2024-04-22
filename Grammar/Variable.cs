using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    internal class Variable
    {
        public Variable(MyType type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public MyType type { get; set; }
        public Object value { get; set; }
    }
}
