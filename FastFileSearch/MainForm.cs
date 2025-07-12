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
using System.Runtime.InteropServices;
// using Shell32;

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

        private Dictionary<string, int> iconCache = new Dictionary<string, int>();
        private const int THUMBNAIL_SIZE = 32;

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

            // Create separate image lists for different sizes
            var smallImageList = new ImageList();
            smallImageList.ImageSize = new Size(16, 16);
            smallImageList.ColorDepth = ColorDepth.Depth32Bit;

            var largeImageList = new ImageList();
            largeImageList.ImageSize = new Size(32, 32);
            largeImageList.ColorDepth = ColorDepth.Depth32Bit;

            listView1.SmallImageList = smallImageList;
            listView1.LargeImageList = largeImageList;

            // Use the larger one as default
            imageList1 = largeImageList;

            // Add columns
            listView1.Columns.Add("Name", 200);
            listView1.Columns.Add("Path", 300);
            listView1.Columns.Add("Date Modified", 120);
            listView1.Columns.Add("Type", 100);
            listView1.Columns.Add("Size", 80);

            // Load system icons
            LoadSystemIcons();
        }

        private async Task LoadThumbnailAsync(ListViewItem item, string filePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    Image thumbnail = CreateImageThumbnail(filePath);
                    if (thumbnail != null)
                    {
                        this.Invoke((Action)(() =>
                        {
                            if (item.ListView != null) // Check if item is still in ListView
                            {
                                string thumbKey = $"thumb_{Path.GetFileName(filePath)}_{DateTime.Now.Ticks}";
                                imageList1.Images.Add(thumbKey, thumbnail);
                                item.ImageKey = thumbKey;
                            }
                        }));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading thumbnail async: {ex.Message}");
                }
            });
        }

        private Icon GetFolderIcon()
        {
            try
            {
                return GetSystemIcon("folder", true);
            }
            catch
            {
                return null;
            }
        }

        private Icon GetSystemIcon(string path, bool isFolder)
        {
            try
            {
                SHFILEINFO shfi = new SHFILEINFO();
                uint flags = SHGFI_ICON | SHGFI_SMALLICON;

                if (isFolder)
                    flags |= SHGFI_USEFILEATTRIBUTES;

                IntPtr hImg = SHGetFileInfo(path,
                    isFolder ? FILE_ATTRIBUTE_DIRECTORY : 0,
                    ref shfi,
                    (uint)Marshal.SizeOf(shfi),
                    flags);

                if (hImg != IntPtr.Zero && shfi.hIcon != IntPtr.Zero)
                {
                    Icon icon = Icon.FromHandle(shfi.hIcon);
                    Icon clonedIcon = (Icon)icon.Clone();
                    DestroyIcon(shfi.hIcon);
                    return clonedIcon;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting icon for {path}: {ex.Message}");
            }
            return null;
        }

        private Image GetThumbnail(string filePath)
        {
            try
            {
                string ext = Path.GetExtension(filePath).ToLower();

                // For images, create thumbnail
                if (IsPictureFile(ext))
                {
                    return CreateImageThumbnail(filePath);
                }

                // For videos, try to get thumbnail (basic implementation)
                if (IsVideoFile(ext))
                {
                    return CreateVideoThumbnail(filePath);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating thumbnail for {filePath}: {ex.Message}");
                return null;
            }
        }

        private Image CreateImageThumbnail(string imagePath)
        {
            try
            {
                using (var image = Image.FromFile(imagePath))
                {
                    return image.GetThumbnailImage(THUMBNAIL_SIZE, THUMBNAIL_SIZE, null, IntPtr.Zero);
                }
            }
            catch
            {
                return null;
            }
        }

        private Image CreateVideoThumbnail(string videoPath)
        {
            // Basic video thumbnail - you might want to use a library like FFMpegCore for better results
            try
            {
                var videoIcon = GetSystemIcon(videoPath, false);
                if (videoIcon != null)
                {
                    return videoIcon.ToBitmap();
                }
            }
            catch
            {
                // Fall back to default icon
            }
            return null;
        }

        private void LoadSystemIcons()
        {
            try
            {
                imageList1.ImageSize = new Size(THUMBNAIL_SIZE, THUMBNAIL_SIZE);
                imageList1.ColorDepth = ColorDepth.Depth32Bit;

                // Add default folder icon
                var folderIcon = GetFolderIcon();
                if (folderIcon != null)
                {
                    imageList1.Images.Add("folder", folderIcon);
                }
                else
                {
                    imageList1.Images.Add("folder", SystemIcons.Question.ToBitmap());
                }

                // Add default file icon
                imageList1.Images.Add("file", SystemIcons.Application.ToBitmap());
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

            // Add debug info
            Text = $"Fast File Search - Running as: {Environment.UserName}";

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

        private string GetOrCreateIconKey(string filePath, string extension)
        {
            string ext = extension.ToLower();

            // Check if we already have this icon cached
            if (iconCache.ContainsKey(ext))
            {
                return iconCache[ext].ToString();
            }

            // Try to get thumbnail first for images/videos
            Image thumbnail = GetThumbnail(filePath);
            if (thumbnail != null)
            {
                string thumbKey = $"thumb_{iconCache.Count}";
                imageList1.Images.Add(thumbKey, thumbnail);
                iconCache[ext] = iconCache.Count;
                return thumbKey;
            }

            // Get system icon for this file type
            Icon icon = GetSystemIcon(filePath, false);
            if (icon != null)
            {
                string iconKey = $"icon_{iconCache.Count}";
                imageList1.Images.Add(iconKey, icon.ToBitmap());
                iconCache[ext] = iconCache.Count;
                icon.Dispose();
                return iconKey;
            }

            // Fall back to default file icon
            return "file";
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr handle);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_SMALLICON = 0x1;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

        private ListViewItem CreateListViewItem(FileInfo file)
        {
            var item = new ListViewItem(file.Name);
            item.SubItems.Add(file.DirectoryName);
            item.SubItems.Add(file.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
            item.SubItems.Add(file.Extension.ToUpper() + " File");
            item.SubItems.Add(FormatFileSize(file.Length));
            item.Tag = file;

            // Set icon/thumbnail
            string iconKey = GetOrCreateIconKey(file.FullName, file.Extension);
            item.ImageKey = iconKey;

            // Load thumbnail asynchronously for image files
            if (IsPictureFile(file.Extension))
            {
                Task.Run(() => LoadThumbnailAsync(item, file.FullName));
            }

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
                // Report progress to UI
                this.Invoke((Action)(() =>
                {
                    statusLabel.Text = "Starting file indexing...";
                }));

                // Get all drives first
                var drives = DriveInfo.GetDrives().Where(d => d.IsReady).ToList();

                foreach (var drive in drives)
                {
                    try
                    {
                        this.Invoke((Action)(() =>
                        {
                            statusLabel.Text = $"Indexing drive {drive.Name}...";
                        }));

                        IndexDirectory(drive.RootDirectory);

                        this.Invoke((Action)(() =>
                        {
                            statusLabel.Text = $"Indexing {drive.Name}... Found {allFiles.Count} files, {allDirectories.Count} directories";
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((Action)(() =>
                        {
                            statusLabel.Text = $"Warning: Could not index drive {drive.Name} - {ex.Message}";
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((Action)(() =>
                {
                    statusLabel.Text = $"Indexing error: {ex.Message}";
                }));
            }
        }


        private void IndexDirectory(DirectoryInfo dir)
        {
            try
            {
                if (searchWorker.CancellationPending) return;

                // Skip only critical system directories
                var skipDirs = new[] {
            "System Volume Information", "$Recycle.Bin", "Recovery",
            "Config.Msi", "Windows\\CSC", "Windows\\Installer"
        };

                if (skipDirs.Any(skip => dir.FullName.EndsWith(skip, StringComparison.OrdinalIgnoreCase)))
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
                catch (UnauthorizedAccessException)
                {
                    // Skip directories we can't access
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error accessing files in {dir.FullName}: {ex.Message}");
                }

                // Recursively index subdirectories
                try
                {
                    var subDirs = dir.GetDirectories();
                    foreach (var subDir in subDirs)
                    {
                        IndexDirectory(subDir);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip directories we can't access
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error accessing subdirectories in {dir.FullName}: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error indexing directory {dir.FullName}: {ex.Message}");
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

        private void TestIndexing()
        {
            if (allFiles.Count == 0 && allDirectories.Count == 0)
            {
                statusLabel.Text = "No files indexed. Try running as administrator or check permissions.";

                // Provide a simple fallback - index current directory
                try
                {
                    var currentDir = new DirectoryInfo(Application.StartupPath);
                    IndexDirectory(currentDir);

                    if (allFiles.Count > 0 || allDirectories.Count > 0)
                    {
                        statusLabel.Text = $"Indexed current directory: {allFiles.Count} files, {allDirectories.Count} directories";
                    }
                }
                catch (Exception ex)
                {
                    statusLabel.Text = $"Error: {ex.Message}";
                }
            }
        }

        private void SearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isSearching = false;

            if (e.Error != null)
            {
                statusLabel.Text = $"Indexing failed: {e.Error.Message}";
                MessageBox.Show($"Indexing error: {e.Error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                statusLabel.Text = $"Ready - Indexed {allFiles.Count} files and {allDirectories.Count} directories";

                // Test if indexing actually worked
                if (allFiles.Count == 0 && allDirectories.Count == 0)
                {
                    // TestIndexing();
                    IndexFiles();
                }
            }
        }

        private void RefreshIndex()
        {
            allFiles = new ConcurrentBag<FileInfo>();
            allDirectories = new ConcurrentBag<DirectoryInfo>();

            if (!searchWorker.IsBusy)
            {
                statusLabel.Text = "Re-indexing files...";
                searchWorker.RunWorkerAsync("index");
            }
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

        private void showInExplorerContextMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                try
                {
                    if (selectedItem.Tag is FileInfo file)
                    {
                        Process.Start("explorer.exe", $"/select,\"{file.FullName}\"");
                    }
                    else if (selectedItem.Tag is DirectoryInfo dir)
                    {
                        Process.Start("explorer.exe", $"\"{dir.FullName}\"");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening in Explorer: {ex.Message}");
                }
            }
        }

        private string GetCurrentDirectory()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                if (selectedItem.Tag is FileInfo file)
                    return file.DirectoryName;
                else if (selectedItem.Tag is DirectoryInfo dir)
                    return dir.FullName;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    var paths = Clipboard.GetText().Split('\n');
                    var targetDir = GetCurrentDirectory();

                    foreach (var path in paths)
                    {
                        if (File.Exists(path))
                        {
                            var fileName = Path.GetFileName(path);
                            var targetPath = Path.Combine(targetDir, fileName);
                            File.Copy(path, targetPath, true);
                        }
                    }

                    MessageBox.Show($"Pasted {paths.Length} items", "Paste Complete");
                    RefreshIndex();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error pasting: {ex.Message}");
            }
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

                if (selectedItem.Tag is FileInfo file)
                {
                    try
                    {
                        // Show Windows properties dialog
                        SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
                        info.cbSize = Marshal.SizeOf(info);
                        info.lpVerb = "properties";
                        info.lpFile = file.FullName;
                        info.nShow = 1;
                        info.fMask = 0x0000000C;
                        ShellExecuteEx(ref info);
                    }
                    catch
                    {
                        // Fallback to custom dialog
                        string infoText = $"File: {file.Name}\n" +
                                       $"Path: {file.FullName}\n" +
                                       $"Size: {FormatFileSize(file.Length)}\n" +
                                       $"Created: {file.CreationTime}\n" +
                                       $"Modified: {file.LastWriteTime}\n" +
                                       $"Attributes: {file.Attributes}";
                        MessageBox.Show(infoText, "Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
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