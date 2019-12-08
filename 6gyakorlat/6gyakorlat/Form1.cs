using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _6gyakorlat.Entities;

namespace _6gyakorlat
{
    public partial class Form1 : Form
    {
        List<Person> Population = new List<Person>();
        List<BirthProbability> BirthProbabilities = new List<BirthProbability>();
        List<DeathProbability> DeathProbabilities = new List<DeathProbability>();
        List<String> Eredmények = new List<String>();

        public Form1()
        {
            InitializeComponent();
        }

        public List<Person> GetPopulation(string csvpath)
        {
            List<Person> population = new List<Person>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    population.Add(new Person()
                    {
                        BirthYear = int.Parse(line[0]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
                        NbrOfChildren = int.Parse(line[2])
                    });
                }
            }

            return population;
        }

        public List<BirthProbability> GetBirthProbabilities(string csvpath)
        {
            List<BirthProbability> birthProbabilities = new List<BirthProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    birthProbabilities.Add(new BirthProbability()
                    {
                        Age = int.Parse(line[0]),
                        NbrOfChildren = int.Parse(line[1]),
                        P = double.Parse(line[2])
                    });
                }
            }

            return birthProbabilities;
        }

        public List<DeathProbability> GetDeathProbabilities(string csvpath)
        {
            List<DeathProbability> deathProbabilities = new List<DeathProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    deathProbabilities.Add(new DeathProbability()
                    {
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[0]),
                        Age = int.Parse(line[1]),
                        P = double.Parse(line[2])
                    });
                }
            }

            return deathProbabilities;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                Population = GetPopulation(textBox1.Text);
                BirthProbabilities = GetBirthProbabilities(@"C:\Temp\születés.csv");
                DeathProbabilities = GetDeathProbabilities(@"C:\Temp\halál.csv");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #region chunk
            Random rng = new Random();
            for (int year = 2005; year < numericUpDown1.Value; year++)
            {

                for (int i = 0; i < Population.Count; i++)
                {
                    Person person = Population[i];
                    if (!person.IsAlive) continue;
                    byte age = (byte)(year - person.BirthYear);


                    double pDeath = (from x in DeathProbabilities
                                     where x.Gender == person.Gender && x.Age == age + 1
                                     select x.P).First();
                    if (rng.NextDouble() <= pDeath) person.IsAlive = false;


                    if (person.Gender == Gender.Female)
                    {

                        double pBirth = (from x in BirthProbabilities
                                         where x.Age == age
                                         select x.P).FirstOrDefault();

                        if (rng.NextDouble() <= pBirth)
                        {
                            Person újszülött = new Person();
                            újszülött.BirthYear = year;
                            újszülött.NbrOfChildren = 0;
                            újszülött.Gender = (Gender)(rng.Next(1, 3));
                            Population.Add(újszülött);
                        }
                    }
                }
                int nbrOfMales = (from x in Population
                                  where x.Gender == Gender.Male && x.IsAlive
                                  select x).Count();
                int nbrOfFemales = (from x in Population
                                    where x.Gender == Gender.Female && x.IsAlive
                                    select x).Count();
                Eredmények.Add(
                    string.Format("Szimulációs év:{0} Fiuk:{1} Lányok:{2}", year, nbrOfMales, nbrOfFemales));
            }

            #endregion
            richTextBox1.Text = String.Join(Environment.NewLine, Eredmények);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            textBox1.Text = ofd.FileName;
        }
    }
}
