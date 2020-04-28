namespace ShortcutMaker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ShortcutMaker.CreateShortcutByCom(@"notepad.lnk", @"C:\Windows\System32\notepad.exe", "shell32.dll, 5");
            ShortcutMaker.CreateShortcutByWsh(@"notepad.lnk", @"C:\Windows\System32\notepad.exe", "shell32.dll, 5");
            ShortcutMaker.CreateShortcutByClsid(@"notepad.lnk", @"C:\Windows\System32\notepad.exe", "shell32.dll, 5");
        }
    }
}