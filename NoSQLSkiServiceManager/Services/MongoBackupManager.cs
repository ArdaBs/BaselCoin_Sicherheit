namespace NoSQLSkiServiceManager.Services
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Timers;

    /// <summary>
    /// Manages periodic backups for MongoDB using a timer.
    /// </summary>
    /// <remarks>
    /// Requires mongodump.exe to be accessible and a designated backup directory.
    /// Backups are scheduled to occur every 24 hours.
    /// </remarks>
    public class MongoBackupManager : IHostedService
    {
        private readonly Timer _backupTimer;
        private readonly string _toolsDirectory;
        private readonly string _databaseName;
        private readonly string _backupDirectory;

        /// <summary>
        /// Initializes a new instance of the MongoBackupManager class.
        /// </summary>
        /// <param name="databaseName">The name of the database to backup.</param>
        /// <param name="toolsDirectory">The directory where MongoDB tools are located.</param>
        /// <param name="backupDirectory">The directory to store backup files.</param>
        public MongoBackupManager(string databaseName, string toolsDirectory, string backupDirectory)
        {
            _databaseName = databaseName;
            _toolsDirectory = toolsDirectory;
            _backupDirectory = backupDirectory;

            // 24h timer for backup, time is in ms
            _backupTimer = new Timer(86400000);
            _backupTimer.Elapsed += OnBackupTimerElapsed;
            _backupTimer.AutoReset = true;
        }

        /// <summary>
        /// Starts the backup timer.
        /// </summary>
        public void Start()
        {
            _backupTimer.Start();
        }

        /// <summary>
        /// Stops the backup timer.
        /// </summary>
        public void Stop()
        {
            _backupTimer.Stop();
        }

        /// <summary>
        /// Starts the backup service, initiating the timer.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MongoBackupManager wird gestartet...");
            Start();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the backup service and disables the timer.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MongoBackupManager wird gestoppt...");
            Stop();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes a backup operation when the timer elapses.
        /// </summary>
        private void OnBackupTimerElapsed(object sender, ElapsedEventArgs e)
        {
            CreateBackup();
        }

        /// <summary>
        /// Creates a backup of the MongoDB database.
        /// </summary>
        /// <remarks>
        /// The backup is compressed and saved to the backup directory with a timestamp.
        /// </remarks>
        private void CreateBackup()
        {
            var mongodumpPath = GetToolPath("mongodump.exe");
            var backupFolderPath = GetBackupPath();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var backupFilePath = Path.Combine(backupFolderPath, $"backup-{timestamp}.gz");

            if (!File.Exists(mongodumpPath))
            {
                Console.WriteLine($"Fehler: 'mongodump.exe' wurde nicht gefunden im Verzeichnis: {mongodumpPath}");
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = mongodumpPath,
                Arguments = $"--archive=\"{backupFilePath}\" --gzip",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            try
            {
                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    var result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    Console.WriteLine("Backup erfolgreich abgeschlossen.");
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Ausführen von mongodump: {ex.Message}");
            }
        }

        /// <summary>
        /// Constructs the full path to a specified tool within the tools directory.
        /// </summary>
        /// <param name="toolName">The name of the tool to locate.</param>
        /// <returns>Full path to the specified tool.</returns>
        private string GetToolPath(string toolName)
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;

            var relativeToolPath = Path.Combine("..", "..", "..", "MongoTools", toolName);

            var toolPath = Path.GetFullPath(Path.Combine(binPath, relativeToolPath));

            return toolPath;
        }

        /// <summary>
        /// Determines the full path to the backup directory, creating it if it does not exist.
        /// </summary>
        /// <returns>Full path to the backup directory.</returns>
        private string GetBackupPath()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;

            var relativeBackupPath = Path.Combine("..", "..", "..", "MongoDbBackups");

            var backupPath = Path.GetFullPath(Path.Combine(binPath, relativeBackupPath));

            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }

            return backupPath;
        }


    }

}
