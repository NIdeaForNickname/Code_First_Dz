using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utilities;
namespace ADO;

class Program
{
    // ReSharper disable once UnusedParameter.Local
    static void Main(string[] args)
    {
        using (var context = new AppDbContext())
        {
            UserRenderer renderer = new UserRenderer(context);
            while (true){
                Console.Clear();
                Console.WriteLine("1 - Показать структуру тб");
                Console.WriteLine("2 - Вывести все доступные данные с таблицы");
                Console.WriteLine("3 - Новый пользователь");
                Console.WriteLine("4 - Сменить имя пользователя");
                Console.WriteLine("5 - Удалить Пользователя");
                Console.WriteLine("Любое другое - Выйти");
                Console.WriteLine("Выберете действие: ");
                string ch = Console.ReadLine();
                Console.Clear();
                switch (ch)
                {
                    case "1": renderer.ShowStructure(); break;
                    case "2": renderer.ShowUsers(); break;
                    case "3": renderer.NewUser(); break;
                    case "4": renderer.EditUser(); break;
                    case "5": renderer.DeleteUser(); break;
                    default: return;
                }
                Console.WriteLine("\nНажмите любую клавишу что бы продолжить");
                Console.ReadLine();
            }
        }
    }
}