using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;

namespace TempAgain{
    class CountryData{
        public string country_code; public float value; public string country_name; public string indicatorname; public string year;
    }
    class Program {
        static void Main(string[] args){
            FileStream f = new FileStream(@"C:\Users\Training\Desktop\csv\Indicators.csv", FileMode.Open, FileAccess.Read);
            StreamReader read = new StreamReader(f);
            FileStream fs = new FileStream(@"C:\Users\Training\Desktop\csv\final1.json", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter write = new StreamWriter(fs);
            FileStream fs1 = new FileStream(@"C:\Users\Training\Desktop\csv\final2.json", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter write1 = new StreamWriter(fs1);
            FileStream fs2 = new FileStream(@"C:\Users\Training\Desktop\csv\final3.json", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter write2 = new StreamWriter(fs2);
            string[] country = { "AFG", "ARM", "AZE", "BHR", "BGD", "BTN", "BRN", "KHM", "CHN", "CXR", "CCK", "IOT", "GEO", "HKG", "IND", "IDN", "IRN", "IRQ", "ISR", "JPN", "JOR", "KAZ", "KWT", "KGZ", "LAO", "LBN", "MAC", "MYS", "MDV", "MNG", "MMR", "NPL", "PRK", "OMN", "PAK", "PHL", "QAT", "SAU", "SGP", "KOR", "LKA", "SYR", "TWN", "TJK", "THA", "TUR", "TKM", "ARE", "UZB", "VNM", "YEM", "SAS", "EAS" };
            List<CountryData> list = new List<CountryData>();
            var str = read.ReadLine();
            string[] words = str.Split(',');
            Stopwatch stopWatch = new Stopwatch();
            StreamReader read3 = new StreamReader(f);
            var str3 = read.ReadLine();
            string space;
            write1.WriteLine("{"); write1.WriteLine("\"India\": [");
            float valueForAvg = 0;
            float valueForFemaleAvg = 0;
            Dictionary<string, float> AvgForMale = new Dictionary<string, float>();
            Dictionary<string, float> AvgForFemale = new Dictionary<string, float>();
            while ((space = read.ReadLine()) != null){
                stopWatch.Start();
                string[] words3 = space.Split(',');
                for (int i = 0; i < words3.Length; i++){
                    if (words3[i].StartsWith("\"")){
                        if (words3[i].EndsWith("\"")) { }
                        else {
                            words3[i] = words3[i] + words3[i + 1];
                            words3 = words3.Where((value, idx) => idx != (i + 1)).ToArray();
                        }
                    }
                }
                string[] words4 = words3;
                foreach (var i in country){
                    if (words3[1] == i) {
                        if (words3[3] == "SP.DYN.LE00.FE.IN"){
                            valueForAvg += float.Parse(words3[5]);
                            if (!AvgForMale.ContainsKey(words3[1])){
                                AvgForMale.Add(words3[1], float.Parse(words3[5]));
                            }
                            else {
                                AvgForMale[words3[1]] += float.Parse(words3[5]);
                            }
                        }
                        else if (words3[3] == "SP.DYN.LE00.MA.IN"){
                            valueForFemaleAvg += float.Parse(words3[5]);
                            if (!AvgForFemale.ContainsKey(words3[1])){
                                AvgForFemale.Add(words3[1], float.Parse(words3[5]));
                            }
                            else{
                                AvgForFemale[words3[1]] += float.Parse(words3[5]);
                            }
                        }
                    }
                }
                if (words4[1] == "IND"){
                    if (words4[4] == "SP.DYN.CBRT.IN"){
                        write1.WriteLine("{");
                        write1.WriteLine("\"" + "Year" + "\"" + ":" + int.Parse(words4[5]) + ",");
                        write1.WriteLine("\"" + "Birth_Rate" + "\"" + ":" + "\"" + words4[6] + "\"" + " ,");
                        write1.Flush();
                    }
                    else if (words4[4] == "SP.DYN.CDRT.IN"){

                        write1.WriteLine("\"" + "Death_Rate" + "\"" + ":" + "\"" + words4[6] + "\"");
                        if (words3[5] == "2013" && words3[6] == "7.385") write1.WriteLine("}");
                        else write1.WriteLine("},");
                        write1.Flush();
                    }
                }
                if (words3[3] == "SP.DYN.LE00.IN") {
                    list.Add(new CountryData() { country_code = words3[1], value = float.Parse(words3[5]), country_name = words3[0] });
                }
            }
            var res = from temp1 in list group temp1 by temp1.country_name into t select new { countryname = t.Key, value = t.Sum(o => o.value) };
            var k = res.OrderByDescending(m => m.value).Take(5);
            write2.WriteLine("[");
            foreach (var i in k){
                if (i.countryname == "Norway"){
                    write2.WriteLine("{" + "\"" + "Country Name" + "\"" + ":" + "\"" + i.countryname + "\"" + "," + "\n" + "\"" + "Values" + "\"" + ":" + i.value + "\n" + "}");
                }
                else{
                    write2.WriteLine("{" + "\"" + "Country Name" + "\"" + ":" + "\"" + i.countryname + "\"" + "," + "\n" + "\"" + "Values" + "\"" + ":" + i.value + "\n" + "}" + ",");
                }
            }
            write1.WriteLine("]"); write1.Flush(); write1.WriteLine("}"); write1.Flush();
            write2.WriteLine("]"); write2.Flush();
            stopWatch.Stop();
            Console.Write(stopWatch.ElapsedMilliseconds);
            int count = 0;
            write.WriteLine("[");
            foreach (KeyValuePair<string, float> entry in AvgForMale){
                write.WriteLine("{");
                write.WriteLine("\"" + "Country" + "\"" + ":" + entry.Key + ",");
                write.WriteLine("\"" + "Average_For_Male" + "\"" + ":" + entry.Value + ",");
                write.WriteLine("\"" + "Average_For_Female" + "\"" + ":" + AvgForFemale[entry.Key]);
                count++;
                write.WriteLine("}");
                if (count != AvgForMale.Count) {
                    write.WriteLine(",");
                }
            }
            write.WriteLine("]");
            write.Flush();
        }
    }
}
