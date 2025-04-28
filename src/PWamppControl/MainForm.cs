using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PWamppConsole
{
    internal partial class MainForm : Form
    {
        // --- IMPORTANT CONFIGURATION ---
        // Replace these paths and names with YOUR specific installation details!

        // Apache Configuration
        private const string ApacheExecutablePath = @"C:\xampp\apache\bin\httpd.exe"; // FIND YOUR httpd.exe
        private const string ApacheWorkingDirectory = @"C:\xampp\apache\bin";       // Usually the same dir as exe
        private const string ApacheProcessName = "httpd";                           // Check Task Manager

        // MySQL Configuration
        private const string MySqlExecutablePath = @"C:\xampp\mysql\bin\mysqld.exe"; // FIND YOUR mysqld.exe
        private const string MySqlWorkingDirectory = @"C:\xampp\mysql\bin";         // Usually the same dir as exe
        private const string MySqlProcessName = "mysqld";                           // Check Task Manager

        // --- Process Tracking ---
        // Store processes started BY THIS APP to manage them directly
        private Process _apacheProcess;
        private Process _mySqlProcess;


        public MainForm()
        {
            InitializeComponent();
            // Ensure Status labels can accommodate "Running" / "Stopped"
            lblApacheStatus.AutoSize = false;
            lblApacheStatus.Width = 100;
            lblApacheStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblMySqlStatus.AutoSize = false;
            lblMySqlStatus.Width = 100;
            lblMySqlStatus.TextAlign = ContentAlignment.MiddleLeft;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Perform initial status check when the form loads
            CheckAllProcessStatus();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Optional: Try to stop processes started by this app when closing
            // Consider adding a confirmation dialog here
            StopProcessInternal(ref _apacheProcess, ApacheProcessName, lblApacheStatus, btnStartApache, btnStopApache, false); // Stop Apache if we started it
            StopProcessInternal(ref _mySqlProcess, MySqlProcessName, lblMySqlStatus, btnStartMySql, btnStopMySql, false);    // Stop MySQL if we started it
        }

        // --- Status Checking ---

        private void CheckAllProcessStatus()
        {
            CheckProcessStatus(ApacheProcessName, lblApacheStatus, btnStartApache, btnStopApache, ref _apacheProcess);
            CheckProcessStatus(MySqlProcessName, lblMySqlStatus, btnStartMySql, btnStopMySql, ref _mySqlProcess);
        }

        /// <summary>
        /// Checks if a process with the given name is running and updates UI.
        /// Also tries to associate a running process with our internal tracker if it's null.
        /// </summary>
        private void CheckProcessStatus(string processName, Label statusLabel, Button startButton, Button stopButton, ref Process trackedProcess)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);
                bool isRunning = processes.Length > 0;

                if (isRunning)
                {
                    // If we don't have a tracked process or it exited, try to grab one of the running ones
                    if (trackedProcess == null || trackedProcess.HasExited)
                    {
                        trackedProcess = processes.FirstOrDefault(); // Grab the first one found
                        // If we successfully grabbed a process, hook its Exited event
                        if (trackedProcess != null && (trackedProcess.MainWindowHandle == IntPtr.Zero || !trackedProcess.HasExited)) // Extra checks
                        {
                            try
                            { // This might fail if we don't have permissions
                                trackedProcess.EnableRaisingEvents = true;
                                trackedProcess.Exited -= Process_Exited; // Unsubscribe first to prevent duplicates
                                trackedProcess.Exited += Process_Exited;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Failed to attach Exited event handler for {processName}: {ex.Message}");
                                trackedProcess = null; // Cannot reliably track this externally started process
                            }
                        }
                    }
                    UpdateStatusLabel(statusLabel, "Running", Color.Green);
                    startButton.Enabled = false;
                    stopButton.Enabled = true;
                }
                else // Not running
                {
                    // If our tracked process thought it was running, clear it
                    if (trackedProcess != null && !trackedProcess.HasExited)
                    {
                        trackedProcess.Exited -= Process_Exited;
                        trackedProcess.Dispose(); // Release resources
                        trackedProcess = null;
                    }
                    else if (trackedProcess != null && trackedProcess.HasExited)
                    {
                        trackedProcess = null; // Already exited, just clear reference
                    }

                    UpdateStatusLabel(statusLabel, "Stopped", Color.Gray);
                    startButton.Enabled = true;
                    stopButton.Enabled = false;
                }

                // Clean up process list array
                foreach (var p in processes) { p.Dispose(); }
            }
            catch (Exception ex)
            {
                UpdateStatusLabel(statusLabel, "Error Checking", Color.Red);
                startButton.Enabled = false;
                stopButton.Enabled = false;
                ShowStatusStripMessage($"Error checking status for {processName}: {ex.Message}");
                Debug.WriteLine($"Error checking process {processName}: {ex.Message}");
            }
        }

        // --- Process Start Logic ---

        private void btnStartApache_Click(object sender, EventArgs e)
        {
            StartProcess(ApacheExecutablePath, ApacheWorkingDirectory, ApacheProcessName, ref _apacheProcess, lblApacheStatus, btnStartApache, btnStopApache);
        }

        private void btnStartMySql_Click(object sender, EventArgs e)
        {
            StartProcess(MySqlExecutablePath, MySqlWorkingDirectory, MySqlProcessName, ref _mySqlProcess, lblMySqlStatus, btnStartMySql, btnStopMySql);
        }

        private void StartProcess(string exePath, string workingDir, string processName, ref Process trackedProcess, Label statusLabel, Button startButton, Button stopButton)
        {
            // Re-check if already running just before starting
            if (Process.GetProcessesByName(processName).Length > 0)
            {
                ShowStatusStripMessage($"{processName} is already running.");
                CheckProcessStatus(processName, statusLabel, startButton, stopButton, ref trackedProcess); // Refresh UI
                return;
            }

            if (!File.Exists(exePath))
            {
                MessageBox.Show($"Executable not found:\n{exePath}\nPlease check the path in the application code.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusLabel(statusLabel, "Exe Not Found", Color.Red);
                return;
            }
            if (!Directory.Exists(workingDir))
            {
                MessageBox.Show($"Working directory not found:\n{workingDir}\nPlease check the path in the application code.", "Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusLabel(statusLabel, "Dir Not Found", Color.Red);
                return;
            }


            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = workingDir,
                WindowStyle = ProcessWindowStyle.Hidden,   // Hide console window
                CreateNoWindow = true,                   // Don't create a window
                UseShellExecute = false,                 // Required for CreateNoWindow/redirect
                // Optional: Redirect output (for debugging startup)
                // RedirectStandardOutput = true,
                // RedirectStandardError = true,
            };

            try
            {
                trackedProcess = new Process { StartInfo = startInfo };

                // Optional: Capture output
                // trackedProcess.OutputDataReceived += (s, args) => Debug.WriteLine($"{processName} OUT: {args.Data}");
                // trackedProcess.ErrorDataReceived += (s, args) => Debug.WriteLine($"{processName} ERR: {args.Data}");

                trackedProcess.EnableRaisingEvents = true; // To detect when it closes unexpectedly
                trackedProcess.Exited += Process_Exited;   // Hook the exited event

                UpdateStatusLabel(statusLabel, "Starting...", Color.Orange);
                startButton.Enabled = false;
                stopButton.Enabled = false;
                Application.DoEvents(); // Refresh UI

                trackedProcess.Start();

                // Optional: Begin reading output streams if redirected
                // trackedProcess.BeginOutputReadLine();
                // trackedProcess.BeginErrorReadLine();

                // Give it a moment to potentially crash immediately
                System.Threading.Thread.Sleep(1000); // Wait 1 second

                if (trackedProcess.HasExited)
                {
                    ShowStatusStripMessage($"{processName} failed to start or exited quickly.");
                    trackedProcess = null; // Clear tracking variable
                    CheckProcessStatus(processName, statusLabel, startButton, stopButton, ref trackedProcess); // Update UI based on actual state
                }
                else
                {
                    ShowStatusStripMessage($"{processName} started successfully.");
                    UpdateStatusLabel(statusLabel, "Running", Color.Green);
                    startButton.Enabled = false;
                    stopButton.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start {processName}:\n{ex.Message}", "Start Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusLabel(statusLabel, "Start Error", Color.Red);
                ShowStatusStripMessage($"Error starting {processName}: {ex.Message}");
                trackedProcess?.Dispose(); // Clean up if partial start occurred
                trackedProcess = null;
                // Re-check status in case it did start but we had an error later
                CheckProcessStatus(processName, statusLabel, startButton, stopButton, ref trackedProcess);
            }
        }


        // --- Process Stop Logic ---

        private void btnStopApache_Click(object sender, EventArgs e)
        {
            StopProcessInternal(ref _apacheProcess, ApacheProcessName, lblApacheStatus, btnStartApache, btnStopApache, true);
        }

        private void btnStopMySql_Click(object sender, EventArgs e)
        {
            StopProcessInternal(ref _mySqlProcess, MySqlProcessName, lblMySqlStatus, btnStartMySql, btnStopMySql, true);
        }

        /// <summary>
        /// Stops a process, prioritizing the tracked instance, then by name.
        /// </summary>
        private void StopProcessInternal(ref Process trackedProcess, string processName, Label statusLabel, Button startButton, Button stopButton, bool showMessages)
        {
            UpdateStatusLabel(statusLabel, "Stopping...", Color.Orange);
            startButton.Enabled = false;
            stopButton.Enabled = false;
            Application.DoEvents(); // Refresh UI

            bool stopped = false;
            Exception lastError = null;

            // Priority 1: Stop the tracked process (if it exists and is running)
            if (trackedProcess != null && !trackedProcess.HasExited)
            {
                try
                {
                    // Unhook event first to avoid race condition if Kill is fast
                    trackedProcess.Exited -= Process_Exited;
                    trackedProcess.Kill();
                    trackedProcess.WaitForExit(5000); // Wait up to 5 seconds for it to terminate
                    stopped = trackedProcess.HasExited;
                    if (stopped)
                    {
                        trackedProcess.Dispose(); // Release resources
                        trackedProcess = null;
                        if (showMessages) ShowStatusStripMessage($"{processName} (tracked) stopped.");
                    }
                    else
                    {
                        if (showMessages) ShowStatusStripMessage($"{processName} (tracked) did not exit after Kill().");
                    }

                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is System.ComponentModel.Win32Exception)
                {
                    // Ignore "process already exited" or "access denied" here, maybe it died or we lost rights.
                    Debug.WriteLine($"Exception stopping tracked {processName} (possibly already exited): {ex.Message}");
                    trackedProcess = null; // Assume it's gone or we can't manage it
                    lastError = ex;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected exception stopping tracked {processName}: {ex.Message}");
                    lastError = ex;
                    // Don't nullify trackedProcess here, maybe it's recoverable? Unlikely.
                    trackedProcess = null;
                }
            }
            else if (trackedProcess != null && trackedProcess.HasExited)
            {
                // It already exited, just clean up
                trackedProcess.Dispose();
                trackedProcess = null;
            }


            // Priority 2: If not stopped via tracked process, find and kill any process by name
            if (!stopped)
            {
                try
                {
                    var processes = Process.GetProcessesByName(processName);
                    if (processes.Length == 0 && lastError == null) // Only show 'not running' if no prior errors occurred
                    {
                        if (showMessages) ShowStatusStripMessage($"{processName} was not running.");
                        stopped = true; // Effectively stopped because it wasn't running
                    }

                    foreach (var p in processes)
                    {
                        try
                        {
                            // Ensure we don't try to kill the already handled tracked process again if it failed above
                            if (trackedProcess != null && trackedProcess.Id == p.Id)
                            {
                                p.Dispose();
                                continue;
                            }

                            p.Kill();
                            p.WaitForExit(5000); // Wait briefly
                            stopped = true; // At least one was killed
                            if (showMessages) ShowStatusStripMessage($"Stopped {processName} (PID: {p.Id}).");
                        }
                        catch (Exception ex) when (ex is InvalidOperationException || ex is System.ComponentModel.Win32Exception)
                        {
                            // Ignore errors like "already exited" or "access denied" for individual processes
                            Debug.WriteLine($"Exception killing {processName} PID {p.Id} (possibly already exited): {ex.Message}");
                            lastError = ex; // Record the last error encountered
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Unexpected error killing {processName} PID {p.Id}: {ex.Message}");
                            lastError = ex;
                        }
                        finally { p.Dispose(); } // Dispose each process handle
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error getting process list for {processName}: {ex.Message}");
                    lastError = ex;
                }
            }


            // Final UI Update
            if (stopped && lastError == null) // Successfully stopped or wasn't running initially
            {
                UpdateStatusLabel(statusLabel, "Stopped", Color.Gray);
                startButton.Enabled = true;
                stopButton.Enabled = false;
            }
            else if (lastError != null) // An error occurred during stop
            {
                UpdateStatusLabel(statusLabel, "Stop Error", Color.Red);
                if (showMessages) MessageBox.Show($"Error stopping {processName}:\n{lastError.Message}", "Stop Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Re-check status to reflect reality as best as possible
                CheckProcessStatus(processName, statusLabel, startButton, stopButton, ref trackedProcess);
            }
            else
            { // This case means stop was attempted but failed without throwing exception (e.g., WaitForExit timeout)
                UpdateStatusLabel(statusLabel, "Stop Failed?", Color.Orange);
                if (showMessages) ShowStatusStripMessage($"{processName} may not have stopped correctly.");
                CheckProcessStatus(processName, statusLabel, startButton, stopButton, ref trackedProcess);
            }
        }


        // --- Event Handlers ---

        /// <summary>
        /// Handles the Exited event for processes started by this application.
        /// Updates the UI to reflect that the process stopped unexpectedly.
        /// IMPORTANT: This runs on a different thread, so UI updates must use Invoke.
        /// </summary>
        private void Process_Exited(object sender, EventArgs e)
        {
            Process exitedProcess = sender as Process;
            if (exitedProcess == null) return;

            string processName = "?";
            Label statusLabel = null;
            Button startButton = null;
            Button stopButton = null;
            ref Process trackedProcessRef = ref _apacheProcess; // Dummy ref

            // Determine which process exited and clear the tracking variable
            // Use Invoke to safely interact with UI controls from this background thread
            if (exitedProcess == _apacheProcess)
            {
                processName = ApacheProcessName;
                statusLabel = lblApacheStatus;
                startButton = btnStartApache;
                stopButton = btnStopApache;
                this.Invoke(new Action(() => _apacheProcess = null)); // Clear tracking variable on UI thread
                trackedProcessRef = ref _apacheProcess;
            }
            else if (exitedProcess == _mySqlProcess)
            {
                processName = MySqlProcessName;
                statusLabel = lblMySqlStatus;
                startButton = btnStartMySql;
                stopButton = btnStopMySql;
                this.Invoke(new Action(() => _mySqlProcess = null)); // Clear tracking variable on UI thread
                trackedProcessRef = ref _mySqlProcess;
            }

            // Update the UI via Invoke
            if (statusLabel != null)
            {
                this.Invoke(new Action(() =>
                {
                    ShowStatusStripMessage($"{processName} exited unexpectedly.");
                    // Re-run the check to ensure UI is accurate
                    // Pass null explicitly as the tracked process is gone
                    Process nullProcess = null;
                    CheckProcessStatus(processName, statusLabel, startButton, stopButton, ref nullProcess);
                    // UpdateStatusLabel(statusLabel, "Stopped (Exited)", Color.DarkOrange);
                    // startButton.Enabled = true;
                    // stopButton.Enabled = false;
                }));
            }

            // Clean up the exited process object
            try { exitedProcess.Dispose(); } catch { /* Ignore disposal errors */ }
        }


        // --- UI Update Helpers ---

        private void UpdateStatusLabel(Label label, string text, Color? color = null)
        {
            // Ensure updates happen on the UI thread
            if (label.InvokeRequired)
            {
                label.Invoke(new Action(() => UpdateStatusLabel(label, text, color)));
                return;
            }

            label.Text = text;
            if (color.HasValue)
            {
                label.ForeColor = color.Value;
            }
            else
            {
                label.ForeColor = SystemColors.ControlText; // Default color
            }
        }

        private void ShowStatusStripMessage(string message)
        {
            // Check if controls exist (might be null if called during form closing)
            if (statusStrip1 == null || toolStripStatusLabel1 == null) return;

            // Ensure updates happen on the UI thread
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new Action(() => ShowStatusStripMessage(message)));
                return;
            }
            toolStripStatusLabel1.Text = $"[{DateTime.Now:HH:mm:ss}] {message}";
        }
    }
}

