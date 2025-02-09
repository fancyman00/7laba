using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba7
{
    [Serializable]
    public class Exam
    { 
        public string Subject { get; set; }

        private byte _mark;
        public byte Mark
        {
            get
            {
                return _mark;
            }
            set
            {
                if (value < 2 || value > 5)
                    throw new ArgumentOutOfRangeException("Оценка должна находиться в диапазоне от 2 до 5");
                _mark = value;
            }
        }

        // Конструктор без параметров, инициализирует предмет пустой строкой и оценку со значением 2
        public Exam()
        {
            Subject = string.Empty;
            _mark = 2;
        }

        // Конструктор с параметрами для инициализации предмета и оценки
        public Exam(string subject, byte mark)
        {
            Subject = subject;
            Mark = mark; // Вызывается сеттер Mark, который проверит допустимость оценки
        }

        // Метод для проверки, является ли оценка неудовлетворительной (оценка 2)
        public bool isBad()
        {
            return Mark == 2;
        }

    }

    // Класс, представляющий студента
    [Serializable]
    public class Student
    {
        // Перечисление для формы обучения: "Бюджет" или "Договор"
        public enum EEducationForm { Budget, Contract }

        // Свойство для хранения ФИО студента
        public string FIO { get; set; }

        // Закрытое поле для хранения курса студента
        private byte _course;

        // Свойство для получения и установки курса студента
        public byte Course
        {
            get { return _course; }
            set
            {
                if (value < 1 || value > 4)
                    throw new ArgumentOutOfRangeException("Несуществующий номер курса");

                _course = value;
            }
        }

        // Закрытое поле для хранения номера группы
        private byte _group;

        // Свойство для получения и установки номера группы
        public byte Group
        {
            get { return _group; }
            set
            {

                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("Несуществующий номер группы");

                _group = value;
            }
        }

        // Массив экзаменов, сдаваемых студентом
        public Exam[] Exams { get; set; }

        // Свойство для хранения формы обучения (Бюджет или Договор)
        public EEducationForm EducationForm { get; set; }

        // Константы для количества экзаменов и сессий
        static public readonly byte CountExams = 5;
        static public readonly byte CountSessions = 8;

        // Конструктор по умолчанию, который инициализирует студента с начальными значениями
        public Student()
        {
            FIO = string.Empty;
            _group = 1;
            _course = 1;
            Exams = new Exam[CountExams * CountSessions];
            for (var i = 0; i < Exams.Length; ++i)
                Exams[i] = new Exam(); // Заполнение массива экзаменов значениями по умолчанию
            EducationForm = EEducationForm.Contract; // Форма обучения по умолчанию — "Договор"
        }

        // Конструктор, инициализирующий студента с заданными параметрами
        public Student(string fio, byte course, byte group, Exam[] exams, EEducationForm educationForm)
        {
            FIO = fio;
            _course = course;
            _group = group;
            Exams = exams;
            EducationForm = educationForm;
        }

        // Конструктор, который считывает данные о студенте из потока (например, из файла)
        // Конструктор для создания объекта Student из текстового файла
        public Student(StreamReader sr)
        {
            // Чтение строки с фамилией, именем и отчеством студента
            FIO = sr.ReadLine();

            // Чтение строки с номером курса студента и преобразование её в тип byte
            Course = byte.Parse(sr.ReadLine());

            // Чтение строки с номером группы студента и преобразование её в тип byte
            Group = byte.Parse(sr.ReadLine());

            // Определение общего количества экзаменов для студента
            // Количество экзаменов = количество предметов (CountExams) * номер курса * 2
            int examNum = CountExams * _course * 2;

            // Инициализация массива экзаменов (Exam) заданного размера
            Exams = new Exam[examNum];

            // Чтение информации об экзаменах из файла
            for (var i = 0; i < examNum; ++i)
            {
                // Читаем строку с названием предмета и оценкой, разделённую символом ':'
                var lines = sr.ReadLine().Split(':');

                // Название предмета (до символа ':')
                var subject = lines[0];

                // Оценка за экзамен (после ':', убираем пробелы)
                var mark = byte.Parse(lines[1].Trim());

                // Создаём объект экзамена (Exam) и добавляем его в массив
                Exams[i] = new Exam(subject, mark);
            }

            // Чтение строки с формой обучения ("бюджет" или "договор")
            if (sr.ReadLine().ToLower() == "бюджет")
            {
                // Если строка содержит "бюджет", устанавливаем форму обучения как бюджетную
                EducationForm = EEducationForm.Budget;
            }
            else
            {
                // В противном случае устанавливаем форму обучения как договорную
                EducationForm = EEducationForm.Contract;
            }
        }


        // Метод для отображения информации о студенте в удобном формате для списка
        public string ToListBox()
        {
            return $"{FIO}, {_course}-й курс, группа {_group}, {(EducationForm == EEducationForm.Budget ? "бюджет" : "договор")}";
        }

        // Переопределение метода ToString для отображения полной информации о студенте
        public override string ToString()
        {
            string result = $"{FIO}\n{_course}\n{_group}\n"; 
            int examNum = _course * CountExams * 2; 
            for (var i = 0; i < examNum; ++i)
                result += Exams[i].ToString() + "\n"; 
            result += EducationForm == EEducationForm.Budget ? "Бюджет" : "Договор"; 
            return result;
        }

        // Метод для получения статуса каждого предмета (сдал или нет)
        public Dictionary<string, bool> GetSubjectsStatus()
        {
            var subjects = new Dictionary<string, bool>(); 

            foreach (var exam in Exams)
            {
                // Если предмет еще не добавлен в словарь, добавляем его со статусом "успешно сдан"
                if (!subjects.ContainsKey(exam.Subject))
                {
                    subjects[exam.Subject] = true; // Предполагаем, что нет неуспевающих, пока не найдем плохую оценку
                }
               
                if (exam.Mark < 3)
                {
                    subjects[exam.Subject] = false; 
                }
            }

            return subjects; // Возвращаем словарь с результатами
        }
    }
}
