using Microsoft.EntityFrameworkCore;

namespace ADO;

public class UserRenderer
{
    AppDbContext db = new AppDbContext();
    
    public UserRenderer(AppDbContext context)
    {
        db = context;
    }

    public void ShowUsers()
    {
        var users = db.Users.ToList();
        Console.WriteLine($"Id \t| Username");
        foreach (var i in users)
        {
            Console.WriteLine($"{i.Id} \t| {i.Username}");
        }
    }

    public void ShowStructure()
    {
       var entityType = db.Model.FindEntityType(typeof(User)); 
       Console.WriteLine($"Имя тб: {entityType.Name}");
       foreach (var i in entityType.GetProperties())
       {
           Console.WriteLine($"Имя: {i.GetColumnName()}, Тип {i.GetColumnType()}");
       }
    }

    public void NewUser()
    {
        Console.Write("Введите имя: ");
        var username = Console.ReadLine();
        Console.Write("Введите пароль: ");
        var password = Console.ReadLine();
        var user  = new User{ Username = username, PasswordHash = password };
        
        if(!db.Users.Any(u => u.Username == username))
        {
            db.Users.Add(new User { Username = username, PasswordHash = password });
            db.SaveChanges();
            Console.WriteLine("Операция прошла успешно: ");
        }
        else
        {
            Console.WriteLine("Операция не прошла успешно: ");
        }
    }

    public void EditUser()
    {
        Console.Write("Введите имя: ");
        var username = Console.ReadLine();
        Console.Write("Введите пароль: ");
        var password = Console.ReadLine();
        
        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null || user?.PasswordHash != password)
        {
            Console.WriteLine("Неправильное имя/пароль");
            return;
        }
        
        Console.Write("Введите новое имя: ");
        string? nm = Console.ReadLine();
        if (nm is { Length: >= 1 } && !db.Users.Any(u => u.Username == nm))
        {
            user!.Username = nm;
            db.SaveChanges();
            Console.WriteLine("Операция прошла успешно");
        }
        else
        {
            Console.WriteLine("Введено не подходящеее имя");
        }
    }

    public void DeleteUser()
    {
        Console.Write("Введите имя: ");
        var username = Console.ReadLine();
        Console.Write("Введите пароль: ");
        var password = Console.ReadLine();
        
        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null || user?.PasswordHash != password) return;
        
        Console.Write("Вы уверены что хотите удалить пользователя? (Д/Н): ");
        string? nm = Console.ReadLine();
        if (nm == "Д")
        {
            db.Users.Remove(user);
            db.SaveChanges();
            Console.WriteLine("Операция прошла успешно");
        }
        else Console.WriteLine("Операция отменена");
    }
}