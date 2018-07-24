/*
 * Обращение к базе данных испольнительных производств Федеральной службы судебных приставов
 * на предмет поиска сведений об ИП по номеру ИП или наименованию юридического лица,
 * являющегося фигурантом ИП
 */

using System;
using System.Text;
using System.Configuration;
using System.Threading;
using System.IO;
using CheckFSSPRUS.Helpers.HelperWebRequest;

namespace CheckFSSPRUS
{
    class Program
    {
        static string _Task;
        static void Main(string[] args)
        {
            string _token = ConfigurationManager.AppSettings["_token"]; //Токен для запроса
            string _region;     //Регион исполнительного производства
            string _name;       //Наименование искомой организации
            string _IP;         //Номер исполнительного производства
            int _typeSearch;    //Критерий поиска ИП
            int _codeStatus;    //Код статуса запроса
            int i = 0;          //Техническая переменная для отображения количества истекшего времени

            while (true)
            {
                //Выбираем критерий поиска
                do
                {
                    Console.WriteLine("Choice type of search: \n1 - Search by name\n2 - Search by IP");
                    _typeSearch = Convert.ToInt16(Console.ReadLine());
                    if (_typeSearch < 1 || _typeSearch > 2)
                    {
                        Console.WriteLine("Incorrect choice action!");
                    }
                }
                while (_typeSearch < 1 || _typeSearch > 2);

                //Вводим регион ИП
                Console.Write("Input region: ");
                _region = Console.ReadLine();

                HelperWebRequest helperWebRequest = new HelperWebRequest();

                //Запрос к API FSSPRUS
                switch (_typeSearch)
                {
                    case 1:
                        Console.Write("Input name: ");
                        _name = Console.ReadLine();

                        FsspResponseLegal fsspLegal = helperWebRequest.WebRequestLegal(_token, _region, _name);
                        if (fsspLegal.Code == 0)
                        {
                            Console.WriteLine("Status: {0}", fsspLegal.Status);
                            Console.WriteLine("Request task: {0}", fsspLegal.Response.Task);
                            _Task = fsspLegal.Response.Task;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: {0} - {1}", fsspLegal.Code, fsspLegal.Exception);
                            Console.ResetColor();
                        }
                        break;
                    case 2:
                        Console.Write("Input IP: ");
                        _IP = Console.ReadLine();

                        FsspResponseIP fsspIP = helperWebRequest.WebRequestIP(_token, _IP);
                        if (fsspIP.Code == 0)
                        {
                            Console.WriteLine("Status: {0}", fsspIP.Status);
                            Console.WriteLine("Request task: {0}", fsspIP.Response.Task);
                            _Task = fsspIP.Response.Task;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: {0} - {1}", fsspIP.Code, fsspIP.Exception);
                            Console.ResetColor();
                        }
                        break;
                    default:
                        _Task = "";
                        break;
                }

                //Отрисовка процесса выполнения запроса
                do
                {
                    FsspResponseStatus fsspStatus = helperWebRequest.WebRequestStatus(_token, _Task);
                    _codeStatus = fsspStatus.Response.Status;
                    Console.WriteLine("Progress: {0}", fsspStatus.Response.Progress);
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    switch (_codeStatus)
                    {
                        case 0:
                            Console.WriteLine("0 — обработка завершена, с помощью метода /result можно получить результаты\r\n");
                            break;
                        case 1:
                            Console.Write("1 — обработка начата, с помощью метода /result можно получить частичные результаты группового запроса");
                            Thread.Sleep(1000);
                            break;
                        case 2:
                            Console.Write("2 — обработка не начиналась, запрос находится в очереди");
                            Thread.Sleep(1000);
                            break;
                        case 3:
                            Console.WriteLine("3 — обработка завершена, имели место ошибки, с помощью метода /result можно получить частичные результаты\r\n");
                            break;
                    }
                    Console.WriteLine("\r\n{0} sec left", i++);
                    Console.SetCursorPosition(0, Console.CursorTop - 2);
                }
                while (_codeStatus > 0);

                //Вывод результата запроса
                FsspResponseResult fsspResult = helperWebRequest.WebRequestResult(_token, _Task);
                foreach (var itemResult1 in fsspResult.Response.Result)
                {
                    if (itemResult1.Result.Count != 0)
                    {
                        Console.WriteLine("Status: {0}", itemResult1.Status);

                        foreach (var itemResult2 in itemResult1.Result)
                        {
                            Console.WriteLine("Name: {0}", itemResult2.Name);
                            Console.WriteLine("Exe_production: {0}", itemResult2.Exe_production);
                            Console.WriteLine("Details: {0}", itemResult2.Details);
                            Console.WriteLine("Subject: {0}", itemResult2.Subject);
                            Console.WriteLine("Department: {0}", itemResult2.Department);
                            Console.WriteLine("Bailiff: {0}", itemResult2.Bailiff);
                            Console.WriteLine("Ip_end: {0}", itemResult2.Ip_end);
                            Console.WriteLine("---------------");

                            File.AppendAllText(@"LogResultIP.csv", DateTime.Now + ";" + itemResult2.Name + ";" + itemResult2.Exe_production + ";" + itemResult2.Details + ";" + itemResult2.Subject + ";" + itemResult2.Department + ";" + itemResult2.Bailiff + ";" + itemResult2.Ip_end + ";" + Environment.NewLine, Encoding.Default);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Data is not found");
                    }
                }

                //Визуальный разграничитель
                Console.WriteLine("================");
            }
        }
    }
}
