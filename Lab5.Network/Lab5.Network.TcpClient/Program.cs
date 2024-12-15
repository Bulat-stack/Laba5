using System.Numerics;
using Lab5.Network.Common;
using Lab5.Network.Common.UserApi;

internal class Program
{
    private static object _locker = new object();

    public static async Task Main(string[] args)
    {
        var serverAdress = new Uri("tcp://127.0.0.1:5555");
        var client = new NetTcpClient(serverAdress);
        Console.WriteLine($"Connect to server at {serverAdress}");
        await client.ConnectAsync();

        var userApi = new UserApiClient(client);
        await ManageUsers(userApi);
        client.Dispose();
    }

    private static async Task ManageUsers(IUserApi userApi)
    {
        PrintMenu();

        while(true) {
            var key = Console.ReadKey(true);

            PrintMenu();

            if (key.Key == ConsoleKey.D1) 
            {
                var users = await userApi.GetAllAsync();
                Console.WriteLine($"| Id    |      Имя игрока          | Статус |");
                foreach (var user in users)
                {
                    Console.WriteLine($"| {user.Id,5} | {user.Name,20} | {user.Status,6} |");
                }
            }

            if (key.Key == ConsoleKey.D2) 
            {
                Console.Write("Enter user id: ");
                var userIdString = Console.ReadLine();
                int.TryParse(userIdString, out var userId);
                var user = await userApi.GetAsync(userId);
                //Console.WriteLine($"Id={user?.Id}, Name={user?.Name}, Active={user?.Active}");
                Console.WriteLine($"| {user?.Id,5} | {user?.Name,20} | {user?.Status,6} |");
            }

            if (key.Key == ConsoleKey.D3) 
            {
              
                Console.Write("Напишите ваше имя: ");
                var addName = Console.ReadLine() ?? "empty";
                var addUser = new User(Id: 0,
                    Name: addName,
                    Status: "Не готов"

                );
                var addResult = await userApi.AddAsync(addUser);

                Console.WriteLine(addResult ? "Ok" : "Error");
                
            }
            if (key.Key == ConsoleKey.D4) // Обновление только статуса пользователя
            {
                Console.Write("Введите ID игрока для обновления статуса: ");
                var updateIdString = Console.ReadLine();
                int.TryParse(updateIdString, out var updateId);

                // Получаем текущие данные пользователя
                var existingUser = await userApi.GetAsync(updateId);
                if (existingUser == null)
                {
                    Console.WriteLine("Игрок с таким ID не найден.");
                    continue;
                }


                // Создаем новый объект пользователя с измененным только статусом
                var updatedUser = new User(
                    Id: existingUser.Id,
                    Name: existingUser.Name,
                    Status: "Готов"
                );

                var updateResult = await userApi.UpdateAsync(updateId, updatedUser);

                Console.WriteLine(updateResult ? "Статус игрока обновлен" : "Ошибка при обновлении статуса игрока");
            }
            if (key.Key == ConsoleKey.D5) // Удаление пользователя
            {
                Console.Write("Введите ID игрока для удаления: ");
                var deleteIdString = Console.ReadLine();
                int.TryParse(deleteIdString, out var deleteId);

                var deleteResult = await userApi.DeleteAsync(deleteId);

                Console.WriteLine(deleteResult ? "Игрок удален" : "Ошибка при удалении игрока");
            }

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
        Console.ReadKey();
        //while (Console.Read)
    }

    private static void PrintMenu()
    {
        lock (_locker)
        {
            Console.WriteLine("1 - Вывести всех игроков");
            Console.WriteLine("2 - Показать игрока по id");
            Console.WriteLine("3 - Добавить игрока");
            Console.WriteLine("4 - Игрок онлайн по id");
            Console.WriteLine("5 - Удалить игрока");
            Console.WriteLine("-------");
        }
    }
    
    

}
