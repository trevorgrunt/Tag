using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Паззл
{
    public partial class MainWindow : Window
    {
        const int gridSize = 4; // Размер поля
        public ObservableCollection<int> Cells { get; private set; } = new ObservableCollection<int>(); // Создаем коллекцию

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Cells; // Связываем данные 
            Fill(); // Появление интерфейса игры
        }

        void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)  // Сообщение о победе
        {
            int val = (int)e.Parameter;
            DoAction(val, (canMove, zero, cur) => {        
                
                if (canMove)
                {
                    Cells[zero] = val;
                    Cells[cur] = 0;
                }
            });

            if (IsCorrect())
                MessageBox.Show("Поздравляю, вы победили!", "Пазл", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) // Возвращает true,
                                                                                   // если команда включена и доступна для использования,
                                                                                   // и false, если команда отключена
        {
            DoAction((int)e.Parameter, (canMove, zero, cur) => e.CanExecute = canMove);
        }

        void Fill() // Заполнение
        {
            var random = new Random();
            var size = gridSize * gridSize;
            var list = Enumerable.Range(0, size).ToList();

            while (list.Count > 0)
            {
                int index = random.Next(0, list.Count - 1);
                Cells.Add(list[index]);
                list.RemoveAt(index);
            }
        }

        public bool IsCorrect() // Проверка на условия победы
        {
            for (int i = 0; i < Cells.Count - 1; ++i)
                if (Cells[i] != i + 1)
                    return false;
            return true;
        }

        public void DoAction(int current, Action<bool, int, int> action) // Само движение
        {
            var zeroIndex = 0;
            var curIndex = 0;
            var canMove = false;

            for (int i = 0; i < Cells.Count; ++i)
            {
                if (Cells[i] == 0)
                    zeroIndex = i;

                if (Cells[i] == current)
                    curIndex = i;
            }

            var diff = curIndex - zeroIndex;
            var column = (curIndex + 1) % gridSize;

            if (diff == gridSize) canMove = true;
            else if (diff == -gridSize) canMove = true;
            else if (diff == 1) canMove = column != 1;
            else if (diff == -1) canMove = column != 0;

            action(canMove, zeroIndex, curIndex);
        }
    }
}
