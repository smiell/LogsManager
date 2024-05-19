using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LogsManager
{
    public partial class Form1 : Form
    {
        private FileSystemWatcher watcher;
        private string filePath;

        public Form1()
        {
            InitializeComponent();
            ResizeControls();

            // Initialize FileSystemWatcher
            watcher = new FileSystemWatcher();
        }

        private void ResizeControls()
        {
            int margin = 12; // Margin left and right
            int buttonHeight = 23; // Button height

            // Customize "Open text file" button
            button1.Location = new Point(margin, margin);
            button1.Width = 119;
            button1.Height = buttonHeight;

            // Customize "Clear logs" button
            button2.Location = new Point(button1.Right + margin, margin);
            button2.Width = 75;
            button2.Height = buttonHeight;

            // Customize RichTextBox
            richTextBox1.Location = new Point(margin, button1.Bottom + margin);
            richTextBox1.Size = new Size(ClientSize.Width - 2 * margin, ClientSize.Height - richTextBox1.Top - margin);

            // Set how to handle the resize event
            this.Resize += new EventHandler(Form1_Resize);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizeControls();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Create a new OpenFileDialog instance
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Set file filters so that the user can only select text files
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

                // Display the open file dialog box and verify that the user has selected the file
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Save the file path
                        filePath = openFileDialog.FileName;

                        // Read the contents of the selected text file
                        string text = File.ReadAllText(filePath);
                        // Display the read content of the file in a RichTextBox control
                        richTextBox1.Text = text;
                        richTextBox1.SelectionStart = richTextBox1.TextLength;
                        richTextBox1.ScrollToCaret();

                        // Start file watcher
                        StartFileWatcher(filePath);
                    }
                    catch (Exception ex)
                    {
                        // Handle Exceptions
                        MessageBox.Show("Error reading the file: " + ex.Message);
                    }
                }
            }
        }

        private void StartFileWatcher(string path)
        {
            // Set watcher file
            watcher.Path = Path.GetDirectoryName(path);
            watcher.Filter = Path.GetFileName(path);

            // Set option for checking file changes on file
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;

            // Handle file change event
            watcher.Changed += FileChanged;
        }

        // Method handles file change
        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            // Get file, open content and load as richbox string
            string text = File.ReadAllText(filePath);
            Invoke(new Action(() =>
            {
                richTextBox1.Text = text;
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.ScrollToCaret();
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Empty rich box string
            richTextBox1.Text = string.Empty;

            // Empty opened file
            if (!string.IsNullOrEmpty(filePath))
            {
                File.WriteAllText(filePath, string.Empty);
            }
        }
    }
}
