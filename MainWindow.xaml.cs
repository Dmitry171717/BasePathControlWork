using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Controlnaya
{
    /// <summary>
    /// Тестовый случай для метода базового пути.
    /// </summary>
    public class TestCase
    {
        /// <summary>Название базового пути.</summary>
        public string PathName { get; set; }

        /// <summary>Входные данные в виде строки.</summary>
        public string Inputs { get; set; }

        /// <summary>Ожидаемое значение результата.</summary>
        public string Expected { get; set; }

        /// <summary>Фактически полученное значение результата.</summary>
        public string Actual { get; set; }

        /// <summary>Статус прохождения теста (пройден / не пройден).</summary>
        public string Passed { get; set; }
    }

    /// <summary>
    /// Строка сравнения правильной и ошибочной версий функции.
    /// </summary>
    public class CompareCase
    {
        /// <summary>Название базового пути.</summary>
        public string PathName { get; set; }

        /// <summary>Входные данные в виде строки.</summary>
        public string Inputs { get; set; }

        /// <summary>Результат правильной версии функции.</summary>
        public string Correct { get; set; }

        /// <summary>Результат ошибочной версии функции.</summary>
        public string Uncorrect { get; set; }

        /// <summary>Итог сравнения: совпадение или расхождение.</summary>
        public string Result { get; set; }
    }

    /// <summary>
    /// Главное окно приложения.
    /// Реализует тестирование методом базового пути для варианта 6:
    /// <list type="bullet">
    /// <item>y = a·x³ + b² + c, при x &lt; 0 и b ≠ 0</item>
    /// <item>y = (x − a) / c, при x &gt; 0 и c ≠ 0</item>
    /// <item>y = 15·x / c, в остальных случаях</item>
    /// </list>
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Инициализирует компоненты главного окна.
        /// </summary>
        public MainWindow() { InitializeComponent(); }

        /// <summary>
        /// Вычисляет значение кусочной функции варианта 6.
        /// </summary>
        /// <param name="x">Аргумент функции.</param>
        /// <param name="a">Коэффициент a.</param>
        /// <param name="b">Коэффициент b.</param>
        /// <param name="c">Коэффициент c.</param>
        /// <returns>Значение y согласно условиям варианта.</returns>
        double CalcY(double x, double a, double b, double c)
        {
            double y;
            if (x < 0 && b != 0)                           // узлы 2, 3
            {
                y = a * Math.Pow(x, 3) + b * b + c;         // узел 4
            }
            else if (x > 0 && c != 0)                       // узлы 5, 6
            {
                y = (x - a) / c;                            // узел 7
            }
            else
            {
                y = 15 * x / c;                              // узел 8
            }
            return y;                                        // узел 9
        }

        /// <summary>
        /// Ошибочная версия функции для демонстрации работы тестов.
        /// Отличается заменой оператора && на || в условиях ветвления.
        /// </summary>
        /// <param name="x">Аргумент функции.</param>
        /// <param name="a">Коэффициент a.</param>
        /// <param name="b">Коэффициент b.</param>
        /// <param name="c">Коэффициент c.</param>
        /// <returns>Значение y, вычисленное по ошибочной логике.</returns>
        double UncorrectY(double x, double a, double b, double c)
        {
            double y;
            if (x < 0 || b != 0)
            {
                y = a * Math.Pow(x, 3) + b * b + c;
            }
            else if (x > 0 || c != 0)
            {
                y = (x - a) / c;
            }
            else
            {
                y = 15 * x / c;
            }
            return y;
        }

        /// <summary>
        /// Форматирует вещественное число для вывода.
        /// Обрабатывает специальные случаи: бесконечность и NaN.
        /// </summary>
        /// <param name="val">Значение для форматирования.</param>
        /// <returns>Строковое представление значения.</returns>
        string R(double val)
        {
            if (double.IsInfinity(val)) return val > 0 ? "+∞" : "-∞";
            if (double.IsNaN(val)) return "не определено";
            return Math.Round(val, 4).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Разбирает строку в вещественное число.
        /// Допускает как точку, так и запятую в качестве разделителя.
        /// </summary>
        /// <param name="s">Строка для разбора.</param>
        /// <returns>Вещественное число.</returns>
        double ParseD(string s)
        {
            return double.Parse(s.Trim().Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Обработчик кнопки "Вычислить".
        /// Считывает a, b, c, x из полей ввода и отображает результат.
        /// </summary>
        void BtnCalc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double a = ParseD(tbA.Text), b = ParseD(tbB.Text),
                       c = ParseD(tbC.Text), x = ParseD(tbX.Text);
                lblResult.Content = "y = " + R(CalcY(x, a, b, c));
            }
            catch { lblResult.Content = "y = Ошибка ввода"; }
        }

        /// <summary>
        /// Запускает набор тестов, обновляет таблицу и статистику.
        /// </summary>
        /// <param name="tests">Список тестовых случаев для выполнения.</param>
        void RunTests(List<TestCase> tests)
        {
            foreach (var t in tests)
            {
                t.Passed = (t.Actual == t.Expected) ? "✔ Пройден" : "✘ Не пройден";
            }
            dgTests.ItemsSource = null;
            dgTests.ItemsSource = tests;
            int passed = 0, failed = 0;
            foreach (var t in tests)
                if (t.Passed.StartsWith("✔")) passed++; else failed++;
            lblStats.Content = "Пройдено: " + passed + "  Не пройдено: " + failed;

            int distinctPaths = tests.Select(t => t.PathName).Distinct().Count();
            double testedness = (double)distinctPaths / 5 * 100;
            lblTested.Content = "Оценка тестированности: " + distinctPaths +
                                " / 5 базовых путей (" + Math.Round(testedness) + "%)";
            DrawChart(passed, failed);
        }

        /// <summary>
        /// Создаёт тестовый случай для указанного базового пути.
        /// </summary>
        /// <param name="pathName">Название пути (например, "Путь 1").</param>
        /// <param name="a">Коэффициент a.</param>
        /// <param name="b">Коэффициент b.</param>
        /// <param name="c">Коэффициент c.</param>
        /// <param name="x">Аргумент x.</param>
        /// <param name="expected">Ожидаемое значение y.</param>
        /// <returns>Заполненный объект <see cref="TestCase"/>.</returns>
        TestCase MakeCase(string pathName, double a, double b, double c,
                          double x, double expected)
        {
            double actual = CalcY(x, a, b, c);
            return new TestCase
            {
                PathName = pathName,
                Inputs = "a=" + a + ", b=" + b + ", c=" + c + ", x=" + x,
                Expected = R(expected),
                Actual = R(actual)
            };
        }

        /// <summary>
        /// Тест пути 1: 1-2-3-4-9 (x &lt; 0, b ≠ 0) — ветка a·x³ + b² + c.
        /// </summary>
        void BtnPath1_Click(object sender, RoutedEventArgs e)
        {
            var t = MakeCase("Путь 1", 2, 3, 1, -2,
                             2 * Math.Pow(-2, 3) + 3 * 3 + 1);
            RunTests(new List<TestCase> { t });
        }

        /// <summary>
        /// Тест пути 2: 1-2-3-5-8-9 (x &lt; 0, b = 0) — ветка 15·x/c.
        /// </summary>
        void BtnPath2_Click(object sender, RoutedEventArgs e)
        {
            var t = MakeCase("Путь 2", 2, 0, 2, -2, 15 * -2.0 / 2);
            RunTests(new List<TestCase> { t });
        }

        /// <summary>
        /// Тест пути 3: 1-2-5-6-7-9 (x &gt; 0, c ≠ 0) — ветка (x − a) / c.
        /// </summary>
        void BtnPath3_Click(object sender, RoutedEventArgs e)
        {
            var t = MakeCase("Путь 3", 2, 3, 2, 4, (4.0 - 2) / 2);
            RunTests(new List<TestCase> { t });
        }

        /// <summary>
        /// Тест пути 4: 1-2-5-6-8-9 (x &gt; 0, c = 0) — деление на ноль, ветка 15·x/c.
        /// </summary>
        void BtnPath4_Click(object sender, RoutedEventArgs e)
        {
            var t = MakeCase("Путь 4", 2, 3, 0, 4, 15 * 4.0 / 0);
            RunTests(new List<TestCase> { t });
        }

        /// <summary>
        /// Тест пути 5: 1-2-5-8-9 (x = 0) — оба условия ложны, ветка 15·x/c.
        /// </summary>
        void BtnPath5_Click(object sender, RoutedEventArgs e)
        {
            var t = MakeCase("Путь 5", 2, 3, 2, 0, 15 * 0.0 / 2);
            RunTests(new List<TestCase> { t });
        }

        /// <summary>
        /// Запускает тесты по всем пяти базовым путям одновременно.
        /// </summary>
        void BtnAllPaths_Click(object sender, RoutedEventArgs e)
        {
            RunTests(new List<TestCase>
            {
                MakeCase("Путь 1", 2, 3, 1, -2, 2 * Math.Pow(-2, 3) + 3 * 3 + 1),
                MakeCase("Путь 2", 2, 0, 2, -2, 15 * -2.0 / 2),
                MakeCase("Путь 3", 2, 3, 2, 4,  (4.0 - 2) / 2),
                MakeCase("Путь 4", 2, 3, 0, 4,  15 * 4.0 / 0),
                MakeCase("Путь 5", 2, 3, 2, 0,  15 * 0.0 / 2),
            });
        }

        /// <summary>
        /// Рисует столбчатую диаграмму результатов тестирования на канвасе.
        /// </summary>
        /// <param name="passed">Количество пройденных тестов.</param>
        /// <param name="failed">Количество непройденных тестов.</param>
        void DrawChart(int passed, int failed)
        {
            chartCanvas.Children.Clear();
            int total = passed + failed;
            if (total == 0) return;
            double maxH = 180, barW = 60;

            double h1 = (double)passed / total * maxH;
            var r1 = new Rectangle { Width = barW, Height = h1, Fill = Brushes.SteelBlue };
            Canvas.SetLeft(r1, 40); Canvas.SetTop(r1, 200 - h1);
            chartCanvas.Children.Add(r1);
            var l1 = new TextBlock { Text = passed.ToString() };
            Canvas.SetLeft(l1, 62); Canvas.SetTop(l1, 200 - h1 - 20);
            chartCanvas.Children.Add(l1);
            var lb1 = new TextBlock { Text = "Пройдено", FontSize = 11 };
            Canvas.SetLeft(lb1, 30); Canvas.SetTop(lb1, 205);
            chartCanvas.Children.Add(lb1);

            double h2 = (double)failed / total * maxH;
            var r2 = new Rectangle { Width = barW, Height = h2, Fill = Brushes.Tomato };
            Canvas.SetLeft(r2, 160); Canvas.SetTop(r2, 200 - h2);
            chartCanvas.Children.Add(r2);
            var l2 = new TextBlock { Text = failed.ToString() };
            Canvas.SetLeft(l2, 182); Canvas.SetTop(l2, 200 - h2 - 20);
            chartCanvas.Children.Add(l2);
            var lb2 = new TextBlock { Text = "Не пройдено", FontSize = 11 };
            Canvas.SetLeft(lb2, 145); Canvas.SetTop(lb2, 205);
            chartCanvas.Children.Add(lb2);

            var title = new TextBlock { Text = "Результаты", FontWeight = FontWeights.Bold };
            Canvas.SetLeft(title, 70); Canvas.SetTop(title, 230);
            chartCanvas.Children.Add(title);
        }

        /// <summary>
        /// Открывает окно с блок-схемой правильной и ошибочной версий.
        /// </summary>
        void BtnFlowchart_Click(object sender, RoutedEventArgs e)
        {
            var w = new Window
            {
                Title = "Блок-схема — правильная и неправильная",
                Width = 900,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            var img = new System.Windows.Controls.Image
            {
                Source = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri(System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, "flowchart12.png"))),
                Stretch = System.Windows.Media.Stretch.Uniform,
                Margin = new Thickness(10)
            };
            w.Content = img;
            w.Show();
        }

        /// <summary>
        /// Добавляет узел на канвас графа программы.
        /// </summary>
        /// <param name="cv">Канвас для рисования.</param>
        /// <param name="x">Координата X левого верхнего угла.</param>
        /// <param name="y">Координата Y левого верхнего угла.</param>
        /// <param name="w">Ширина узла.</param>
        /// <param name="h">Высота узла.</param>
        /// <param name="text">Текст внутри узла.</param>
        /// <param name="predicate">Если true — узел-предикат (эллипс), иначе прямоугольник.</param>
        void AddNode(Canvas cv, double x, double y, double w, double h,
                     string text, bool predicate)
        {
            if (predicate)
            {
                var el = new Ellipse
                {
                    Width = w,
                    Height = h,
                    Fill = Brushes.Moccasin,
                    Stroke = Brushes.DarkOrange,
                    StrokeThickness = 2
                };
                Canvas.SetLeft(el, x); Canvas.SetTop(el, y);
                cv.Children.Add(el);
            }
            else
            {
                var rc = new Rectangle
                {
                    Width = w,
                    Height = h,
                    Fill = Brushes.LightSteelBlue,
                    Stroke = Brushes.SteelBlue,
                    StrokeThickness = 2
                };
                Canvas.SetLeft(rc, x); Canvas.SetTop(rc, y);
                cv.Children.Add(rc);
            }
            var tb = new TextBlock
            {
                Text = text,
                FontSize = 10,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Width = w - 4
            };
            Canvas.SetLeft(tb, x + 2); Canvas.SetTop(tb, y + h / 2 - 14);
            cv.Children.Add(tb);
        }

        /// <summary>
        /// Рисует направленное ребро между двумя точками на канвасе графа.
        /// </summary>
        /// <param name="cv">Канвас для рисования.</param>
        /// <param name="x1">Координата X начальной точки.</param>
        /// <param name="y1">Координата Y начальной точки.</param>
        /// <param name="x2">Координата X конечной точки.</param>
        /// <param name="y2">Координата Y конечной точки.</param>
        /// <param name="label">Подпись ребра (Да/Нет).</param>
        void AddEdge(Canvas cv, double x1, double y1, double x2, double y2,
                     string label)
        {
            var line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Black,
                StrokeThickness = 1.5
            };
            cv.Children.Add(line);
            if (!string.IsNullOrEmpty(label))
            {
                var tb = new TextBlock
                {
                    Text = label,
                    FontSize = 9,
                    Foreground = Brushes.DarkRed,
                    Background = Brushes.White
                };
                Canvas.SetLeft(tb, (x1 + x2) / 2);
                Canvas.SetTop(tb, (y1 + y2) / 2);
                cv.Children.Add(tb);
            }
        }

        /// <summary>
        /// Открывает окно с управляющим потоковым графом и пояснениями к нему.
        /// </summary>
        void BtnGraph_Click(object sender, RoutedEventArgs e)
        {
            var w = new Window
            {
                Title = "Управляющий потоковый граф — Вариант 6",
                Width = 950,
                Height = 650,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            var grid = new Grid { Margin = new Thickness(15) };
            grid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = new GridLength(1, GridUnitType.Star) });

            var text = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                FontSize = 13,
                Margin = new Thickness(0, 0, 15, 0),
                Text =
                "УПРАВЛЯЮЩИЙ ПОТОКОВЫЙ ГРАФ\n\n" +
                "Узлы: 1—начало, 2—x<0?, 3—b≠0?, 4—y=a·x³+b²+c,\n" +
                "5—x>0?, 6—c≠0?, 7—y=(x−a)/c, 8—y=15x/c, 9—конец\n\n" +
                "V(G) = E − N + 2 = 12 − 9 + 2 = 5\n" +
                "V(G) = p + 1 = 4 + 1 = 5"
            };

            var img = new System.Windows.Controls.Image
            {
                Source = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri(System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, "graph.png"))),
                Stretch = System.Windows.Media.Stretch.Uniform
            };

            Grid.SetColumn(text, 0);
            Grid.SetColumn(img, 1);
            grid.Children.Add(text);
            grid.Children.Add(img);

            var scroll = new ScrollViewer
            {
                Content = grid,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            w.Content = scroll;
            w.Show();
        }

        /// <summary>
        /// Открывает окно сравнения правильной и ошибочной версий функции
        /// по всем пяти базовым путям.
        /// </summary>
        void BtnCompare_Click(object sender, RoutedEventArgs e)
        {
            var cases = new List<(string name, double a, double b, double c, double x)>
            {
                ("Путь 1", 2, 3, 1, -2),
                ("Путь 2", 2, 0, 2, -2),
                ("Путь 3", 2, 3, 2,  4),
                ("Путь 4", 2, 3, 0,  4),
                ("Путь 5", 2, 3, 2,  0),
            };

            var rows = new List<CompareCase>();
            int caught = 0;
            foreach (var c in cases)
            {
                double correct = CalcY(c.x, c.a, c.b, c.c);
                double uncorrect = UncorrectY(c.x, c.a, c.b, c.c);
                bool diff = R(correct) != R(uncorrect);
                if (diff) caught++;
                rows.Add(new CompareCase
                {
                    PathName = c.name,
                    Inputs = "a=" + c.a + ", b=" + c.b + ", c=" + c.c + ", x=" + c.x,
                    Correct = R(correct),
                    Uncorrect = R(uncorrect),
                    Result = diff ? "✘ Расхождение (баг найден)" : "= Совпадает"
                });
            }

            var w = new Window
            {
                Title = "Сравнение с ошибочной версией (&& → ||)",
                Width = 750,
                Height = 420,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            var grid = new DataGrid
            {
                AutoGenerateColumns = false,
                IsReadOnly = true,
                Margin = new Thickness(10),
                ItemsSource = rows
            };
            grid.Columns.Add(new DataGridTextColumn
            { Header = "Путь", Binding = new System.Windows.Data.Binding("PathName"), Width = 70 });
            grid.Columns.Add(new DataGridTextColumn
            { Header = "Входные данные", Binding = new System.Windows.Data.Binding("Inputs"), Width = 200 });
            grid.Columns.Add(new DataGridTextColumn
            { Header = "Правильная", Binding = new System.Windows.Data.Binding("Correct"), Width = 110 });
            grid.Columns.Add(new DataGridTextColumn
            { Header = "Ошибочная", Binding = new System.Windows.Data.Binding("Uncorrect"), Width = 110 });
            grid.Columns.Add(new DataGridTextColumn
            { Header = "Результат", Binding = new System.Windows.Data.Binding("Result"), Width = 180 });

            var panel = new StackPanel();
            panel.Children.Add(new TextBlock
            {
                Text = "Ошибочная версия: && заменено на ||.\n" +
                       "Тесты по базовым путям поймали " + caught + " из 5 расхождений.",
                Margin = new Thickness(10, 10, 10, 0),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 13
            });
            panel.Children.Add(grid);
            w.Content = panel;
            w.Show();
        }

        /// <summary>
        /// Открывает окно с рассчитанными метриками программы:
        /// цикломатическая сложность, Холстед, Чепин, структурная сложность.
        /// </summary>
        void BtnMetrics_Click(object sender, RoutedEventArgs e)
        {
            var w = new Window
            {
                Title = "Метрики программы",
                Width = 650,
                Height = 650,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            var tb = new TextBlock
            {
                Text =
                "1. ЦИКЛОМАТИЧЕСКАЯ СЛОЖНОСТЬ (Маккейб)\n" +
                "N=9, E=12, p=4\n" +
                "V(G) = 12 - 9 + 2 = 5\n\n" +
                "2. МЕТРИКИ ХОЛСТЕДА\n" +
                "n1=13, n2=8, N1=23, N2=23\n" +
                "V≈202.0, D≈18.69, E≈3776.9\n\n" +
                "3. МЕТРИКА ЧЕПИНА\n" +
                "P=1, M=1, C=3, T=0 → Q=12\n\n" +
                "4. СТРУКТУРНАЯ СЛОЖНОСТЬ (Джилб)\n" +
                "S(p) = 4\n\n" +
                "5. ТЕСТИРОВАННОСТЬ\n" +
                "TV = 5/5 × 100% = 100%",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(15),
                FontSize = 13,
                FontFamily = new FontFamily("Consolas")
            };
            var scroll = new ScrollViewer { Content = tb };
            w.Content = scroll;
            w.Show();
        }

        /// <summary>
        /// Открывает окно с условием задания (вариант 6).
        /// </summary>
        private void BtnTask_Click(object sender, RoutedEventArgs e)
        {
            var w = new Window
            {
                Title = "Задание",
                Width = 600,
                Height = 550,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            var tb = new TextBlock
            {
                Text = "Контрольная работа\n" +
       "Управление качеством и тестирование программного обеспечения\n\n" +
       "Цель: закрепление теоретических знаний и практических навыков в разработке программы тестирования методом базового пути.\n\n" +
       "Вариант 6:\n" +
       "y = a·x³ + b² + c,  при x<0, b≠0\n" +
       "y = (x-a)/c,        при x>0, c≠0\n" +
       "y = 15·x/c,         в остальных случаях\n\n" +
       "Приложение должно обеспечивать:\n" +
       "  • ввод исходных данных;\n" +
       "  • расчёт данных;\n" +
       "  • вывод управляющего графа программы;\n" +
       "  • расчёт цикломатической сложности графа;\n" +
       "  • тестирование и вывод данных (метод базового пути);\n" +
       "  • оценку тестированности программы;\n" +
       "  • расчёт метрик Холстеда и Чепина;\n" +
       "  • расчёт метрики структурной сложности.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(15),
                FontSize = 13
            };
            var scroll = new ScrollViewer { Content = tb };
            w.Content = scroll;
            w.Show();
        }
    }
}