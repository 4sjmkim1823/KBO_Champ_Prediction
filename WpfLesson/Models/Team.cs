using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLesson.Models
{
    public class Team
    {
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public int PredictedYear { get; set; }
        public List<int> HistoricalChampionships { get; set; } = new List<int>();
        public int Wins { get; set; }
        public int Losses { get; set; }
        public double WinRate 
        {
            get
            {
                return Wins > 0 ? (double)Wins / 144.0 : 0.0;
            }
            
        }
    }
}
