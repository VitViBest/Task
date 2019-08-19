using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpamLesson5Programm1
{
    class Program
    {
        public class Person
        {
            public string Name { get; set; }

            public Person(string name)
            {
                Name = name;
            }

            public virtual void GetInfo()
            {
                Console.WriteLine($"Человек: имя-{Name}");
            }
        }

        class Soccer:Person
        {

            public int Age { get; set; }

            public int Skill { get; set; }

            public Soccer(string name, int age, int skill):base(name)
            {
                Age = age;
                Skill = skill;
            }

            public override void GetInfo()
            {
                Console.WriteLine($"Игрок: имя-{Name}, возраст-{Age}, навык-{Skill}");
            }
        }

        class Team
        {
            public string Name { get; set; }

            public List<Soccer> Soccers { get; set; }

            public Coach Coach { get; set; }

            public double Skill { get
                {
                  return Soccers.Sum(s => s.Skill)*Coach.Fortyna;
                } }

            public Team(string name, Coach coach)
            {
                Name = name;
                Soccers = new List<Soccer>();
                Coach = coach;
            }

            public void Add(Soccer soccer)
            {
                Soccers.Add(soccer);
            }

            public List<Soccer> GetByName()
            {
                var result = new List<Soccer>();

                result = Soccers.OrderBy(s => s.Name).ToList();

                return result;
            }

            public List<Soccer> GetBySkill()
            {
                var result = new List<Soccer>();

                result = Soccers.Where(s=>s.Age>30).OrderByDescending(s=>s.Skill).ToList();

                return result;
            }
        }

        class Coach:Person
        {
            public double Fortyna { get; set; }

            public Coach(string name, double fortyna) : base(name)
            {
                Fortyna = fortyna;
            }

            public override void GetInfo()
            {
                Console.WriteLine($"Тренер: имя-{Name}, везение-{Fortyna}");
            }
        }

        class Referi:Person
        {
            public  enum Preferences
            {
                Neutral=0,
                ForFirst=1,
                ForSecond=2
            }

            public Preferences Preference { get; set; }

            public Referi(string name, Preferences preference) : base(name)
            {
                Preference = preference;
            }

            public void Fail(Team team,string fail)
            {
                Console.WriteLine($"Нарушение команда {team.Name}: {fail}");
            }

            public void Goal(Team team)
            {
                Console.WriteLine($"Команад {team.Name} забила гол");
            }

            public void Additional(int time)
            {
                Console.WriteLine($"Дополнительное время {time}");
            }

            public override void GetInfo()
            {
                Console.WriteLine($"Судья: имя-{Name}, подсуживание-{Preference}");
            }
        }

        class HistoryItem
        {
            public string Team1 { get; set; }
            public string Team2 { get; set; }
            public string Score { get; set; }

            public HistoryItem(string t1, string t2,string s)
            {
                Team1 = t1;
                Team2 = t2;
                Score = s;
            }

            public void GetInfo()
            {
                Console.WriteLine($"Матч {Team1} против {Team2} счет {Score};");
            }
        }

        class Game
        {
           public static List<HistoryItem> Items = new List<HistoryItem>();
            static Game()
            {
                try
                {
                    using (StreamReader sr = new StreamReader("Games.txt"))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null) {
                            string[] s = line.Split('|');
                            Items.Add(new HistoryItem(s[0], s[1], s[2]));
                        }
                    }
                }
                catch
                {
                    Items = new List<HistoryItem>();
                }
            }

            public static void GetGames()
            {
                foreach (var i in Items)
                {
                    i.GetInfo();
                }
            }

            public Team FirstTeam { get; set; }

            public Team SecondTeam { get; set; }

            public Referi Referi { get; set; }

            public delegate void Fail(Team team, string fail);

            public delegate void Goal(Team team);

            public delegate void Additional(int time);

            public event Fail FailHandler;

            public event Goal GoalHandler;

            public event Additional AdditionalHandler;

            public Game(Team team1,Team team2, Referi referi)
            {
                FirstTeam = team1;
                SecondTeam = team2;
                Referi = referi;
                FailHandler += referi.Fail;
                GoalHandler += referi.Goal;
                AdditionalHandler += referi.Additional;
            }

            private Random random = new Random();
            public void Start()
            {
                Console.WriteLine($"{FirstTeam.Name} против {SecondTeam.Name}");
                int t1 = 0;
                int t2 = 0;
                for (int i = 1; i < 30; i++)
                {
                    switch (random.Next(1, 10))
                    {
                        case 1:
                            if(Referi.Preference!=Referi.Preferences.ForFirst)
                                FailHandler.Invoke(FirstTeam, " игра рукой");
                            break;
                        case 2:
                            if (Referi.Preference != Referi.Preferences.ForSecond)
                                FailHandler.Invoke(SecondTeam, " игра рукой");
                            break;
                        case 3:
                            if (Winner() == SecondTeam.Name)
                            {
                                GoalHandler.Invoke(SecondTeam);
                                t2++;
                            }
                            else
                            {
                                if (Winner() == FirstTeam.Name)
                                {
                                    GoalHandler.Invoke(FirstTeam);
                                    t1++;
                                }
                                else
                                {
                                    if (random.Next(1, 100)%2==0)
                                    {
                                        GoalHandler.Invoke(SecondTeam);
                                        t2++;
                                    }
                                    else
                                    {
                                        GoalHandler.Invoke(FirstTeam);
                                        t1++;
                                    }
                                }
                            }     
                            break;
                        case 4:
                            try
                            {
                               switch (random.Next(1, 5))
                                {
                                    case 1:
                                        throw new GameExeption("На стадионе бомба", 12);
                                    case 2:
                                        throw new GameExeption("Зависла система", 3);
                                }
                            }
                            catch(GameExeption ex)
                            {
                                Console.WriteLine($"Чп: {ex.Message}, матч перенесен на {ex.Overtime} ч. Данные счета были потеряны");
                                t1 = 0;
                                t2 = 0;
                            }
                            break;
                    }

                }
                if (random.Next(0, 100) % 2 == 0)
                {
                    AdditionalHandler.Invoke(random.Next(1,11));
                }
                Console.WriteLine($"Матч завершился со счетом {t1}:{t2}");
                using (StreamWriter sr = new StreamWriter("Games.txt",true))
                {
                    sr.WriteLine($"{FirstTeam.Name}|{SecondTeam.Name}|{t1}:{t2}");
                }
            }

            class GameExeption : Exception
            {
                public int Overtime { get; set; }
                public GameExeption ( string message, int overtime) : base(message)
                {
                    Overtime = overtime;
                }
            }

            public string Winner()
            {
                if (Referi.Preference == Referi.Preferences.Neutral)
                {
                    double chanceFirst = FirstTeam.Skill;
                    double chanceSecond = SecondTeam.Skill;
                    string result = "ничья";
                    if ((Math.Max(chanceFirst, chanceSecond) - Math.Min(chanceFirst, chanceSecond)) * 100 / Math.Max(chanceFirst, chanceSecond) > 10)
                    {
                        if (chanceFirst > chanceSecond)
                            result = FirstTeam.Name;
                        else
                            result = SecondTeam.Name;
                    }
                    return result;
                }
                else
                {
                    if (Referi.Preference == Referi.Preferences.ForFirst)
                        return FirstTeam.Name;
                    else
                        return SecondTeam.Name;
                }
            }
        }

        static void Main(string[] args)
        {
            Random random = new Random();
            Team team1 = new Team("Шахтер", new Coach("Виталий", random.Next(5, 16) / 10.0));
            Team team2 = new Team("Днепр", new Coach("Славик", random.Next(5, 16) / 10.0));
            Referi.Preferences preferences = Referi.Preferences.Neutral;
            switch(random.Next(0, 3))
            {
                case 1: preferences = Referi.Preferences.ForFirst;
                    break;
                case 2:
                    preferences = Referi.Preferences.ForSecond;
                    break;
            }
            Referi referi = new Referi("Богдан", preferences);

            Console.WriteLine("Первая команда:"+team1.Name);
            team1.Coach.GetInfo();
            for (int i = 1; i <= 11; i++)
            {
                team1.Add(new Soccer("Игрок" + i.ToString(), random.Next(20, 40), random.Next(0, 101)));
                team1.Soccers.Last().GetInfo();
            }
            Console.WriteLine("Мастерство:" + team1.Skill);
            Console.WriteLine();
            Console.WriteLine("Вторая команда:" + team2.Name);
            team2.Coach.GetInfo();
            for (int i = 1; i <= 11; i++)
            {
                team2.Add(new Soccer("Игрок" + i.ToString(), random.Next(20, 40), random.Next(0, 101)));
                team2.Soccers.Last().GetInfo();
            }
            Console.WriteLine("Мастерство:" + team2.Skill);
            Console.WriteLine();
            Game game = new Game(team1, team2,referi);
            game.Referi.GetInfo();
            Console.WriteLine();
            Console.WriteLine("История:");
            Game.GetGames();

            Console.WriteLine();
            Console.WriteLine("Сортировка по алфавиту команды "+team1.Name);
            foreach(var i in team1.GetByName())
            {
                i.GetInfo();
            }
            Console.WriteLine();
            Console.WriteLine("Сортировка по алфавиту команды " + team2.Name);
            foreach (var i in team2.GetByName())
            {
                i.GetInfo();
            }
            Console.WriteLine();
            Console.WriteLine("Сортировка по мастерству команды " + team1.Name);
            foreach (var i in team1.GetBySkill())
            {
                i.GetInfo();
            }
            Console.WriteLine();
            Console.WriteLine("Сортировка по мастерству команды " + team2.Name);
            foreach (var i in team2.GetBySkill())
            {
                i.GetInfo();
            }

            Console.WriteLine();

            game.Start();
            Console.ReadKey();
        }
    }
}
