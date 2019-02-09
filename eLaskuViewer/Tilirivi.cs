using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace eLaskuViewer
{
    class Tilirivi
    {
        public DateTime SuoritePaiva;
        public float Summa;
        public string Selite;
        public string Arkistotunnus;
        public string Saaja;
        public string SaajanTilinumero;
        public string MaksupalveluID;

        public static List<Tilirivi> ReadFile(string path)
        {
            List<Tilirivi> retval = new List<Tilirivi>();

            string[] lines = File.ReadAllLines(path, Encoding.GetEncoding(1252));

            int line;

            for (line = 0; line < lines.Length; line++)
            {
                DateTime dt;

                string[] tok = lines[line].Split(";".ToArray());

                if (DateTime.TryParse(tok[0], out dt))
                    break;
            }
            while (line < lines.Length - 6)
            {
                string[] tok1 = lines[line].Split(";".ToArray());
                string[] tok2 = lines[line+1].Split(";".ToArray());
                string[] tok3 = lines[line+2].Split(";".ToArray());
                string[] tok4 = lines[line+3].Split(";".ToArray());
                string[] tok5 = lines[line+4].Split(";".ToArray());
                string[] tok6 = lines[line+5].Split(";".ToArray());
                string[] tok7 = lines[line+6].Split(";".ToArray());

                if(tok1.Length==1)
                {
                    line++;
                    continue;
                }
                Tilirivi t = new Tilirivi();
                t.SuoritePaiva = DateTime.Parse(tok1[0]);
                t.Summa = float.Parse(tok1[2]);
                t.Selite = tok1[4];
                t.Arkistotunnus = tok1[6];
                t.Saaja = tok2[4];
                t.SaajanTilinumero = tok2[6];

                t.MaksupalveluID = t.Selite.Contains("\"MAKSUPALVELU\"") ? t.Selite.Substring(14).Trim() : null;

                line += 7;
                retval.Add(t);
            }

            return (retval);
        }
    }
}
