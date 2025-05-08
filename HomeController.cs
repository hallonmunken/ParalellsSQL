 private string GetConnectionString(string databaseFileName)
 {
     var fullDatabasePath = Path.Combine(_environment.ContentRootPath, databaseFileName);

     bool isParallels = IsRunningInParallels();
     bool mdfExists = System.IO.File.Exists(fullDatabasePath);

     if (!isParallels && _environment.IsDevelopment() && mdfExists)
     {
         Console.WriteLine($"[DB] Using LocalDB at: {fullDatabasePath}");
         return @$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={fullDatabasePath};Integrated Security=True;Connect Timeout=15";
     }
     else
     {
         Console.WriteLine("[DB] Using Docker SQL Server at 0.0.0.0:1433 (Database: TeoriDB)");
         return @"Server=0.0.0.0,1433;Initial Catalog=TeoriDB;User Id=username;Password=Password;Encrypt=False;";
     }
 }

 private bool IsRunningInParallels()
 {
     try
     {
         using var searcher = new ManagementObjectSearcher("SELECT Manufacturer FROM Win32_ComputerSystem");
         foreach (var obj in searcher.Get())
         {
             string manufacturer = obj["Manufacturer"]?.ToString() ?? "";
             if (manufacturer.ToLower().Contains("parallels"))
             {
                 Console.WriteLine("[ENV] Detected Parallels VM.");
                 return true;
             }
         }
     }
     catch
     {
         Console.WriteLine("[ENV] Unable to detect system manufacturer.");
     }

     return false;
 }
