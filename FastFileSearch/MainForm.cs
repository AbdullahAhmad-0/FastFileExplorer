using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace FastFileSearch
{
    public partial class MainForm : Form
    {
        private ConcurrentBag<FileInfo> allFiles;
        private ConcurrentBag<DirectoryInfo> allDirectories;
        private BackgroundWorker searchWorker;
        private bool isSearching = false;
        private string currentSearchTerm = "";
        private bool matchCase = false;
        private bool matchWholeWord = false;
        private bool matchPath = false;
        private bool matchDiacritics = false;
        private bool enableRegex = false;
        private string currentFilter = "Everything";

        public MainForm()
        {
            InitializeComponent();
            InitializeFileSearch();
        }

        private void InitializeFileSearch()
        {
            allFiles = new ConcurrentBag<FileInfo>();
            allDirectories = new ConcurrentBag<DirectoryInfo>();

            // Initialize background worker for file searching
            searchWorker = new BackgroundWorker();
            searchWorker.WorkerReportsProgress = true;
            searchWorker.WorkerSupportsCancellation = true;
            searchWorker.DoWork += SearchWorker_DoWork;
            searchWorker.ProgressChanged += SearchWorker_ProgressChanged;
            searchWorker.RunWorkerCompleted += SearchWorker_RunWorkerCompleted;

            // Set up ListView
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.MultiSelect = true;
            listView1.SmallImageList = imageList1;
            listView1.LargeImageList = imageList1;

            // Add columns
            listView1.Columns.Add("Name", 200);
            listView1.Columns.Add("Path", 300);
            listView1.Columns.Add("Date Modified", 120);
            listView1.Columns.Add("Type", 100);
            listView1.Columns.Add("Size", 80);

            // Load system icons
            LoadSystemIcons();
        }

        private void LoadSystemIcons()
        {
            try
            {
                imageList1.ImageSize = new Size(16, 16);
                imageList1.ColorDepth = ColorDepth.Depth32Bit;

                // Add file type icons
                imageList1.Images.Add("folder", SystemIcons.Exclamation.ToBitmap());
                imageList1.Images.Add("file", SystemIcons.Application.ToBitmap());
                imageList1.Images.Add("txt", SystemIcons.Information.ToBitmap());
                imageList1.Images.Add("doc", SystemIcons.Application.ToBitmap());
                imageList1.Images.Add("pdf", SystemIcons.Application.ToBitmap());
                imageList1.Images.Add("exe", SystemIcons.Application.ToBitmap());
                imageList1.Images.Add("jpg", SystemIcons.Information.ToBitmap());
                imageList1.Images.Add("png", SystemIcons.Information.ToBitmap());
                imageList1.Images.Add("gif", SystemIcons.Information.ToBitmap());
                imageList1.Images.Add("bmp", SystemIcons.Information.ToBitmap());
                imageList1.Images.Add("mp3", SystemIcons.Asterisk.ToBitmap());
                imageList1.Images.Add("wav", SystemIcons.Asterisk.ToBitmap());
                imageList1.Images.Add("mp4", SystemIcons.Asterisk.ToBitmap());
                imageList1.Images.Add("avi", SystemIcons.Asterisk.ToBitmap());
                imageList1.Images.Add("zip", SystemIcons.Warning.ToBitmap());
                imageList1.Images.Add("rar", SystemIcons.Warning.ToBitmap());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading icons: {ex.Message}");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            statusLabel.Text = "Ready - Enter search term to begin";
            itemCountLabel.Text = "0 items";

            // Start indexing files in background
            if (!searchWorker.IsBusy)
            {
                statusLabel.Text = "Indexing files...";
                searchWorker.RunWorkerAsync("index");
            }
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text.Length >= 2)
            {
                currentSearchTerm = searchBox.Text;
                PerformSearch();
            }
            else if (searchBox.Text.Length == 0)
            {
                listView1.Items.Clear();
                itemCountLabel.Text = "0 items";
                statusLabel.Text = "Ready";
            }
        }

        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (searchBox.Text.Length > 0)
                {
                    currentSearchTerm = searchBox.Text;
                    PerformSearch();
                }
            }
        }

        private void PerformSearch()
        {
            if (isSearching) return;
            if (string.IsNullOrEmpty(currentSearchTerm)) return;

            listView1.Items.Clear();
            var searchTerm = matchCase ? currentSearchTerm : currentSearchTerm.ToLower();
            var results = new List<ListViewItem>();

            try
            {
                // Parallel search for better performance
                var fileResults = new ConcurrentBag<ListViewItem>();
                var dirResults = new ConcurrentBag<ListViewItem>();

                // Search files in parallel
                Parallel.ForEach(allFiles, file =>
                {
                    if (IsMatch(file.Name, file.FullName, searchTerm) && PassesFilter(file))
                    {
                        var item = CreateListViewItem(file);
                        fileResults.Add(item);
                    }
                });

                // Search directories in parallel
                Parallel.ForEach(allDirectories, dir =>
                {
                    if (IsMatch(dir.Name, dir.FullName, searchTerm) && PassesFilter(dir))
                    {
                        var item = CreateListViewItem(dir);
                        dirResults.Add(item);
                    }
                });

                // Combine results
                results.AddRange(fileResults);
                results.AddRange(dirResults);

                // Sort results by name
                results.Sort((x, y) => string.Compare(x.Text, y.Text, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search error: {ex.Message}");
                return;
            }

            // Add results to ListView
            listView1.Items.AddRange(results.ToArray());
            itemCountLabel.Text = $"{results.Count} items";
            statusLabel.Text = $"Found {results.Count} items matching '{currentSearchTerm}'";
        }

        private bool IsMatch(string fileName, string fullPath, string searchTerm)
        {
            string searchIn = matchPath ? fullPath : fileName;

            if (!matchCase)
                searchIn = searchIn.ToLower();

            if (enableRegex)
            {
                try
                {
                    var options = matchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                    return Regex.IsMatch(searchIn, searchTerm, options);
                }
                catch
                {
                    return false;
                }
            }

            if (matchWholeWord)
            {
                var words = searchIn.Split(' ', '\t', '.', '-', '_');
                return words.Any(word => word.Equals(searchTerm, matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));
            }

            return searchIn.Contains(searchTerm);
        }

        private bool PassesFilter(FileInfo file)
        {
            switch (currentFilter)
            {
                case "Everything": return true;
                case "Audio": return IsAudioFile(file.Extension);
                case "Compressed": return IsCompressedFile(file.Extension);
                case "Document": return IsDocumentFile(file.Extension);
                case "Executable": return IsExecutableFile(file.Extension);
                case "Picture": return IsPictureFile(file.Extension);
                case "Video": return IsVideoFile(file.Extension);
                default: return true;
            }
        }

        private bool PassesFilter(DirectoryInfo dir)
        {
            return currentFilter == "Everything" || currentFilter == "Folder";
        }

        private bool IsAudioFile(string ext)
        {
            string[] audioExts = { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a" };
            return audioExts.Contains(ext.ToLower());
        }

        private bool IsCompressedFile(string ext)
        {
            string[] compressedExts = { ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".xz" };
            return compressedExts.Contains(ext.ToLower());
        }

        private bool IsDocumentFile(string ext)
        {
            string[] docExts = { ".doc", ".docx", ".pdf", ".txt", ".rtf", ".odt", ".xls", ".xlsx", ".ppt", ".pptx" };
            return docExts.Contains(ext.ToLower());
        }

        private bool IsExecutableFile(string ext)
        {
            string[] execExts = { ".exe", ".msi", ".bat", ".cmd", ".com", ".scr" };
            return execExts.Contains(ext.ToLower());
        }

        private bool IsPictureFile(string ext)
        {
            string[] picExts = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".ico", ".webp" };
            return picExts.Contains(ext.ToLower());
        }

        private bool IsVideoFile(string ext)
        {
            string[] videoExts = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm" };
            return videoExts.Contains(ext.ToLower());
        }

        private ListViewItem CreateListViewItem(FileInfo file)
        {
            var item = new ListViewItem(file.Name);
            item.SubItems.Add(file.DirectoryName);
            item.SubItems.Add(file.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
            item.SubItems.Add(file.Extension.ToUpper() + " File");
            item.SubItems.Add(FormatFileSize(file.Length));
            item.Tag = file;
            item.ImageKey = GetIconKey(file.Extension);
            return item;
        }

        private ListViewItem CreateListViewItem(DirectoryInfo dir)
        {
            var item = new ListViewItem(dir.Name);
            item.SubItems.Add(dir.Parent?.FullName ?? "");
            item.SubItems.Add(dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
            item.SubItems.Add("Folder");
            item.SubItems.Add("");
            item.Tag = dir;
            item.ImageKey = "folder";
            return item;
        }

        private string GetIconKey(string extension)
        {
            switch (extension.ToLower())
            {
                case ".txt": return "txt";
                case ".doc":
                case ".docx": return "doc";
                case ".pdf": return "pdf";
                case ".exe": return "exe";
                case ".jpg":
                case ".jpeg": return "jpg";
                case ".png": return "png";
                case ".gif": return "gif";
                case ".bmp": return "bmp";
                case ".mp3": return "mp3";
                case ".wav": return "wav";
                case ".mp4": return "mp4";
                case ".avi": return "avi";
                case ".zip": return "zip";
                case ".rar": return "rar";
                default: return "file";
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void SearchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var command = e.Argument as string;

            if (command == "index")
            {
                IndexFiles();
            }
        }

        private void IndexFiles()
        {
            try
            {
                var drives = DriveInfo.GetDrives().Where(d => d.IsReady).ToList();

                Parallel.ForEach(drives, drive =>
                {
                    try
                    {
                        IndexDirectory(drive.RootDirectory);
                    }
                    catch (Exception)
                    {
                        // Skip drives that can't be accessed
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error indexing files: {ex.Message}");
            }
        }

        private void IndexDirectory(DirectoryInfo dir)
        {
            try
            {
                if (searchWorker.CancellationPending) return;

                // Skip system and hidden directories for performance
                if ((dir.Attributes & FileAttributes.System) == FileAttributes.System ||
                    (dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    return;

                // Add directory to list
                allDirectories.Add(dir);

                // Add files in this directory
                try
                {
                    var files = dir.GetFiles();
                    foreach (var file in files)
                    {
                        allFiles.Add(file);
                    }
                }
                catch (Exception) { }

                // Recursively index subdirectories
                try
                {
                    var subDirs = dir.GetDirectories();
                    Parallel.ForEach(subDirs, subDir =>
                    {
                        // Skip some system directories but allow more access
                        if (subDir.Name.StartsWith("$") ||
                            subDir.Name == "System Volume Information" ||
                            subDir.Name == "Recovery")
                            return;

                        IndexDirectory(subDir);
                    });
                }
                catch (Exception) { }
            }
            catch (Exception)
            {
                // Skip directories we can't access
            }
        }

        private void matchCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            matchCase = !matchCase;
            matchCaseToolStripMenuItem.Checked = matchCase;
            if (!string.IsNullOrEmpty(currentSearchTerm))
                PerformSearch();
        }

        private void matchWholeWordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            matchWholeWord = !matchWholeWord;
            matchWholeWordToolStripMenuItem.Checked = matchWholeWord;
            if (!string.IsNullOrEmpty(currentSearchTerm))
                PerformSearch();
        }

        private void matchPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            matchPath = !matchPath;
            matchPathToolStripMenuItem.Checked = matchPath;
            if (!string.IsNullOrEmpty(currentSearchTerm))
                PerformSearch();
        }

        private void matchDiacriticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            matchDiacritics = !matchDiacritics;
            matchDiacriticsToolStripMenuItem.Checked = matchDiacritics;
            if (!string.IsNullOrEmpty(currentSearchTerm))
                PerformSearch();
        }

        private void enableRegexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableRegex = !enableRegex;
            enableRegexToolStripMenuItem.Checked = enableRegex;
            if (!string.IsNullOrEmpty(currentSearchTerm))
                PerformSearch();
        }



        private void SearchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update progress if needed
        }

        private void SearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isSearching = false;
            statusLabel.Text = $"Ready - Indexed {allFiles.Count} files and {allDirectories.Count} directories";
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                OpenSelectedItem();
            }
        }

        private void OpenSelectedItem()
        {
            try
            {
                var selectedItem = listView1.SelectedItems[0];
                if (selectedItem.Tag is FileInfo file)
                {
                    Process.Start(file.FullName);
                }
                else if (selectedItem.Tag is DirectoryInfo dir)
                {
                    Process.Start("explorer.exe", dir.FullName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening item: {ex.Message}");
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update status based on selection
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                if (selectedItem.Tag is FileInfo file)
                {
                    statusLabel.Text = $"Selected: {file.FullName}";
                }
                else if (selectedItem.Tag is DirectoryInfo dir)
                {
                    statusLabel.Text = $"Selected: {dir.FullName}";
                }
            }
        }

        // Menu and Context Menu Event Handlers
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutSelectedItems();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedItems();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Paste functionality would be implemented here
            MessageBox.Show("Paste functionality not implemented in this demo.");
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Selected = true;
            }
        }

        private void CutSelectedItems()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var paths = new List<string>();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    if (item.Tag is FileInfo file)
                        paths.Add(file.FullName);
                    else if (item.Tag is DirectoryInfo dir)
                        paths.Add(dir.FullName);
                }

                if (paths.Count > 0)
                {
                    Clipboard.SetText(string.Join("\n", paths));
                    statusLabel.Text = $"Cut {paths.Count} items to clipboard";
                }
            }
        }

        private void CopySelectedItems()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var paths = new List<string>();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    if (item.Tag is FileInfo file)
                        paths.Add(file.FullName);
                    else if (item.Tag is DirectoryInfo dir)
                        paths.Add(dir.FullName);
                }

                if (paths.Count > 0)
                {
                    Clipboard.SetText(string.Join("\n", paths));
                    statusLabel.Text = $"Copied {paths.Count} items to clipboard";
                }
            }
        }

        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.LargeIcon;
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Details;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.List;
        }

        // Context Menu Event Handlers
        private void openContextMenuItem_Click(object sender, EventArgs e)
        {
            OpenSelectedItem();
        }

        private void openWithContextMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                if (selectedItem.Tag is FileInfo file)
                {
                    try
                    {
                        Process.Start("rundll32.exe", $"shell32.dll,OpenAs_RunDLL {file.FullName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening with dialog: {ex.Message}");
                    }
                }
            }
        }

        private void cutContextMenuItem_Click(object sender, EventArgs e)
        {
            CutSelectedItems();
        }

        private void copyContextMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedItems();
        }

        private void deleteContextMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {listView1.SelectedItems.Count} item(s)?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        try
                        {
                            if (item.Tag is FileInfo file)
                            {
                                file.Delete();
                            }
                            else if (item.Tag is DirectoryInfo dir)
                            {
                                dir.Delete(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting item: {ex.Message}");
                        }
                    }

                    // Refresh the search
                    PerformSearch();
                }
            }
        }

        private void renameContextMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                var selectedItem = listView1.SelectedItems[0];
                string currentName = selectedItem.Text;

                string newName = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter new name:", "Rename", currentName);

                if (!string.IsNullOrEmpty(newName) && newName != currentName)
                {
                    try
                    {
                        if (selectedItem.Tag is FileInfo file)
                        {
                            string newPath = Path.Combine(file.DirectoryName, newName);
                            file.MoveTo(newPath);
                        }
                        else if (selectedItem.Tag is DirectoryInfo dir)
                        {
                            string newPath = Path.Combine(dir.Parent.FullName, newName);
                            dir.MoveTo(newPath);
                        }

                        // Refresh the search
                        PerformSearch();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error renaming item: {ex.Message}");
                    }
                }
            }
        }

        private void propertiesContextMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                var selectedItem = listView1.SelectedItems[0];
                string info = "";

                if (selectedItem.Tag is FileInfo file)
                {
                    info = $"File: {file.Name}\n" +
                           $"Path: {file.FullName}\n" +
                           $"Size: {FormatFileSize(file.Length)}\n" +
                           $"Created: {file.CreationTime}\n" +
                           $"Modified: {file.LastWriteTime}\n" +
                           $"Attributes: {file.Attributes}";
                }
                else if (selectedItem.Tag is DirectoryInfo dir)
                {
                    info = $"Folder: {dir.Name}\n" +
                           $"Path: {dir.FullName}\n" +
                           $"Created: {dir.CreationTime}\n" +
                           $"Modified: {dir.LastWriteTime}\n" +
                           $"Attributes: {dir.Attributes}";
                }

                MessageBox.Show(info, "Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void everythingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilter("Everything");
        }

        private void audioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilter("Audio");
        }

        private void compressedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilter("Compressed");
        }

        private void documentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilter("Document");
        }

        private void executableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilter("Executable");
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilter("Folder");
        }

        private void pictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilter("Picture");
        }

        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilter("Video");
        }

        private void SetFilter(string filter)
        {
            currentFilter = filter;

            // Update menu check states
            everythingToolStripMenuItem.Checked = (filter == "Everything");
            audioToolStripMenuItem.Checked = (filter == "Audio");
            compressedToolStripMenuItem.Checked = (filter == "Compressed");
            documentToolStripMenuItem.Checked = (filter == "Document");
            executableToolStripMenuItem.Checked = (filter == "Executable");
            folderToolStripMenuItem.Checked = (filter == "Folder");
            pictureToolStripMenuItem.Checked = (filter == "Picture");
            videoToolStripMenuItem.Checked = (filter == "Video");

            if (!string.IsNullOrEmpty(currentSearchTerm))
                PerformSearch();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Options dialog not implemented in this demo.", "Options", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fast File Search v1.0\nA file search application demo.", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (searchWorker != null && searchWorker.IsBusy)
            {
                searchWorker.CancelAsync();
            }
            base.OnFormClosing(e);
        }
    }
}