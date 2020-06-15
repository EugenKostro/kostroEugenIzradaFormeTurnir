using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnirAsistentModel.Models;

// * load the text file
// * convert the text to List<NagradaModel>
// find the max ID
// Add the new record with the new ID (max + 1)
// Convert the prizes to list<string>
// Save the list<string> to the text file

namespace TurnirAsistentModel.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        public static string FullFilePath(this string fileName) 
        {
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{fileName }";
        }

        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file) == false)
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }
        public static List<NagradaModel> ConvertToNagradaModels(this List<string> lines)
        {
            List<NagradaModel> output = new List<NagradaModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                NagradaModel p = new NagradaModel();
                p.ID = int.Parse(cols[0]);
                p.Mjesto = int.Parse(cols[1]);
                p.NazivMjesta = cols[2];
                p.IznosNagrade = decimal.Parse(cols[3]);
                p.PostotakNagrade = double.Parse(cols[4]);
                output.Add(p);
            }

            return output;

            
        }

        public static List<OsobaModel> ConvertToOsobaModels(this List<string> lines)
        {
            List<OsobaModel> output = new List<OsobaModel>();

            foreach(string line in lines)
            {
                string[] cols = line.Split(',');

                OsobaModel p = new OsobaModel();
                p.ID = int.Parse(cols[0]);
                p.Ime = cols[1];
                p.Prezime = cols[2];
                p.EmailAdresa = cols[3];
                p.BrojMobitela = cols[4];
                output.Add(p);

            }
            return output;

        }
        public static List<EkipaModel> ConvertToEkipaModels(this List<string> lines, string osobaFileName)
        {
            //id, ime ekipe, list of ids seperated by the pipe
            //3, Tim's Team,1|3|5
            List<EkipaModel> output = new List<EkipaModel>();
            List<OsobaModel> osoba = osobaFileName.FullFilePath().LoadFile().ConvertToOsobaModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                EkipaModel t = new EkipaModel();
                t.ID = int.Parse(cols[0]);
                t.ImeEkipe = cols[1];

                string[] osobaIDs = cols[2].Split('|');

                foreach (string ID in osobaIDs)
                {                                                       
                    t.ClanoviEkipe.Add(osoba.Where(x => x.ID == int.Parse(ID)).First());
                }
                return output;
            }
        }

        public static void SaveToPrizeFile(this List<NagradaModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (NagradaModel p in models)
            {
                lines.Add($"{ p.ID },{ p.Mjesto },{ p.NazivMjesta },{ p.IznosNagrade },{ p.PostotakNagrade }");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        public static void SaveToOsobaFile(this List<OsobaModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach(OsobaModel p in models)
            {
                lines.Add($"{p.ID },{ p.Ime },{ p.Prezime },{ p.EmailAdresa },{p.BrojMobitela }");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToEkipaFile(this List<EkipaModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (EkipaModel t in models)
            {
                lines.Add($"{ t.ID },{ t.ImeEkipe },{ ConvertOsobaListToString(t.ClanoviEkipe) }");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        private static string ConvertOsobaListToString(List<OsobaModel> osobe)
        {
            string output = "";

            if (osobe.Count == 0)
            {
                return "";
            }
            
            foreach (OsobaModel p in osobe)
            {
                output += $"{ p.ID }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }
    }
}
