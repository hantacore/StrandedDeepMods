namespace StrandedDeepMapper
{
    partial class WorldRandomizerTool
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbWorkshopPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.bRefreshSlots = new System.Windows.Forms.Button();
            this.rbSlot3 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.rbSlot2 = new System.Windows.Forms.RadioButton();
            this.rbSlot1 = new System.Windows.Forms.RadioButton();
            this.rbSlot0 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDataDirectory = new System.Windows.Forms.TextBox();
            this.bSearchDataDirectory = new System.Windows.Forms.Button();
            this.tbMapsDirectory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbSDDefaultDirectory = new System.Windows.Forms.TextBox();
            this.tbMissionsPath = new System.Windows.Forms.TextBox();
            this.bSteam = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.bHowto = new System.Windows.Forms.Button();
            this.tsiOpenMapDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuOpenDirectory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.gbWorldOptions = new System.Windows.Forms.GroupBox();
            this.cbUseMissions = new System.Windows.Forms.CheckBox();
            this.cbMixMaps = new System.Windows.Forms.CheckBox();
            this.cbDeep = new System.Windows.Forms.CheckBox();
            this.bRestore = new System.Windows.Forms.Button();
            this.bUnselectAll = new System.Windows.Forms.Button();
            this.bSelectAll = new System.Windows.Forms.Button();
            this.bDeleteWorld = new System.Windows.Forms.Button();
            this.bBackupWorld = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.bGenerate = new System.Windows.Forms.Button();
            this.rbScarse = new System.Windows.Forms.RadioButton();
            this.rbSea = new System.Windows.Forms.RadioButton();
            this.rbDense = new System.Windows.Forms.RadioButton();
            this.gbWorldType = new System.Windows.Forms.GroupBox();
            this.clbMaps = new System.Windows.Forms.CheckedListBox();
            this.tbBigIslandsProportion = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.contextMenuOpenDirectory.SuspendLayout();
            this.gbWorldOptions.SuspendLayout();
            this.gbWorldType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbBigIslandsProportion)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tbWorkshopPath);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.bRefreshSlots);
            this.groupBox1.Controls.Add(this.rbSlot3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.rbSlot2);
            this.groupBox1.Controls.Add(this.rbSlot1);
            this.groupBox1.Controls.Add(this.rbSlot0);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbDataDirectory);
            this.groupBox1.Controls.Add(this.bSearchDataDirectory);
            this.groupBox1.Controls.Add(this.tbMapsDirectory);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbSDDefaultDirectory);
            this.groupBox1.Controls.Add(this.tbMissionsPath);
            this.groupBox1.Controls.Add(this.bSteam);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1133, 228);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setup";
            // 
            // tbWorkshopPath
            // 
            this.tbWorkshopPath.Location = new System.Drawing.Point(155, 181);
            this.tbWorkshopPath.Name = "tbWorkshopPath";
            this.tbWorkshopPath.ReadOnly = true;
            this.tbWorkshopPath.Size = new System.Drawing.Size(975, 20);
            this.tbWorkshopPath.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 184);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(131, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Workshop maps found in :";
            // 
            // bRefreshSlots
            // 
            this.bRefreshSlots.Location = new System.Drawing.Point(247, 58);
            this.bRefreshSlots.Name = "bRefreshSlots";
            this.bRefreshSlots.Size = new System.Drawing.Size(75, 23);
            this.bRefreshSlots.TabIndex = 25;
            this.bRefreshSlots.Text = "Refresh";
            this.bRefreshSlots.UseVisualStyleBackColor = true;
            this.bRefreshSlots.Click += new System.EventHandler(this.bRefreshSlots_Click);
            // 
            // rbSlot3
            // 
            this.rbSlot3.AutoSize = true;
            this.rbSlot3.Location = new System.Drawing.Point(189, 61);
            this.rbSlot3.Name = "rbSlot3";
            this.rbSlot3.Size = new System.Drawing.Size(52, 17);
            this.rbSlot3.TabIndex = 24;
            this.rbSlot3.TabStop = true;
            this.rbSlot3.Text = "Slot 4";
            this.rbSlot3.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(825, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Locate your Stranded Deep installation folder (usually in : drive / Program Files" +
    " (x86) / Steam / steamapps / common / Stranded Deep, add Experimental at the end" +
    " if needed)";
            // 
            // rbSlot2
            // 
            this.rbSlot2.AutoSize = true;
            this.rbSlot2.Location = new System.Drawing.Point(131, 61);
            this.rbSlot2.Name = "rbSlot2";
            this.rbSlot2.Size = new System.Drawing.Size(52, 17);
            this.rbSlot2.TabIndex = 22;
            this.rbSlot2.TabStop = true;
            this.rbSlot2.Text = "Slot 3";
            this.rbSlot2.UseVisualStyleBackColor = true;
            // 
            // rbSlot1
            // 
            this.rbSlot1.AutoSize = true;
            this.rbSlot1.Location = new System.Drawing.Point(73, 61);
            this.rbSlot1.Name = "rbSlot1";
            this.rbSlot1.Size = new System.Drawing.Size(52, 17);
            this.rbSlot1.TabIndex = 21;
            this.rbSlot1.TabStop = true;
            this.rbSlot1.Text = "Slot 2";
            this.rbSlot1.UseVisualStyleBackColor = true;
            // 
            // rbSlot0
            // 
            this.rbSlot0.AutoSize = true;
            this.rbSlot0.Location = new System.Drawing.Point(15, 61);
            this.rbSlot0.Name = "rbSlot0";
            this.rbSlot0.Size = new System.Drawing.Size(52, 17);
            this.rbSlot0.TabIndex = 20;
            this.rbSlot0.TabStop = true;
            this.rbSlot0.Text = "Slot 1";
            this.rbSlot0.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(824, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Locate your Stranded Deep Data folder (usually in : drive / user / AppData / Loca" +
    "lLow / Beam Team Games / Stranded Deep / Data, add Experimental at the end if ne" +
    "eded)";
            // 
            // tbDataDirectory
            // 
            this.tbDataDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDataDirectory.Location = new System.Drawing.Point(15, 35);
            this.tbDataDirectory.Name = "tbDataDirectory";
            this.tbDataDirectory.ReadOnly = true;
            this.tbDataDirectory.Size = new System.Drawing.Size(960, 20);
            this.tbDataDirectory.TabIndex = 0;
            // 
            // bSearchDataDirectory
            // 
            this.bSearchDataDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSearchDataDirectory.Location = new System.Drawing.Point(988, 32);
            this.bSearchDataDirectory.Name = "bSearchDataDirectory";
            this.bSearchDataDirectory.Size = new System.Drawing.Size(139, 23);
            this.bSearchDataDirectory.TabIndex = 1;
            this.bSearchDataDirectory.Text = "Find Data directory...";
            this.bSearchDataDirectory.UseVisualStyleBackColor = true;
            this.bSearchDataDirectory.Click += new System.EventHandler(this.bSearchDataDirectory_Click);
            // 
            // tbMapsDirectory
            // 
            this.tbMapsDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMapsDirectory.Location = new System.Drawing.Point(108, 84);
            this.tbMapsDirectory.Name = "tbMapsDirectory";
            this.tbMapsDirectory.ReadOnly = true;
            this.tbMapsDirectory.Size = new System.Drawing.Size(1025, 20);
            this.tbMapsDirectory.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Missions found in :";
            // 
            // tbSDDefaultDirectory
            // 
            this.tbSDDefaultDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSDDefaultDirectory.Location = new System.Drawing.Point(15, 130);
            this.tbSDDefaultDirectory.Name = "tbSDDefaultDirectory";
            this.tbSDDefaultDirectory.ReadOnly = true;
            this.tbSDDefaultDirectory.Size = new System.Drawing.Size(960, 20);
            this.tbSDDefaultDirectory.TabIndex = 11;
            // 
            // tbMissionsPath
            // 
            this.tbMissionsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMissionsPath.Location = new System.Drawing.Point(115, 157);
            this.tbMissionsPath.Name = "tbMissionsPath";
            this.tbMissionsPath.ReadOnly = true;
            this.tbMissionsPath.Size = new System.Drawing.Size(1015, 20);
            this.tbMissionsPath.TabIndex = 18;
            // 
            // bSteam
            // 
            this.bSteam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSteam.Location = new System.Drawing.Point(986, 124);
            this.bSteam.Name = "bSteam";
            this.bSteam.Size = new System.Drawing.Size(141, 27);
            this.bSteam.TabIndex = 12;
            this.bSteam.Text = "Find SD directory...";
            this.bSteam.UseVisualStyleBackColor = true;
            this.bSteam.Click += new System.EventHandler(this.bSearchInstallationDir_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Maps found in :";
            // 
            // bHowto
            // 
            this.bHowto.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bHowto.ForeColor = System.Drawing.Color.Green;
            this.bHowto.Location = new System.Drawing.Point(7, 237);
            this.bHowto.Name = "bHowto";
            this.bHowto.Size = new System.Drawing.Size(340, 41);
            this.bHowto.TabIndex = 32;
            this.bHowto.Text = "How to use";
            this.bHowto.UseVisualStyleBackColor = true;
            this.bHowto.Click += new System.EventHandler(this.bHowto_Click);
            // 
            // tsiOpenMapDirectory
            // 
            this.tsiOpenMapDirectory.Name = "tsiOpenMapDirectory";
            this.tsiOpenMapDirectory.Size = new System.Drawing.Size(177, 22);
            this.tsiOpenMapDirectory.Text = "Open this map directory";
            // 
            // contextMenuOpenDirectory
            // 
            this.contextMenuOpenDirectory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiOpenMapDirectory});
            this.contextMenuOpenDirectory.Name = "contextMenuOpenDirectory";
            this.contextMenuOpenDirectory.ShowImageMargin = false;
            this.contextMenuOpenDirectory.Size = new System.Drawing.Size(178, 26);
            // 
            // gbWorldOptions
            // 
            this.gbWorldOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWorldOptions.Controls.Add(this.cbUseMissions);
            this.gbWorldOptions.Controls.Add(this.cbMixMaps);
            this.gbWorldOptions.Controls.Add(this.cbDeep);
            this.gbWorldOptions.Location = new System.Drawing.Point(773, 238);
            this.gbWorldOptions.Name = "gbWorldOptions";
            this.gbWorldOptions.Size = new System.Drawing.Size(193, 102);
            this.gbWorldOptions.TabIndex = 33;
            this.gbWorldOptions.TabStop = false;
            this.gbWorldOptions.Text = "World options";
            // 
            // cbUseMissions
            // 
            this.cbUseMissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUseMissions.AutoSize = true;
            this.cbUseMissions.Enabled = false;
            this.cbUseMissions.Location = new System.Drawing.Point(9, 15);
            this.cbUseMissions.Name = "cbUseMissions";
            this.cbUseMissions.Size = new System.Drawing.Size(87, 17);
            this.cbUseMissions.TabIndex = 5;
            this.cbUseMissions.Text = "Use missions";
            this.cbUseMissions.UseVisualStyleBackColor = true;
            // 
            // cbMixMaps
            // 
            this.cbMixMaps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMixMaps.Checked = true;
            this.cbMixMaps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMixMaps.Location = new System.Drawing.Point(9, 38);
            this.cbMixMaps.Name = "cbMixMaps";
            this.cbMixMaps.Size = new System.Drawing.Size(111, 35);
            this.cbMixMaps.TabIndex = 9;
            this.cbMixMaps.Text = "Mix user and procedural maps";
            this.cbMixMaps.UseVisualStyleBackColor = true;
            // 
            // cbDeep
            // 
            this.cbDeep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDeep.AutoSize = true;
            this.cbDeep.Location = new System.Drawing.Point(9, 78);
            this.cbDeep.Name = "cbDeep";
            this.cbDeep.Size = new System.Drawing.Size(104, 17);
            this.cbDeep.TabIndex = 10;
            this.cbDeep.Text = "Start in the deep";
            this.cbDeep.UseVisualStyleBackColor = true;
            // 
            // bRestore
            // 
            this.bRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bRestore.Location = new System.Drawing.Point(300, 618);
            this.bRestore.Name = "bRestore";
            this.bRestore.Size = new System.Drawing.Size(97, 61);
            this.bRestore.TabIndex = 31;
            this.bRestore.Text = "Restore world from backup";
            this.bRestore.UseVisualStyleBackColor = true;
            this.bRestore.Click += new System.EventHandler(this.bRestore_Click);
            // 
            // bUnselectAll
            // 
            this.bUnselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bUnselectAll.Location = new System.Drawing.Point(623, 293);
            this.bUnselectAll.Name = "bUnselectAll";
            this.bUnselectAll.Size = new System.Drawing.Size(147, 23);
            this.bUnselectAll.TabIndex = 30;
            this.bUnselectAll.Text = "Unselect All";
            this.bUnselectAll.UseVisualStyleBackColor = true;
            this.bUnselectAll.Click += new System.EventHandler(this.bUnselectAll_Click);
            // 
            // bSelectAll
            // 
            this.bSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSelectAll.Location = new System.Drawing.Point(623, 264);
            this.bSelectAll.Name = "bSelectAll";
            this.bSelectAll.Size = new System.Drawing.Size(147, 23);
            this.bSelectAll.TabIndex = 29;
            this.bSelectAll.Text = "Select All";
            this.bSelectAll.UseVisualStyleBackColor = true;
            this.bSelectAll.Click += new System.EventHandler(this.bSelectAll_Click);
            // 
            // bDeleteWorld
            // 
            this.bDeleteWorld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bDeleteWorld.ForeColor = System.Drawing.Color.Red;
            this.bDeleteWorld.Location = new System.Drawing.Point(118, 618);
            this.bDeleteWorld.Name = "bDeleteWorld";
            this.bDeleteWorld.Size = new System.Drawing.Size(176, 62);
            this.bDeleteWorld.TabIndex = 28;
            this.bDeleteWorld.Text = "Delete current world (Stranded Deep is stuck due to incompatible island or direct" +
    "ory corruption)";
            this.bDeleteWorld.UseVisualStyleBackColor = true;
            this.bDeleteWorld.Click += new System.EventHandler(this.bDeleteWorld_Click);
            // 
            // bBackupWorld
            // 
            this.bBackupWorld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bBackupWorld.Location = new System.Drawing.Point(7, 619);
            this.bBackupWorld.Name = "bBackupWorld";
            this.bBackupWorld.Size = new System.Drawing.Size(105, 61);
            this.bBackupWorld.TabIndex = 27;
            this.bBackupWorld.Text = "Backup current world";
            this.bBackupWorld.UseVisualStyleBackColor = true;
            this.bBackupWorld.Click += new System.EventHandler(this.bBackupWorld_Click);
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.Location = new System.Drawing.Point(616, 436);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(507, 249);
            this.tbLog.TabIndex = 24;
            // 
            // bGenerate
            // 
            this.bGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGenerate.ForeColor = System.Drawing.Color.DarkGreen;
            this.bGenerate.Location = new System.Drawing.Point(614, 390);
            this.bGenerate.Name = "bGenerate";
            this.bGenerate.Size = new System.Drawing.Size(507, 40);
            this.bGenerate.TabIndex = 26;
            this.bGenerate.Text = "GENERATE WORLD";
            this.bGenerate.UseVisualStyleBackColor = true;
            this.bGenerate.Click += new System.EventHandler(this.bGenerate_Click);
            // 
            // rbScarse
            // 
            this.rbScarse.AutoSize = true;
            this.rbScarse.Location = new System.Drawing.Point(6, 43);
            this.rbScarse.Name = "rbScarse";
            this.rbScarse.Size = new System.Drawing.Size(150, 17);
            this.rbScarse.TabIndex = 2;
            this.rbScarse.Text = "Scarse Islands (24 is. max)";
            this.rbScarse.UseVisualStyleBackColor = true;
            // 
            // rbSea
            // 
            this.rbSea.AutoSize = true;
            this.rbSea.Location = new System.Drawing.Point(6, 66);
            this.rbSea.Name = "rbSea";
            this.rbSea.Size = new System.Drawing.Size(129, 17);
            this.rbSea.TabIndex = 1;
            this.rbSea.Text = "Open Sea (10 is. max)";
            this.rbSea.UseVisualStyleBackColor = true;
            // 
            // rbDense
            // 
            this.rbDense.AutoSize = true;
            this.rbDense.Checked = true;
            this.rbDense.Location = new System.Drawing.Point(7, 20);
            this.rbDense.Name = "rbDense";
            this.rbDense.Size = new System.Drawing.Size(113, 17);
            this.rbDense.TabIndex = 0;
            this.rbDense.TabStop = true;
            this.rbDense.Text = "Dense Atoll (48 is.)";
            this.rbDense.UseVisualStyleBackColor = true;
            // 
            // gbWorldType
            // 
            this.gbWorldType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWorldType.Controls.Add(this.rbScarse);
            this.gbWorldType.Controls.Add(this.rbSea);
            this.gbWorldType.Controls.Add(this.rbDense);
            this.gbWorldType.Location = new System.Drawing.Point(972, 233);
            this.gbWorldType.Name = "gbWorldType";
            this.gbWorldType.Size = new System.Drawing.Size(158, 100);
            this.gbWorldType.TabIndex = 25;
            this.gbWorldType.TabStop = false;
            this.gbWorldType.Text = "World type";
            // 
            // clbMaps
            // 
            this.clbMaps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbMaps.CheckOnClick = true;
            this.clbMaps.FormattingEnabled = true;
            this.clbMaps.Location = new System.Drawing.Point(7, 282);
            this.clbMaps.Name = "clbMaps";
            this.clbMaps.Size = new System.Drawing.Size(601, 319);
            this.clbMaps.TabIndex = 23;
            this.clbMaps.MouseDown += new System.Windows.Forms.MouseEventHandler(this.clbMaps_MouseDown);
            // 
            // tbBigIslandsProportion
            // 
            this.tbBigIslandsProportion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBigIslandsProportion.Location = new System.Drawing.Point(718, 343);
            this.tbBigIslandsProportion.Name = "tbBigIslandsProportion";
            this.tbBigIslandsProportion.Size = new System.Drawing.Size(248, 45);
            this.tbBigIslandsProportion.TabIndex = 12;
            this.tbBigIslandsProportion.Value = 2;
            this.tbBigIslandsProportion.Visible = false;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(620, 344);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 35;
            this.label5.Text = "More small islands";
            this.label5.Visible = false;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(972, 344);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 36;
            this.label6.Text = "More big islands";
            this.label6.Visible = false;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(421, 238);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(346, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "If you select ALL islands, the algorithm will randomly pick in the selection";
            // 
            // WorldRandomizerTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbBigIslandsProportion);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bHowto);
            this.Controls.Add(this.gbWorldOptions);
            this.Controls.Add(this.bRestore);
            this.Controls.Add(this.bUnselectAll);
            this.Controls.Add(this.bSelectAll);
            this.Controls.Add(this.bDeleteWorld);
            this.Controls.Add(this.bBackupWorld);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.bGenerate);
            this.Controls.Add(this.gbWorldType);
            this.Controls.Add(this.clbMaps);
            this.Name = "WorldRandomizerTool";
            this.Size = new System.Drawing.Size(1139, 712);
            this.Load += new System.EventHandler(this.WorldRandomizerTool_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuOpenDirectory.ResumeLayout(false);
            this.gbWorldOptions.ResumeLayout(false);
            this.gbWorldOptions.PerformLayout();
            this.gbWorldType.ResumeLayout(false);
            this.gbWorldType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbBigIslandsProportion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDataDirectory;
        private System.Windows.Forms.Button bSearchDataDirectory;
        private System.Windows.Forms.TextBox tbMapsDirectory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbSDDefaultDirectory;
        private System.Windows.Forms.TextBox tbMissionsPath;
        private System.Windows.Forms.Button bSteam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bHowto;
        private System.Windows.Forms.ToolStripMenuItem tsiOpenMapDirectory;
        private System.Windows.Forms.ContextMenuStrip contextMenuOpenDirectory;
        private System.Windows.Forms.GroupBox gbWorldOptions;
        private System.Windows.Forms.CheckBox cbUseMissions;
        private System.Windows.Forms.CheckBox cbMixMaps;
        private System.Windows.Forms.CheckBox cbDeep;
        private System.Windows.Forms.Button bRestore;
        private System.Windows.Forms.Button bUnselectAll;
        private System.Windows.Forms.Button bSelectAll;
        private System.Windows.Forms.Button bDeleteWorld;
        private System.Windows.Forms.Button bBackupWorld;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button bGenerate;
        private System.Windows.Forms.RadioButton rbScarse;
        private System.Windows.Forms.RadioButton rbSea;
        private System.Windows.Forms.RadioButton rbDense;
        private System.Windows.Forms.GroupBox gbWorldType;
        private System.Windows.Forms.CheckedListBox clbMaps;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rbSlot2;
        private System.Windows.Forms.RadioButton rbSlot1;
        private System.Windows.Forms.RadioButton rbSlot0;
        private System.Windows.Forms.RadioButton rbSlot3;
        private System.Windows.Forms.Button bRefreshSlots;
        private System.Windows.Forms.TrackBar tbBigIslandsProportion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbWorkshopPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}
