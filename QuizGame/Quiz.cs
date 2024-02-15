using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame
{
    public class Quiz
    {
        public int QuizId;
        public string QuizName;
        public List<XmlQuestionData> QuizList;

        public Quiz(int quizId, string quizName, List<XmlQuestionData> quizList)
        {
            QuizId = quizId;
            QuizName = quizName;
            QuizList = new List<XmlQuestionData>(quizList);
        }
    }
}
