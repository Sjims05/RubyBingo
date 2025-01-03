using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubyBingoApp.MVVM.ViewModel
{
    public class SavedBoardWidget
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string DateTime { get; set; }
        public string Progress { get; set; }
        public List<int> BoardHits { get; set; }
        public string FilePath { get; set; }

        public SavedBoardWidget()
        {

        }
    }
}
