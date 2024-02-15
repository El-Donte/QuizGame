using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuizGame
{
    public class User
    {
        public string Login;
        public string Password;
        public DateTime DateTimeBirthDay;

        public User(string login, string password)
        {
            Login = login;
            Password = password;
            ReadBirthDayFromFile();
        }

        public void ReadBirthDayFromFile()
        {
            StreamReader reader = new StreamReader(Menu.PathUsers);
            string line = reader.ReadLine();
            while((line = reader.ReadLine()) != null)
            {
                
                string[] words = line.Split('-');
                for(int i = 0; i < words.Length-1; i++)
                {
                    words[i] = words[i].Trim();
                }

                if (words.Contains(Login))
                {
                    DateTimeBirthDay = DateTime.Parse(words[2]);
                }
            }
            reader.Close();
        }

        public void ChangePassword()
        {
            string newPassword = "";
            newPassword = Menu.EnterPassword(newPassword);
            string oldPassword = Password;
            Password = newPassword;

            SaveAccauntDataInFile(oldPassword, DateTimeBirthDay);

            Console.Clear();
            Console.WriteLine("Пароль успешно обновлен!");
            Console.ReadKey();
        }

        public void ChangeDateTimeBirthDay()
        {
            DateTime newDateTimeBirthDay = new DateTime();
            newDateTimeBirthDay = Menu.EnterBirthDay(newDateTimeBirthDay);
            DateTime oldBirthDay = DateTimeBirthDay;
            DateTimeBirthDay = newDateTimeBirthDay;

            SaveAccauntDataInFile(Password, oldBirthDay);

            Console.Clear();
            Console.WriteLine("Дата рождения успешно обновлена!");
            Console.ReadKey();
        }

        private void SaveAccauntDataInFile(string oldPassword, DateTime oldDateTime)
        {
            var reader = new StreamReader(Menu.PathUsers);
            string tmp = reader.ReadToEnd();
            reader.Close();
            tmp = tmp.Replace($"{Login} - {oldPassword} - {oldDateTime.Date.ToString()}",
                $"{Login} - {Password} - {DateTimeBirthDay.ToString()}");
            StreamWriter writer = new StreamWriter(Menu.PathUsers, false, Encoding.Default);

            string[] lines = tmp.Split('\n');
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
            writer.Close();
        }

        public void StartQuiz(int choiceQuiz, AllQuizes myAllQuizzez, Dictionary<KeyValuePair<string, string>, int> statistics)
        {
            Console.Clear();
            Console.WriteLine($"Начинаем викторину \"{myAllQuizzez.AllQuizzezList[choiceQuiz].QuizName}\".");
            Console.WriteLine("\nДля продолжения нажмите любую кнопку...");
            Console.ReadKey();

            int correctAnswers = 0;
            int i = 0;
            while (i < myAllQuizzez.AllQuizzezList[choiceQuiz].QuizList.Count)
            {
                Console.Clear();
                Console.WriteLine(myAllQuizzez.AllQuizzezList[choiceQuiz].QuizList[i].Question+"\n");
                
                for (int j = 0; j < myAllQuizzez.AllQuizzezList[choiceQuiz].QuizList[i].PossibleOptionsList.Count; j++)
                {
                    Console.WriteLine($"{j} - {myAllQuizzez.AllQuizzezList[choiceQuiz].QuizList[i].PossibleOptionsList[j]}");
                }
                Console.WriteLine("\nВведите номер правильного ответа.");
                Console.WriteLine("Если их несколько, введите номера подряд без пробелов и запятых и нажмите Enter, иначе ответ не засчитается.");
                Console.Write("\nВвод: ");
                string userAnswers = Console.ReadLine();
                string corAnswer = "";
                foreach (int answ in myAllQuizzez.AllQuizzezList[choiceQuiz].QuizList[i].AnswersList)
                    corAnswer += answ.ToString();

                if (userAnswers == corAnswer)
                    ++correctAnswers;
                i++;
            }

            KeyValuePair<string, string> del = new KeyValuePair<string, string>(myAllQuizzez.AllQuizzezList[choiceQuiz].QuizName, Login);
            statistics.Remove(del);
            statistics.Add(new KeyValuePair<string, string>(myAllQuizzez.AllQuizzezList[choiceQuiz].QuizName, Login), correctAnswers);
            Menu.SaveStatisticsInFile(statistics);

            Console.Clear();
            Console.WriteLine("Викторина окончена!");
            Console.WriteLine($"Количество правильных ответов: {correctAnswers} из {myAllQuizzez.AllQuizzezList[choiceQuiz].QuizList.Count}.");
            Console.WriteLine();
        }

        public void StartMixedQuiz(AllQuizes myAllQuizzez, Dictionary<KeyValuePair<string, string>, int> statistics)
        {
            Console.Clear();
            Console.WriteLine($"Начинаем смешанную викторину!");
            Console.WriteLine("\nДля продолжения нажмите любую кнопку...");
            Console.ReadKey();

            List<XmlQuestionData> allQuestions = new List<XmlQuestionData>();
            foreach (Quiz quiz in myAllQuizzez.AllQuizzezList)
            {
                foreach (XmlQuestionData question in quiz.QuizList)
                    allQuestions.Add(question);
            }

            Random rnd = new Random();
            var shufledQuestions = allQuestions.OrderBy(x => rnd.Next()).ToList();

            shufledQuestions.RemoveRange(20, shufledQuestions.Count - 20);

            int correctAnswers = 0;
            int i = 0;
            while (i < shufledQuestions.Count)
            {
                Console.Clear();
                Console.WriteLine(shufledQuestions[i].Question + "\n");

                for (int j = 0; j < shufledQuestions[i].PossibleOptionsList.Count; j++)
                {
                    Console.WriteLine($"{j} - {shufledQuestions[i].PossibleOptionsList[j]}");
                }
                Console.WriteLine("\nВведите номер правильного ответа.");
                Console.WriteLine("Если их несколько, введите номера подряд без пробелов и запятых и нажмите Enter, иначе ответ не засчитается.");
                Console.Write("\nВвод: ");
                string userAnswers = Console.ReadLine();

                string corAnswer = "";
                foreach (int answ in shufledQuestions[i].AnswersList)
                    corAnswer += answ.ToString();
                if (userAnswers == corAnswer)
                    ++correctAnswers;
                i++;
            }

            KeyValuePair<string, string> del = new KeyValuePair<string, string>("Смешанная викторина", Login);
            statistics.Remove(del);
            statistics.Add(new KeyValuePair<string, string>("Смешанная викторина", Login), correctAnswers);
            Menu.SaveStatisticsInFile(statistics);

            Console.Clear();
            Console.WriteLine("Викторина окончена!");
            Console.WriteLine($"Количество правильных ответов: {correctAnswers} из {allQuestions.Count}.");
            Console.WriteLine();
        }


        public void ViewPastQuizzezResults(Dictionary<KeyValuePair<string, string>, int> statistics)
        {
            Console.Clear();
            Console.WriteLine("Результаты ваших ранее пройденных викторин:");
            Console.WriteLine();
            var MyQuizzezStats = from x in statistics
                                 where x.Key.Value.Contains(Login)
                                 select x;
            if (MyQuizzezStats == null || MyQuizzezStats.Count() == 0)
            {
                Console.Clear();
                Console.WriteLine($"Вы еще не прошли ни одной викторины!");
                Console.ReadKey();
            }
            else
            {
                foreach (var item in MyQuizzezStats)
                    Console.WriteLine($"\"{item.Key.Key}\" - {item.Value} правильных ответов.");
                Console.ReadKey();
            }
        }

        public void Exit()
        {
            Menu newMenu = new Menu();
        }
    }
}
