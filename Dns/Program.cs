using System.Diagnostics;
using System.Text;

namespace Dns
{
    public class ConfigModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string DNS1 { get; set; }
        public string DNS2 { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.ResetColor();

            string result = Execute("Get-DnsClientServerAddress");

            Console.WriteLine(result);

            int interfaceIndex = -1;
            Console.ForegroundColor = ConsoleColor.Blue;

            do
            {
                Console.WriteLine("Enter the interface index number");

                string? readLine = Console.ReadLine();

                if (int.TryParse(readLine, out interfaceIndex) == false)
                    interfaceIndex = -1;

            } while (interfaceIndex == -1);

            Console.ResetColor();

            List<ConfigModel> lstMenu =
            [
                new ConfigModel()
                {
                    Id = 0,
                    Title = "Reset"
                },
                .. Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfigModel>>(File.ReadAllText("Config.json")),
            ];

            Console.WriteLine(String.Join(',', lstMenu.Select(x => $"{x.Id}-{x.Title}   ")));

            int menuId = -1;
            Console.ForegroundColor = ConsoleColor.Blue;

            do
            {

                Console.WriteLine("Enter the menu index");

                string? readLine = Console.ReadLine();

                if (int.TryParse(readLine, out menuId) == false)
                    menuId = -1;

                if (lstMenu.Any(x => x.Id == menuId) == false)
                    menuId = -1;

            } while (menuId == -1);

            Console.ResetColor();

            if (menuId == 0)
            {
                result = Execute($"Set-DnsClientServerAddress -InterfaceIndex {interfaceIndex} -ResetServerAddresses");
            }
            else
            {
                ConfigModel config = lstMenu.First(x => x.Id == menuId);

                result = Execute($"Set-DnsClientServerAddress -InterfaceIndex {interfaceIndex} -ServerAddresses ('{config.DNS1}','{config.DNS2}')");
            }

            result = Execute("Get-DnsClientServerAddress");

            Console.WriteLine(result);

            Console.ForegroundColor = ConsoleColor.Green;

            result = Execute("ipconfig /flushDns");

            Console.WriteLine(result);

            Console.WriteLine("**************** Finish ****************");

            Console.ReadKey();
        }

        public static string Execute(string command)
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe",
                    Arguments = command,
                    Verb = "runas",
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                }
            };

            process.Start();

            string result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return result;
        }
    }
}