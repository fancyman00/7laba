using Laba7;
using System.IO;
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Laba7
{
    public partial class StudentsListForm : Form
    {
        // Инициализация компонентов формы (управляющие элементы, их расположение и т. д.)
        public StudentsListForm()
        {
            InitializeComponent();
        }
        // Переменная для хранения индекса выбранного студента (по умолчанию -1, что означает "не выбран")
        private int selectedNumber = -1;  

        private AllStudents studentsList = new AllStudents();  // Список студентов (инициализируется как пустой список)

        private string fileName = string.Empty;  // Переменная для хранения имени текущего файла (если файл открыт)
        private enum FileType  // Перечисление для типов файлов
        {
            None, Txt, Bin, Xml
        }

        private FileType fileType = FileType.None;  // Переменная для хранения текущего типа файла (по умолчанию None)

        // Обработчик события нажатия на пункт меню "Справка"
        private void help_click(object sender, EventArgs e)
        {
            MessageBox.Show("Найти курс, на котором наибольшее число студентов..", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Обработчик события нажатия на пункт меню "Открыть"
        private void open_file_click(object sender, EventArgs e)
        {
            listBoxSubjects.Items.Clear();  // Очищаем список предметов
            studentsList = new AllStudents();  // Создаем новый объект для списка студентов
            var dlg = new OpenFileDialog();  // Создаем диалоговое окно для открытия файла
            dlg.Filter = "Text (*.txt)|*.txt|Binary (*.bin)|*.bin|XML (*.xml)|*.xml";

            if (dlg.ShowDialog() == DialogResult.OK)  // Если пользователь выбрал файл
            {
                fileName = dlg.FileName;  // Сохраняем путь к выбранному файлу
                var extension = fileName.Substring(fileName.LastIndexOf("."));  // Получаем расширение файла
                switch (extension)  // В зависимости от расширения файла выполняем соответствующие действия
                {
                    case ".txt":
                        fileType = FileType.Txt;
                        studentsList.OpenTxtFile(fileName);
                        ShowList();
                        break;

                    case ".bin":
                        fileType = FileType.Bin;
                        studentsList.OpenBinFile(fileName);
                        ShowList();
                        break;

                    case ".xml":
                        fileType = FileType.Xml;
                        studentsList.OpenXmlFile(fileName);
                        ShowList();
                        break;

                    default:
                        break;
                }
            }
        }

        // Метод для отображения списка студентов
        private void ShowList(int index = 0)
        {
            var cnt = listBoxAllStudents.Items.Count;  // Получаем количество элементов в списке студентов
            for (var i = cnt - 1; i >= index; --i)  // Удаляем элементы списка, начиная с индекса
            {
                listBoxAllStudents.Items.RemoveAt(i);
            }
            for (var i = index; i < studentsList.Students.Count; i++)  // Добавляем студентов в список с индексом начиная с переданного
                listBoxAllStudents.Items.Add((i + 1).ToString() + ". " + studentsList.Students[i].ToListBox());  // Добавляем студентов в ListBox
            listBoxAllStudents.Enabled = true;  // Включаем ListBox
        }

        // Обработчик события для создания нового файла
        private void create_file_click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();  // Создаем диалоговое окно для сохранения файла
            dlg.DefaultExt = ".txt";
            studentsList = new AllStudents();
            fileName = String.Empty;
            if (dlg.ShowDialog() != DialogResult.Cancel)  // Если пользователь не отменил сохранение
            {
                fileName = dlg.FileName;  // Сохраняем путь к выбранному файлу
                StreamWriter f_out = new StreamWriter(fileName, false, Encoding.UTF8);  // Создаем поток для записи в файл с кодировкой UTF-8
                f_out.Close();
                listBoxAllStudents.Items.Clear();
                MessageBox.Show("Файл успешно создан");  // Выводим сообщение об успешном создании файла
            }
        }

        // Обработчик события для сохранения файла
        private void save_file_click(object sender, EventArgs e)
        {
            switch (fileType)  // В зависимости от типа файла выполняем различные действия
            {
                case FileType.None:
                    // Если файл не выбран, вызываем метод для создания нового файла
                    create_file_click(sender, e);
                    break;

                case FileType.Txt:
                    studentsList.SaveTxtFile(fileName);
                    MessageBox.Show($"Файл {fileName.Substring(fileName.LastIndexOf("\\") + 1)} успешно сохранен");
                    break;

                case FileType.Xml:
                    studentsList.SaveXmlFile(fileName);
                    MessageBox.Show($"Файл {fileName.Substring(fileName.LastIndexOf("\\") + 1)} успешно сохранен");
                    break;

                case FileType.Bin:
                    studentsList.SaveBinFile(fileName);
                    MessageBox.Show($"Файл {fileName.Substring(fileName.LastIndexOf("\\") + 1)} успешно сохранен");
                    break;
            }
        }

        // Обработчик события для сохранения файла с выбором другого пути или формата
        private void save_AS_file_click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();  // Создаем диалоговое окно для сохранения файла
            dlg.Filter = "Text (*.txt)|*.txt|Binary (*.bin)|*.bin|XML (*.xml)|*.xml";

            if (dlg.ShowDialog() == DialogResult.OK)  // Если пользователь выбрал файл
            {
                fileName = dlg.FileName;  // Сохраняем путь к файлу
                var extension = fileName.Substring(fileName.LastIndexOf("."));
                switch (extension)
                {
                    case ".txt":
                        fileType = FileType.Txt;
                        studentsList.SaveTxtFile(fileName);
                        break;

                    case ".bin":
                        fileType = FileType.Bin;
                        studentsList.SaveBinFile(fileName);
                        break;

                    case ".xml":
                        fileType = FileType.Xml;
                        studentsList.SaveXmlFile(fileName);
                        break;

                    default:
                        break;
                }
            }
        }

        private void task_click(object sender, EventArgs e) // Обработчик для задачи 
        {
            listBoxSubjects.Items.Clear();

            // Проверка на наличие студентов
            if (listBoxAllStudents.Items.Count < 1)
            {
                MessageBox.Show("Список пуст.");
            }
            else
            {
                // Получаем список курсов с наименьшим числом неуспевающих студентов
                List<int> coursesWithLeastFailures = studentsList.FindCoursesWithMostStudents();

                // Формируем строку с курсами
                if (coursesWithLeastFailures.Count > 0)
                {
                    string result = "Курсы с наибольшим числом студентов: "
                                    + string.Join(", ", coursesWithLeastFailures);
                    listBoxSubjects.Items.Add(result);
                    listBoxSubjects.Enabled = true;
                }
                else
                {
                    listBoxSubjects.Items.Add("Не удалось найти курсы.");
                }
            }
        }


        // Обработчик события для добавления нового студента
        private void add_student_click(object sender, EventArgs e)
        {
            var form = new StudentForm();  // Вызываем форму для добавления нового студента
            form.ShowDialog();
            if (form.Changed)  // Если данные студента были изменены
            {
                studentsList.Students.Add(StudentForm.student);
                selectedNumber = studentsList.Students.Count - 1;  // Обновляем индекс выбранного студента
                listBoxAllStudents.Items.Add((selectedNumber + 1).ToString() +
                    ". " + studentsList.Students[selectedNumber].ToListBox());  // Добавляем студента в список
            }
            if (listBoxSubjects.Items.Count > 0)
                task_click(sender, e);  // Если есть предметы, выполняем задачу
        }

        // Обработчик события для изменения данных студента
        private void change_student(object sender, EventArgs e)
        {
            selectedNumber = listBoxAllStudents.SelectedIndex;  // Получаем индекс выбранного студента
            if (selectedNumber >= 0)  // Если студент выбран
            {
                var student = studentsList.Students[selectedNumber];  // Получаем выбранного студента
                var form = new StudentForm(student);  // Открываем форму для редактирования данных студента
                form.ShowDialog();  
                if (form.Changed)  
                {
                    studentsList.Students[selectedNumber] = StudentForm.student;  // Обновляем данные студента в списке
                    ShowList(selectedNumber);
                }
                listBoxAllStudents.SelectedIndex = -1;  // Снимаем выделение с элемента в списке
                if (listBoxSubjects.Items.Count > 0)
                    task_click(sender, e);  // Если есть предметы, выполняем задачу
            }
        }

        // Обработчик события для удаления студента
        private void delete_student_click(object sender, EventArgs e)
        {
            string s = showDeleteDialog("Введите номер удаляемого студента", "Удаление");
            if (s != String.Empty)  // Если номер студента введен
            {
                int index = int.Parse(s) - 1;  // Получаем индекс студента (нумерация с 1)
                if (index <= listBoxAllStudents.Items.Count && index >= 0)  // Если индекс валиден
                {
                    studentsList.Students.RemoveAt(index);  // Удаляем студента из списка
                    ShowList(index);  // Перерисовываем список студентов
                    if (listBoxSubjects.Items.Count > 0)
                        task_click(sender, e);  // Если есть предметы, выполняем задачу
                }
                else
                    MessageBox.Show("Неверный номер студента", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Сообщаем о неверном номере
            }
        }

        // Метод для отображения диалога для удаления студента
        private static string showDeleteDialog(string message, string header)
        {
            Form formForDelete = new Form()  // Создаем форму для ввода данных
            {
                Width = 330,
                Height = 130,
                FormBorderStyle = FormBorderStyle.Fixed3D,
                Text = header,
                StartPosition = FormStartPosition.CenterScreen,
            };
            Label textLabel = new Label() { Left = 10, Top = 20, Text = message, Width = message.Length * 12 };  // Текстовое поле с сообщением
            TextBox textBox = new TextBox() { Left = 10, Top = 50, Width = 100 };  // Поле для ввода номера студента
            Button confirmation = new Button() { Text = "OK", Left = 223, Width = 60, Top = 50, DialogResult = DialogResult.OK };  // Кнопка подтверждения
            textBox.KeyPress += (sender, e) =>  // Обработчик для проверки ввода только цифр
            {
                char number = e.KeyChar;
                if (number != 8 && !char.IsDigit(number))  // Если введен нецифровой символ
                    e.Handled = true;
            };
            formForDelete.Controls.Add(textBox);
            formForDelete.Controls.Add(textLabel);
            formForDelete.Controls.Add(confirmation);
            formForDelete.AcceptButton = confirmation;  // Привязка кнопки Enter к кнопке OK
            confirmation.Click += (sender, e) =>
            {
                formForDelete.Close();  // Закрытие формы после нажатия OK
            };
            return formForDelete.ShowDialog() == DialogResult.OK ? textBox.Text : "";  // Возвращаем введенный номер студента или пустую строку
        }
    }
}
