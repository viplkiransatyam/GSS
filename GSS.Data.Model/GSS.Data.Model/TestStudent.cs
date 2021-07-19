using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class TestStudent
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public List<Marks> Marks { get; set; }
    }

    public class Marks
    {
        public string Subject { get; set; }
        public int Mark { get; set; }
    }
}
