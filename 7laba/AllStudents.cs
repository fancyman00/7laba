using Laba7;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Laba7
{
    // Класс AllStudents, который управляет списком студентов и их данными
    public class AllStudents
    {
        // Список студентов
        public List<Student> Students = new List<Student>();

        // Конструктор класса AllStudents, инициализирует пустой список студентов
        public AllStudents()
        {
            Students = new List<Student>();
        }

        // Метод для открытия текстового файла и считывания данных студентов
        public void OpenTxtFile(string fileName)
        {
            // Создаем объект StreamReader для чтения файла
            var sr = new StreamReader(fileName);
            try
            {
                while (!sr.EndOfStream)
                {
                    // Создание нового студента на основе строки из файла и добавление в список студентов
                    var student = new Student(sr);
                    Students.Add(student);
                }
            }
            // Обработка ошибок в случае некорректного формата данных
            catch (FormatException ex)
            {
                Students.Clear(); // Очищаем список студентов
                MessageBox.Show("Числа в файле должны быть положительными цифрами", "Некорректный файл!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Students.Clear();
                MessageBox.Show(ex.Message, "Некорректный файл!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IndexOutOfRangeException ex)
            {
                Students.Clear();
                MessageBox.Show("Номер курса не соответствует количеству сессий", "Некорректный файл!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Students.Clear();
                MessageBox.Show(ex.Message, "Некорректный файл!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sr.Close(); // Закрытие потока после завершения работы
            }
        }

        // Метод для сохранения данных студентов в текстовый файл
        public void SaveTxtFile(string fileName)
        {
            using (var sw = new StreamWriter(fileName))
            {
                // Записываем каждый объект студента в файл
                foreach (var student in Students)
                    sw.WriteLine(student);
            }
        }

        // Метод для открытия бинарного файла и
        // Десериализации данных студентов
        public void OpenBinFile(string fileName)
        {
            var binFormatter = new BinaryFormatter(); // Создаем бинарный форматтер
            var file = new FileStream(fileName, FileMode.OpenOrCreate); // Открываем файл для чтения
            try
            {
                // Десериализуем список студентов из бинарного файла
                Students = binFormatter.Deserialize(file) as List<Student>;
            }
            catch (Exception ex)
            {
                Students.Clear(); // Очищаем список студентов при ошибке
                MessageBox.Show(ex.Message, "Некорректный файл!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                file.Close(); // Закрываем файл
            }
        }

        // Метод для сохранения данных студентов в бинарный файл
        public void SaveBinFile(string fileName)
        {
            var binFormatter = new BinaryFormatter(); // Создаем бинарный форматтер
            using (var file = new FileStream(fileName, FileMode.OpenOrCreate)) // Открываем файл для записи
            {
                // Сериализуем список студентов в бинарный файл
                binFormatter.Serialize(file, Students);
            }
        }

        // Метод для открытия XML файла и
        // Десериализации данных студентов
        public void OpenXmlFile(string fileName)
        {
            var xmlFormatter = new XmlSerializer(typeof(List<Student>)); // Создаем сериализатор для списка студентов
            try
            {
                using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read)) // Открываем файл для чтения
                {
                    // Десериализуем список студентов из XML файла
                    Students = xmlFormatter.Deserialize(file) as List<Student>;
                }
            }
            catch (Exception ex)
            {
                Students.Clear(); // Очищаем список студентов при ошибке
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Некорректный файл!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // Метод для сохранения данных студентов в XML файл
        public void SaveXmlFile(string fileName)
        {
            var xmlFormatter = new XmlSerializer(typeof(List<Student>)); // Создаем сериализатор для списка студентов
            try
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write)) // Открываем файл для записи, перезаписывая его
                {
                    // Сериализуем список студентов в XML файл
                    xmlFormatter.Serialize(file, Students);
                }
                MessageBox.Show($"Данные успешно сохранены в файл: {fileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Метод для нахождения всех курсов с наибольшим числом студентов
        public List<int> FindCoursesWithMostStudents()
        {
            // Словарь для хранения количества студентов по курсам
            var studentsByCourse = new Dictionary<int, int>();

            // Проходим по всем студентам
            foreach (var student in Students)
            {
                // Получаем текущий номер курса студента
                int course = student.Course;

                // Инициализируем счетчик для курса, если он еще не добавлен
                if (!studentsByCourse.ContainsKey(course))
                {
                    studentsByCourse[course] = 0;
                }

                // Увеличиваем количество студентов на курсе
                studentsByCourse[course]++;
            }

            // Проверяем, есть ли студенты
            if (studentsByCourse.Count == 0)
            {
                throw new InvalidOperationException("Список студентов пуст.");
            }

            // Поиск наибольшего числа студентов
            int maxStudents = int.MinValue;

            // Список курсов с наибольшим числом студентов
            var coursesWithMaxStudents = new List<int>();

            // Перебираем все курсы и ищем наибольшее количество студентов
            foreach (var kvp in studentsByCourse)
            {
                if (kvp.Value > maxStudents) // Если нашли курс с большим количеством студентов
                {
                    maxStudents = kvp.Value;
                    coursesWithMaxStudents.Clear(); // Очищаем список, так как нашли новый максимум
                    coursesWithMaxStudents.Add(kvp.Key); // Добавляем текущий курс
                }
                else if (kvp.Value == maxStudents) // Если количество студентов равно текущему максимуму
                {
                    coursesWithMaxStudents.Add(kvp.Key); // Добавляем курс в список
                }
            }

            // Возвращаем список курсов с наибольшим числом студентов
            return coursesWithMaxStudents;
        }

        //метод для нахождения всех курсов с минимальным числом неуспевающих студентов
        public List<int> FindCoursesWithLeastFailingStudents()
        {
            // Словарь для хранения количества неуспевающих студентов по курсам
            var failingStudentsByCourse = new Dictionary<int, int>();

            // Проходим по всем студентам
            foreach (var student in Students)
            {
                // Получаем текущий номер курса студента
                int course = student.Course;

                // Инициализируем счетчик для курса, если он еще не добавлен
                if (!failingStudentsByCourse.ContainsKey(course))
                {
                    failingStudentsByCourse[course] = 0;
                }

                // Проверяем статус предметов студента
                var subjectStatuses = student.GetSubjectsStatus();

                // Если хотя бы один предмет "не сдан", увеличиваем счетчик
                if (subjectStatuses.ContainsValue(false))
                {
                    failingStudentsByCourse[course]++;
                }
            }

            // Проверяем, есть ли студенты
            if (failingStudentsByCourse.Count == 0)
            {
                throw new InvalidOperationException("Список студентов пуст.");
            }

            // Поиск минимального количества неуспевающих студентов
            int minFailures = int.MaxValue;

            // Список курсов с минимальным количеством неуспевающих студентов
            var coursesWithMinFailures = new List<int>();

            // Перебираем все курсы и ищем минимальное количество
            foreach (var kvp in failingStudentsByCourse)
            {
                if (kvp.Value < minFailures) // Если нашли курс с меньшим количеством неуспевающих
                {
                    minFailures = kvp.Value;
                    coursesWithMinFailures.Clear(); // Очищаем список, так как нашли новый минимум
                    coursesWithMinFailures.Add(kvp.Key); // Добавляем текущий курс
                }
                else if (kvp.Value == minFailures) // Если количество неуспевающих студентов равно текущему минимуму
                {
                    coursesWithMinFailures.Add(kvp.Key); // Добавляем курс в список
                }
            }

            // Возвращаем список курсов с минимальным количеством неуспевающих студентов
            return coursesWithMinFailures;
        }


    }
}
