using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuizGame
{
    public class Menu
    {
        public User CurrentUser;
        public AllQuizes MyAllQuizzez;
        public static string PathUsers;
        public Dictionary<string, string> Users;
        public static string PathStats;
        public Dictionary<KeyValuePair<string, string>, int> Statistics;

        static Menu()
        {
            PathUsers = "Users.txt";
            PathStats = "stats.txt";
        }

        public Menu()
        {
            MyAllQuizzez = new AllQuizes();
            Users = new Dictionary<string, string>();
            Statistics = new Dictionary<KeyValuePair<string, string>, int>();
            StreamReader reader = new StreamReader(PathUsers);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if(line == "")
                {
                    break;
                }
                string[] words = line.Split('-');
                Users.Add(words[0].Trim(), words[1].Trim());
            }
            reader.Close();

            reader = new StreamReader(PathStats);
            while ((line = reader.ReadLine()) != null)
            {
                
                string[] words = line.Split('-');
                Statistics.Add(new KeyValuePair<string, string>(words[0].Trim(), words[1].Trim()), Convert.ToInt32(words[2]));
            }
            reader.Close();
            ShowLoginOrRegister(); // внутри будет инициализирован MyAccount
        }
        public void ShowLoginOrRegister() // первый вызываемый метод меню.
        {
            char choice;
            do
            {
                Console.Clear();
                Console.WriteLine("Викторина");
                Console.WriteLine("\n1 - Регистрация нового пользователя");
                Console.WriteLine("2 - Вход в аккаунт");
                Console.WriteLine("\n3 - Выход из программы");
                choice = Console.ReadKey().KeyChar;
                switch (choice)
                {
                    case '1':
                        Register();
                        continue;
                    case '2':
                        Console.Clear();
                        Console.WriteLine("Введите ваш логин: ");
                        string login = Console.ReadLine();
                        if (EnterLogin(login) == false)
                        {
                            choice = 'q';
                            continue;
                        }
                        else
                        {
                            string pass = "";
                            pass = EnterPassword(pass);
                            if (IsTruePassword(login, pass) == false)
                            {
                                choice = 'q';
                                continue;
                            }
                            CurrentUser = new User(login, pass);
                        }
                        break;
                    case '3':
                        break;
                    default:
                        continue;
                }
            }
            while (choice != '1' && choice != '2' && choice != '3');

            if (choice != '3')
                ShowMainMenu();
            else
                Console.Clear();
        }
        public void ShowMainMenu()
        {
            char choice;
            do
            {
                Console.Clear();
                Console.WriteLine($"Вход выполнен, {CurrentUser.Login}!");
                Console.WriteLine("\n1 - Стартовать новую викторину");
                Console.WriteLine("2 - Посмотреть результаты своих прошлых викторин");
                Console.WriteLine("3 - Посмотреть Топ-20 по конкретной викторине");
                Console.WriteLine("\n4 - Поменять пароль");
                Console.WriteLine("5 - Изменить дату рождения");
                Console.WriteLine("\n6 - Выход из аккаунта");
                Console.WriteLine("7 - Выход из программы");
                choice = Console.ReadKey().KeyChar;
                switch (choice)
                {
                    case '1':
                        ShowMenuAllQuizzez();
                        continue;
                    case '2':
                        CurrentUser.ViewPastQuizzezResults(Statistics);
                        continue;
                    case '3':
                        ViewTop20();
                        continue;
                    case '4':
                        CurrentUser.ChangePassword();
                        continue;
                    case '5':
                        CurrentUser.ChangeDateTimeBirthDay();
                        continue;
                    case '6':
                        CurrentUser.Exit();
                        break;
                    case '7':
                        break;
                    default:
                        continue;
                }
            }
            while (choice != '7' && choice != '6');
            Console.Clear();
        }
        public void ShowMenuAllQuizzez()
        {
            int choiceQuiz;
            do
            {
                Console.Clear();
                Console.WriteLine("Выберите викторину для старта, нажав соответствующий ей номер: ");
                Console.WriteLine();
                List<int> quizIds = new List<int>();
                foreach (Quiz quiz in MyAllQuizzez.AllQuizzezList)
                {
                    Console.WriteLine($"{quiz.QuizId} - {quiz.QuizName}");
                    quizIds.Add(Convert.ToInt32(quiz.QuizId));
                }
                Console.WriteLine("\nДля старта викторины со случайными вопросами из всех викторин введите слово \"микс\".");
                Console.Write("\nВвод: ");
                try
                {
                    string choice = Console.ReadLine();
                    if (choice == "микс")
                    {
                        choiceQuiz = -1;
                        break;
                    }
                    choiceQuiz = Convert.ToInt32(choice);
                }
                catch
                {
                    Console.WriteLine("\nНеверный ввод!");
                    Console.ReadKey();
                    continue;
                }
                if (quizIds.Contains(choiceQuiz) == false)
                {
                    Console.WriteLine("\nНеверный ввод!");
                    Console.ReadKey();
                    continue;
                }
                break;
            }
            while (true);

            if (choiceQuiz == -1)
                CurrentUser.StartMixedQuiz(MyAllQuizzez, Statistics);
            else
                CurrentUser.StartQuiz(choiceQuiz, MyAllQuizzez, Statistics);
        }
        public void Register()
        {
            string login;
            do
            {
                Console.Clear();
                Console.WriteLine("Введите логин: ");
                login = Console.ReadLine();
                if (Users.ContainsKey(login))
                {
                    Console.Clear();
                    Console.WriteLine("Данный логин уже зарегистрирован!");
                    Console.ReadKey();
                    continue;
                }
                break;
            }
            while (true);

            string password = "";
            password = EnterPassword(password);
            Users.Add(login, password);

            DateTime dateTime = new DateTime();
            dateTime = EnterBirthDay(dateTime);
            SaveNewAccInFile(login,password,dateTime);

            CurrentUser = new User(login, password);

            Console.Clear();
            Console.WriteLine("Регистрация прошла успешно!");
            Console.ReadKey();
        }

        public bool EnterLogin(string login)
        {
            do
            {
                if (!Users.ContainsKey(login))
                {
                    Console.Clear();
                    Console.WriteLine("Данный логин не зарегистрирован!");
                    Console.ReadKey();
                    return false;
                }
                break;
            }
            while (true);
            return true;
        }

        public static string EnterPassword(string pass)
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Введите пароль цифрами от 0 до 9: ");
                pass = Console.ReadLine();
                break;
            }
            while (true);
            return pass;
        }

        public static DateTime EnterBirthDay(DateTime dateTime)
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Введите Вашу дату рождения в формате дд.мм.гггг : ");
                try
                {
                    dateTime = Convert.ToDateTime(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Неправильный формат!");
                    Console.ReadKey();
                    continue;
                }
                break;
            }
            while (true);
            return dateTime;
        }

        public void SaveNewAccInFile(string login, string pass,DateTime dateTime)
        {
            StreamWriter writer = new StreamWriter(PathUsers, true);
            writer.WriteLine($"{login} - {pass} - {dateTime.Date.ToString()}");
            writer.Close();
        }

        public bool IsTruePassword(string login, string pass)
        {
            if (Users[login] == pass)
                return true;
            else
            {
                Console.Clear();
                Console.WriteLine("Введен неверный пароль!");
                Console.ReadKey();
                return false;
            }
        }

        public static void SaveStatisticsInFile(Dictionary<KeyValuePair<string, string>, int> statistics)
        {
            StreamWriter writer = new StreamWriter(PathStats);
            
            foreach (var item in statistics)
            {
                writer.WriteLine($"{item.Key.Key} - {item.Key.Value} - {item.Value}");
            }

            writer.Close();
        }

        public void ViewTop20()
        {
            int choiceQuiz;
            string quizNameToView;
            do
            {
                Console.Clear();
                Console.WriteLine("Выберите викторину для просмотра ТОП-20 лучших прошедших ее пользователей: \n");
                List<int> quizIds = new List<int>();
                foreach (Quiz quiz in MyAllQuizzez.AllQuizzezList)
                {
                    Console.WriteLine($"{quiz.QuizId} - {quiz.QuizName}");
                    quizIds.Add(Convert.ToInt32(quiz.QuizId));
                }
                Console.WriteLine("\nДля просмотра ТОП-20 смешанных викторин введите слово \"микс\".");
                Console.Write("\nВвод: ");
                try
                {
                    string choice = Console.ReadLine();
                    if (choice == "микс")
                    {
                        quizNameToView = "Смешанная викторина";
                        break;
                    }
                    choiceQuiz = Convert.ToInt32(choice);
                }
                catch
                {
                    Console.WriteLine("\nНеверный ввод!");
                    Console.ReadKey();
                    continue;
                }
                if (quizIds.Contains(choiceQuiz) == false)
                {
                    Console.WriteLine("\nНеверный ввод!");
                    Console.ReadKey();
                    continue;
                }
                quizNameToView = MyAllQuizzez.AllQuizzezList[choiceQuiz].QuizName;
                break;
            }
            while (true);

            var statsToShow = from x in Statistics
                              where x.Key.Key.Contains(quizNameToView)
                              orderby x.Value descending
                              select x;
            if (statsToShow == null || statsToShow.Count() == 0)
            {
                Console.Clear();
                Console.WriteLine($"Викторину \"{quizNameToView}\" еще никто не проходил!");
                Console.ReadKey();
            }
            else
            {
                var results = statsToShow.Take(20);
                Console.Clear();
                Console.WriteLine($"ТОП-20 участников викторины \"{quizNameToView}\":");
                Console.WriteLine();
                int i = 1;
                foreach (var item in results)
                    Console.WriteLine($"{i++} место: {item.Key.Value} ({item.Value} правильных ответов).");
                Console.ReadKey();
            }
        }
    }
}
