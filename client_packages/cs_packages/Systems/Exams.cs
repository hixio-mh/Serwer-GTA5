using System;
using System.Collections.Generic;
using System.Text;
using Main;
using RAGE;
using Manager;
using Extend;
using Newtonsoft.Json.Linq;

namespace Systems
{
    public enum ELicense
    {
        A, B, C
    }

    public class CQuestion
    {
        public readonly string question;
        public readonly List<string> answers;
        public readonly byte correct;
        public CQuestion(string question, byte correct, List<string> answers)
        {
            this.question = question;
            this.correct = correct;
            this.answers = answers;
        }
    }

    public class CExam {
        public List<CQuestion> Questions = new List<CQuestion>();
        public List<Vector3> RouteManeuveringArea;
        public List<Vector3> RouteCity;
        ELicense License;

        public CExam(ELicense license)
        {
            License = license;
            UpdateQuestions();
        }

        public void UpdateQuestions()
        {
            Globals.Managers.rpc.TriggerServer(CRPCManager.ERPCs.EXAMS_QUESTIONS, (int)License);
        }
    }

    class CExamA : CExam
    {
        public CExamA() : base(ELicense.A)
        {

            RouteManeuveringArea = new List<Vector3>
            {
                new Vector3(0,0,0),
                new Vector3(10,0,0),
                new Vector3(20,0,0),
            };

            RouteCity = new List<Vector3>
            {
                new Vector3(0,0,0),
                new Vector3(10,0,0),
                new Vector3(20,0,0),
            };
        }
    }
    class CExamB : CExam
    {
        public CExamB() : base(ELicense.B)
        {
            RouteManeuveringArea = new List<Vector3>
            {
                new Vector3(0,0,0),
                new Vector3(10,0,0),
                new Vector3(20,0,0),
            };

            RouteCity = new List<Vector3>
            {
                new Vector3(0,0,0),
                new Vector3(10,0,0),
                new Vector3(20,0,0),
            };

        }
    }
    class CExamC : CExam
    {
        public CExamC() : base(ELicense.C)
        {
            RouteManeuveringArea = new List<Vector3>
            {
                new Vector3(0,0,0),
                new Vector3(10,0,0),
                new Vector3(20,0,0),
            };

            RouteCity = new List<Vector3>
            {
                new Vector3(0,0,0),
                new Vector3(10,0,0),
                new Vector3(20,0,0),
            };
        }
    }

    public class CExams
    {

        List<CExam> Exams;

        public CExams()
        {
            Exams = new List<CExam> { new CExamA(), new CExamB(), new CExamC() };
        }

        public void OnExamQuestionsCallback(CRPCExamQuestionsCallback obj)
        {
            JArray questions = obj.questions;
            int count = questions.Count;
            if (count <= 0) return;

            int license = (int)questions[0]["exam"];
            CExam exam = Exams[license];

            List<CQuestion> questionsList = new List<CQuestion>(count);
            byte correct;
            List<string> abcd = new List<string>(4);
            for (int i = 0; i < count; i++)
            {
                JToken token = questions[i];
                abcd.Clear();
                abcd.Add((string)token["a"]);
                abcd.Add((string)token["b"]);
                abcd.Add((string)token["c"]);
                abcd.Add((string)token["d"]);
                abcd.Shuffle();
                correct = (byte)abcd.IndexOf((string)token["a"]);

                questionsList.Add(new CQuestion((string)token["question"], correct, abcd));
            }
            //Questions.Clear();
            //ChatExtend.Output("questionsList {0}", questionsList.Serialize());
            //questions.ToObject<CQuestion>();
        }
    }
}
