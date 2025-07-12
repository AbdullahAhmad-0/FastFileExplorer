namespace FastFileSearch
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolStripMenuItem matchCaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem matchWholeWordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem matchPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem matchDiacriticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableRegexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem everythingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem audioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem executableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem folderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem videoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showInExplorerContextMenuItem;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            cutToolStripMenuItem = new ToolStripMenuItem();
            copyToolStripMenuItem = new ToolStripMenuItem();
            pasteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            selectAllToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            matchCaseToolStripMenuItem = new ToolStripMenuItem();
            matchWholeWordToolStripMenuItem = new ToolStripMenuItem();
            matchPathToolStripMenuItem = new ToolStripMenuItem();
            matchDiacriticsToolStripMenuItem = new ToolStripMenuItem();
            enableRegexToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            filterToolStripMenuItem = new ToolStripMenuItem();
            everythingToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            audioToolStripMenuItem = new ToolStripMenuItem();
            compressedToolStripMenuItem = new ToolStripMenuItem();
            documentToolStripMenuItem = new ToolStripMenuItem();
            executableToolStripMenuItem = new ToolStripMenuItem();
            folderToolStripMenuItem = new ToolStripMenuItem();
            pictureToolStripMenuItem = new ToolStripMenuItem();
            videoToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            largeIconsToolStripMenuItem = new ToolStripMenuItem();
            detailsToolStripMenuItem = new ToolStripMenuItem();
            listToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            searchPanel = new Panel();
            searchBox = new TextBox();
            searchLabel = new Label();
            listView1 = new ListView();
            columnName = new ColumnHeader();
            columnPath = new ColumnHeader();
            columnDateModified = new ColumnHeader();
            columnType = new ColumnHeader();
            columnSize = new ColumnHeader();
            contextMenuStrip1 = new ContextMenuStrip(components);
            openContextMenuItem = new ToolStripMenuItem();
            openWithContextMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            cutContextMenuItem = new ToolStripMenuItem();
            copyContextMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            deleteContextMenuItem = new ToolStripMenuItem();
            renameContextMenuItem = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            showInExplorerContextMenuItem = new ToolStripMenuItem();
            propertiesContextMenuItem = new ToolStripMenuItem();
            imageList1 = new ImageList(components);
            statusStrip1 = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            itemCountLabel = new ToolStripStatusLabel();
            menuStrip1.SuspendLayout();
            searchPanel.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, searchToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1400, 24);
            menuStrip1.TabIndex = 3;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(93, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator2, selectAllToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "&Edit";
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.Size = new Size(122, 22);
            cutToolStripMenuItem.Text = "Cu&t";
            cutToolStripMenuItem.Click += cutToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new Size(122, 22);
            copyToolStripMenuItem.Text = "&Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.Size = new Size(122, 22);
            pasteToolStripMenuItem.Text = "&Paste";
            pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(119, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.Size = new Size(122, 22);
            selectAllToolStripMenuItem.Text = "Select &All";
            selectAllToolStripMenuItem.Click += selectAllToolStripMenuItem_Click;
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { matchCaseToolStripMenuItem, matchWholeWordToolStripMenuItem, matchPathToolStripMenuItem, matchDiacriticsToolStripMenuItem, enableRegexToolStripMenuItem, toolStripSeparator1, filterToolStripMenuItem });
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(54, 20);
            searchToolStripMenuItem.Text = "&Search";
            // 
            // matchCaseToolStripMenuItem
            // 
            matchCaseToolStripMenuItem.CheckOnClick = true;
            matchCaseToolStripMenuItem.Name = "matchCaseToolStripMenuItem";
            matchCaseToolStripMenuItem.Size = new Size(177, 22);
            matchCaseToolStripMenuItem.Text = "Match &Case";
            matchCaseToolStripMenuItem.Click += matchCaseToolStripMenuItem_Click;
            // 
            // matchWholeWordToolStripMenuItem
            // 
            matchWholeWordToolStripMenuItem.CheckOnClick = true;
            matchWholeWordToolStripMenuItem.Name = "matchWholeWordToolStripMenuItem";
            matchWholeWordToolStripMenuItem.Size = new Size(177, 22);
            matchWholeWordToolStripMenuItem.Text = "Match &Whole Word";
            matchWholeWordToolStripMenuItem.Click += matchWholeWordToolStripMenuItem_Click;
            // 
            // matchPathToolStripMenuItem
            // 
            matchPathToolStripMenuItem.CheckOnClick = true;
            matchPathToolStripMenuItem.Name = "matchPathToolStripMenuItem";
            matchPathToolStripMenuItem.Size = new Size(177, 22);
            matchPathToolStripMenuItem.Text = "Match &Path";
            matchPathToolStripMenuItem.Click += matchPathToolStripMenuItem_Click;
            // 
            // matchDiacriticsToolStripMenuItem
            // 
            matchDiacriticsToolStripMenuItem.CheckOnClick = true;
            matchDiacriticsToolStripMenuItem.Name = "matchDiacriticsToolStripMenuItem";
            matchDiacriticsToolStripMenuItem.Size = new Size(177, 22);
            matchDiacriticsToolStripMenuItem.Text = "Match &Diacritics";
            matchDiacriticsToolStripMenuItem.Click += matchDiacriticsToolStripMenuItem_Click;
            // 
            // enableRegexToolStripMenuItem
            // 
            enableRegexToolStripMenuItem.CheckOnClick = true;
            enableRegexToolStripMenuItem.Name = "enableRegexToolStripMenuItem";
            enableRegexToolStripMenuItem.Size = new Size(177, 22);
            enableRegexToolStripMenuItem.Text = "Enable &Regex";
            enableRegexToolStripMenuItem.Click += enableRegexToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(174, 6);
            // 
            // filterToolStripMenuItem
            // 
            filterToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { everythingToolStripMenuItem, toolStripSeparator3, audioToolStripMenuItem, compressedToolStripMenuItem, documentToolStripMenuItem, executableToolStripMenuItem, folderToolStripMenuItem, pictureToolStripMenuItem, videoToolStripMenuItem });
            filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            filterToolStripMenuItem.Size = new Size(177, 22);
            filterToolStripMenuItem.Text = "&Filter";
            // 
            // everythingToolStripMenuItem
            // 
            everythingToolStripMenuItem.Checked = true;
            everythingToolStripMenuItem.CheckState = CheckState.Checked;
            everythingToolStripMenuItem.Name = "everythingToolStripMenuItem";
            everythingToolStripMenuItem.Size = new Size(140, 22);
            everythingToolStripMenuItem.Text = "&Everything";
            everythingToolStripMenuItem.Click += everythingToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(137, 6);
            // 
            // audioToolStripMenuItem
            // 
            audioToolStripMenuItem.Name = "audioToolStripMenuItem";
            audioToolStripMenuItem.Size = new Size(140, 22);
            audioToolStripMenuItem.Text = "&Audio";
            audioToolStripMenuItem.Click += audioToolStripMenuItem_Click;
            // 
            // compressedToolStripMenuItem
            // 
            compressedToolStripMenuItem.Name = "compressedToolStripMenuItem";
            compressedToolStripMenuItem.Size = new Size(140, 22);
            compressedToolStripMenuItem.Text = "&Compressed";
            compressedToolStripMenuItem.Click += compressedToolStripMenuItem_Click;
            // 
            // documentToolStripMenuItem
            // 
            documentToolStripMenuItem.Name = "documentToolStripMenuItem";
            documentToolStripMenuItem.Size = new Size(140, 22);
            documentToolStripMenuItem.Text = "&Document";
            documentToolStripMenuItem.Click += documentToolStripMenuItem_Click;
            // 
            // executableToolStripMenuItem
            // 
            executableToolStripMenuItem.Name = "executableToolStripMenuItem";
            executableToolStripMenuItem.Size = new Size(140, 22);
            executableToolStripMenuItem.Text = "&Executable";
            executableToolStripMenuItem.Click += executableToolStripMenuItem_Click;
            // 
            // folderToolStripMenuItem
            // 
            folderToolStripMenuItem.Name = "folderToolStripMenuItem";
            folderToolStripMenuItem.Size = new Size(140, 22);
            folderToolStripMenuItem.Text = "&Folder";
            folderToolStripMenuItem.Click += folderToolStripMenuItem_Click;
            // 
            // pictureToolStripMenuItem
            // 
            pictureToolStripMenuItem.Name = "pictureToolStripMenuItem";
            pictureToolStripMenuItem.Size = new Size(140, 22);
            pictureToolStripMenuItem.Text = "&Picture";
            pictureToolStripMenuItem.Click += pictureToolStripMenuItem_Click;
            // 
            // videoToolStripMenuItem
            // 
            videoToolStripMenuItem.Name = "videoToolStripMenuItem";
            videoToolStripMenuItem.Size = new Size(140, 22);
            videoToolStripMenuItem.Text = "&Video";
            videoToolStripMenuItem.Click += videoToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { largeIconsToolStripMenuItem, detailsToolStripMenuItem, listToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "&View";
            // 
            // largeIconsToolStripMenuItem
            // 
            largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            largeIconsToolStripMenuItem.Size = new Size(134, 22);
            largeIconsToolStripMenuItem.Text = "&Large Icons";
            largeIconsToolStripMenuItem.Click += largeIconsToolStripMenuItem_Click;
            // 
            // detailsToolStripMenuItem
            // 
            detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            detailsToolStripMenuItem.Size = new Size(134, 22);
            detailsToolStripMenuItem.Text = "&Details";
            detailsToolStripMenuItem.Click += detailsToolStripMenuItem_Click;
            // 
            // listToolStripMenuItem
            // 
            listToolStripMenuItem.Name = "listToolStripMenuItem";
            listToolStripMenuItem.Size = new Size(134, 22);
            listToolStripMenuItem.Text = "&List";
            listToolStripMenuItem.Click += listToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { optionsToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(116, 22);
            optionsToolStripMenuItem.Text = "&Options";
            optionsToolStripMenuItem.Click += optionsToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(107, 22);
            aboutToolStripMenuItem.Text = "&About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // searchPanel
            // 
            searchPanel.Controls.Add(searchBox);
            searchPanel.Controls.Add(searchLabel);
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Location = new Point(0, 24);
            searchPanel.Margin = new Padding(4, 3, 4, 3);
            searchPanel.Name = "searchPanel";
            searchPanel.Padding = new Padding(6);
            searchPanel.Size = new Size(1400, 40);
            searchPanel.TabIndex = 1;
            // 
            // searchBox
            // 
            searchBox.Dock = DockStyle.Fill;
            searchBox.Location = new Point(57, 6);
            searchBox.Margin = new Padding(4, 3, 4, 3);
            searchBox.Name = "searchBox";
            searchBox.Size = new Size(1337, 23);
            searchBox.TabIndex = 1;
            searchBox.TextChanged += searchBox_TextChanged;
            searchBox.KeyDown += searchBox_KeyDown;
            // 
            // searchLabel
            // 
            searchLabel.AutoSize = true;
            searchLabel.Dock = DockStyle.Left;
            searchLabel.Location = new Point(6, 6);
            searchLabel.Margin = new Padding(4, 0, 4, 0);
            searchLabel.Name = "searchLabel";
            searchLabel.Padding = new Padding(0, 6, 6, 0);
            searchLabel.Size = new Size(51, 21);
            searchLabel.TabIndex = 0;
            searchLabel.Text = "Search:";
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnName, columnPath, columnDateModified, columnType, columnSize });
            listView1.ContextMenuStrip = contextMenuStrip1;
            listView1.Dock = DockStyle.Fill;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.LargeImageList = imageList1;
            listView1.Location = new Point(0, 64);
            listView1.Margin = new Padding(4, 3, 4, 3);
            listView1.Name = "listView1";
            listView1.Size = new Size(1400, 606);
            listView1.SmallImageList = imageList1;
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            listView1.DoubleClick += listView1_DoubleClick;
            // 
            // columnName
            // 
            columnName.Text = "Name";
            columnName.Width = 250;
            // 
            // columnPath
            // 
            columnPath.Text = "Path";
            columnPath.Width = 400;
            // 
            // columnDateModified
            // 
            columnDateModified.Text = "Date Modified";
            columnDateModified.Width = 150;
            // 
            // columnType
            // 
            columnType.Text = "Type";
            columnType.Width = 150;
            // 
            // columnSize
            // 
            columnSize.Text = "Size";
            columnSize.Width = 100;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { openContextMenuItem, openWithContextMenuItem, toolStripSeparator4, cutContextMenuItem, copyContextMenuItem, toolStripSeparator5, deleteContextMenuItem, renameContextMenuItem, toolStripSeparator6, showInExplorerContextMenuItem, propertiesContextMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(163, 198);
            // 
            // openContextMenuItem
            // 
            openContextMenuItem.Name = "openContextMenuItem";
            openContextMenuItem.Size = new Size(162, 22);
            openContextMenuItem.Text = "&Open";
            openContextMenuItem.Click += openContextMenuItem_Click;
            // 
            // openWithContextMenuItem
            // 
            openWithContextMenuItem.Name = "openWithContextMenuItem";
            openWithContextMenuItem.Size = new Size(162, 22);
            openWithContextMenuItem.Text = "Open &With";
            openWithContextMenuItem.Click += openWithContextMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(159, 6);
            // 
            // cutContextMenuItem
            // 
            cutContextMenuItem.Name = "cutContextMenuItem";
            cutContextMenuItem.Size = new Size(162, 22);
            cutContextMenuItem.Text = "Cu&t";
            cutContextMenuItem.Click += cutContextMenuItem_Click;
            // 
            // copyContextMenuItem
            // 
            copyContextMenuItem.Name = "copyContextMenuItem";
            copyContextMenuItem.Size = new Size(162, 22);
            copyContextMenuItem.Text = "&Copy";
            copyContextMenuItem.Click += copyContextMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(159, 6);
            // 
            // deleteContextMenuItem
            // 
            deleteContextMenuItem.Name = "deleteContextMenuItem";
            deleteContextMenuItem.Size = new Size(162, 22);
            deleteContextMenuItem.Text = "&Delete";
            deleteContextMenuItem.Click += deleteContextMenuItem_Click;
            // 
            // renameContextMenuItem
            // 
            renameContextMenuItem.Name = "renameContextMenuItem";
            renameContextMenuItem.Size = new Size(162, 22);
            renameContextMenuItem.Text = "&Rename";
            renameContextMenuItem.Click += renameContextMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(159, 6);
            // 
            // showInExplorerContextMenuItem
            // 
            showInExplorerContextMenuItem.Name = "showInExplorerContextMenuItem";
            showInExplorerContextMenuItem.Size = new Size(162, 22);
            showInExplorerContextMenuItem.Text = "Show in Explorer";
            showInExplorerContextMenuItem.Click += showInExplorerContextMenuItem_Click;
            // 
            // propertiesContextMenuItem
            // 
            propertiesContextMenuItem.Name = "propertiesContextMenuItem";
            propertiesContextMenuItem.Size = new Size(162, 22);
            propertiesContextMenuItem.Text = "&Properties";
            propertiesContextMenuItem.Click += propertiesContextMenuItem_Click;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { statusLabel, itemCountLabel });
            statusStrip1.Location = new Point(0, 670);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1400, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(39, 17);
            statusLabel.Text = "Ready";
            // 
            // itemCountLabel
            // 
            itemCountLabel.Name = "itemCountLabel";
            itemCountLabel.Size = new Size(45, 17);
            itemCountLabel.Text = "0 items";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1400, 692);
            Controls.Add(listView1);
            Controls.Add(searchPanel);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "Fast File Search";
            Load += MainForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            searchPanel.ResumeLayout(false);
            searchPanel.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnPath;
        private System.Windows.Forms.ColumnHeader columnDateModified;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnSize;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWithContextMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem cutContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyContextMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem deleteContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameContextMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem propertiesContextMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel itemCountLabel;
    }
}