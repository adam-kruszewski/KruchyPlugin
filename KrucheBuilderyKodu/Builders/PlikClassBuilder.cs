using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrucheBuilderyKodu.Builders
{
    public class PlikClassBuilder : ICodeBuilder
    {
        private const string jednostkaWciecia = StaleDlaKodu.JednostkaWciecia;
        private string NazwaRodzajuObiektu { get; set; }
        private string Namespace { get; set; }
        private IList<string> Usingi { get; set; }
        private string Nazwa { get; set; }
        private IList<ICodeBuilder> Konstruktory { get; set; }
        private IList<ICodeBuilder> Metody { get; set; }
        private IList<ICodeBuilder> AtrybutyKlasy { get; set; }
        private ICodeBuilder Obiekt { get; set; }

        public PlikClassBuilder()
        {
            ZNazwaRodzajuObiektu("class");
            Usingi = new List<string>();
            Konstruktory = new List<ICodeBuilder>();
            AtrybutyKlasy = new List<ICodeBuilder>();
        }

        public PlikClassBuilder ZNazwaRodzajuObiektu(string nazwa)
        {
            this.NazwaRodzajuObiektu = nazwa;
            return this;
        }

        public PlikClassBuilder WNamespace(string _namespace)
        {
            Namespace = _namespace;
            return this;
        }

        public PlikClassBuilder DodajUsing(string _using)
        {
            Usingi.Add(_using);
            return this;
        }

        public PlikClassBuilder ZNazwa(string nazwa)
        {
            Nazwa = nazwa;
            return this;
        }

        public PlikClassBuilder ZObiektem(ICodeBuilder klasaBuilder)
        {
            Obiekt = klasaBuilder;
            return this;
        }

        public PlikClassBuilder ZAtrybutemKlasy(string nazwa, params string[] parametry)
        {
            var atrybutBuilder = new AtrybutBuilder().ZNazwa(nazwa);
            foreach (var p in parametry)
                atrybutBuilder.DodajWartoscParametru(p);
            AtrybutyKlasy.Add(atrybutBuilder);
            return this;
        }

        public PlikClassBuilder DodajKonstruktor(ICodeBuilder builder)
        {
            Konstruktory.Add(builder);
            return this;
        }

        public PlikClassBuilder DodajMetode(ICodeBuilder metodaBuilder)
        {
            Metody.Add(metodaBuilder);
            return this;
        }

        public string Build(string wciecie = "")
        {
            var outputBuilder = new StringBuilder();

            //usingi
            var usingi = Usingi.OrderBy(o => o).ToList();
            foreach (var u in usingi)
                outputBuilder.AppendLine("using " + u + ";");
            //namespace
            outputBuilder.AppendLine();
            outputBuilder.AppendLine("namespace " + Namespace);
            outputBuilder.AppendLine("{");
            GenerujZawartoscNamespace(outputBuilder);
            outputBuilder.Append("}");
            return outputBuilder.ToString();
        }

        private void GenerujZawartoscNamespace(StringBuilder outputBuilder)
        {
            outputBuilder.Append(Obiekt.Build(StaleDlaKodu.WielokrotnoscWciecia(1)));

            ////atrybuty klasy
            //foreach (var a in AtrybutyKlasy)
            //    outputBuilder.Append(a.Build(jednostkaWciecia));
            ////foreach (var a in )
            ////klasa
            //var klasaBuilder = new ClassBuilder().ZNazwa(Nazwa);
            //outputBuilder.AppendLine(klasaBuilder.Build(jednostkaWciecia));
            //konstruktory

            //metody

        }
    }
}