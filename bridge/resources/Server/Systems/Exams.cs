using System;
using System.Collections.Generic;
using System.Text;
using Main;
using Database;
using Extend;
using GTANetworkAPI;
using Logic.Account;
using Vehicle = GTANetworkAPI.Vehicle;

namespace Systems
{
    public class CExams
    {
        public List<CExamQuestion> questions = new List<CExamQuestion>();
        public void UpdateQuestions()
        {
            questions.Clear();
            Globals.Mysql.GetTableRows(ref questions);
        }

        public List<CExamQuestion> GetExamQuestions(byte exam)
        {
            object asdf = questions.FindAll(question => question.exam == exam);
            return questions.FindAll(question => question.exam == exam);
        }

        public CExams()
        {
            UpdateQuestions();
        }
    }
}